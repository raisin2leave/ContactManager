using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AppCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AppCore.Dto;
using AppCore.Services;
using AppCore.Entities;
using Infrastructure.EntityFramework.Context;
using Infrastructure.EntityFramework.Entities;
using Infrastructure.Security;

namespace Infrastructure.Security;

public class AuthService(
    UserManager<CrmUser> userManager,
    ContactsDbContext context,
    JwtSettings jwtOptions) : IAuthService
{
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email) 
            ?? throw new Exception("Invalid email or password.");

        if (!await userManager.CheckPasswordAsync(user, dto.Password))
        {
            await userManager.AccessFailedAsync(user);
            throw new Exception("Invalid email or password.");
        }

        if (user.Status != SystemUserStatus.Active) throw new Exception("Account inactive.");
        
        await userManager.ResetAccessFailedCountAsync(user);
        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
    {
        var principal = GetPrincipalFromExpiredToken(dto.AccessToken);
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("Invalid token.");
        var user = await userManager.FindByIdAsync(userId) ?? throw new Exception("User not found.");

        var refreshToken = await context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == dto.RefreshToken && t.UserId == userId)
            ?? throw new Exception("Invalid refresh token.");

        if (!refreshToken.IsActive) throw new Exception("Token expired or revoked.");

        var newResponse = await GenerateAuthResponseAsync(user);
        refreshToken.Revoke(newResponse.RefreshToken);
        await context.SaveChangesAsync();

        return newResponse;
    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        var token = await context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken)
            ?? throw new Exception("Token does not exist.");
        
        token.Revoke();
        await context.SaveChangesAsync();
    }

    private async Task<AuthResponseDto> GenerateAuthResponseAsync(CrmUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var accessToken = GenerateAccessToken(user, roles);
        var refreshToken = await GenerateRefreshTokenAsync(user.Id);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(jwtOptions.ExpirationInMinutes),
            User = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Department = user.Department,
                Status = user.Status,
                Roles = roles
            }
        };
    }

    private string GenerateAccessToken(CrmUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new("department", user.Department),
            new("status", user.Status.ToString()), // Added for the ActiveUser policy
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtOptions.ExpirationInMinutes),
            signingCredentials: new SigningCredentials(jwtOptions.GetSymmetricKey(), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<RefreshToken> GenerateRefreshTokenAsync(string userId)
    {
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresAt = DateTime.UtcNow.AddDays(jwtOptions.RefreshTokenDays)
        };

        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();
        return refreshToken;
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = jwtOptions.GetSymmetricKey(),
            ValidateLifetime = false 
        };

        var principal = new JwtSecurityTokenHandler().ValidateToken(accessToken, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}