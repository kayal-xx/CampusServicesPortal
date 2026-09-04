using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CampusServicesPortal.Api.Entities;
using Microsoft.IdentityModel.Tokens;

namespace CampusServicesPortal.Api.Security;

public class JwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTime ExpiresAt) GenerateToken(
        Student student)
    {
        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "JWT key is not configured."
            );

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var expiryMinutes = int.TryParse(
            _configuration["Jwt:ExpiryMinutes"],
            out var configuredMinutes
        )
            ? configuredMinutes
            : 60;

        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new List<Claim>
        {
            new(
                ClaimTypes.NameIdentifier,
                student.Id.ToString()
            ),
            new(
                ClaimTypes.Name,
                student.FullName
            ),
            new(
                ClaimTypes.Email,
                student.Email
            ),
            new(
                ClaimTypes.Role,
                student.Role
            ),
            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()
            )
        };

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        );

        var signingCredentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256
        );

        var jwtToken = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: signingCredentials
        );

        var token = new JwtSecurityTokenHandler()
            .WriteToken(jwtToken);

        return (token, expiresAt);
    }
}