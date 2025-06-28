using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MentorMenteeApp.API.Configuration;
using MentorMenteeApp.API.Models.Entities;
using MentorMenteeApp.API.Services.Interfaces;

namespace MentorMenteeApp.API.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;

    public JwtService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        var now = DateTime.UtcNow;
        var jti = Guid.NewGuid().ToString();

        var claims = new List<Claim>
        {
            // RFC 7519 Standard Claims
            new(JwtRegisteredClaimNames.Iss, _jwtSettings.Issuer),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Aud, _jwtSettings.Audience),
            new(JwtRegisteredClaimNames.Exp, new DateTimeOffset(now.AddHours(_jwtSettings.ExpirationInHours)).ToUnixTimeSeconds().ToString()),
            new(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(now).ToUnixTimeSeconds().ToString()),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString()),
            new(JwtRegisteredClaimNames.Jti, jti),
            
            // Custom Claims
            new("name", user.Name),
            new("email", user.Email),
            new("role", user.Role.ToString().ToLowerInvariant()),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = now.AddHours(_jwtSettings.ExpirationInHours),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}
