using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AppCore.Dto;
using AppCore.Services;
using System.Security.Claims;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try 
        {
            var response = await authService.LoginAsync(dto);
            return Ok(response);
        }
        catch (Exception ex) 
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        try 
        {
            var response = await authService.RefreshTokenAsync(dto);
            return Ok(response);
        }
        catch (Exception ex) 
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult> Revoke([FromBody] string refreshToken)
    {
        await authService.RevokeTokenAsync(refreshToken);
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var user = new UserDto
        {
            Id = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
            Email = User.FindFirstValue(ClaimTypes.Email)!,
            FirstName = User.FindFirstValue(ClaimTypes.GivenName)!,
            LastName = User.FindFirstValue(ClaimTypes.Surname)!,
            Department = User.FindFirstValue("department")!,
            Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value)
        };
        return Ok(user);
    }
}