using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure.Security;

public class JwtSettings(IConfiguration configuration)
{
    private static readonly string Section = "Jwt"; 

    public string Issuer => configuration[$"{Section}:Issuer"] ?? throw new InvalidOperationException("Issuer not set.");
    public string Audience => configuration[$"{Section}:Audience"] ?? throw new InvalidOperationException("Audience not set.");
    public string SecretKey => configuration[$"{Section}:SecretKey"] ?? throw new InvalidOperationException("Secret key not set.");
    public int ExpirationInMinutes => configuration.GetSection(Section).GetValue<int>("ExpiryInMinutes");
    public int RefreshTokenDays => configuration.GetSection(Section).GetValue<int>("RefreshTokenDays");

    public SymmetricSecurityKey GetSymmetricKey() =>
        new(Encoding.UTF8.GetBytes(SecretKey));
}