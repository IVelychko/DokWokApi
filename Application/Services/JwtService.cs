using Domain.Abstractions.Services;
using Domain.Entities;
using Domain.Shared;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.DTOs.Models.Jwt;

namespace Application.Services;

public class JwtService : ISecurityTokenService<User, JwtSecurityToken>
{
    private readonly JwtConfiguration _jwtConfiguration;

    public JwtService(JwtConfiguration jwtConfiguration)
    {
        _jwtConfiguration = jwtConfiguration;
    }

    public JwtSecurityToken ValidateToken(string token, TokenValidationParameters tokenValidationParameters)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(token, nameof(token));
        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
        return (JwtSecurityToken)validatedToken;
    }

    public bool IsTokenSecurityAlgorithmValid(JwtSecurityToken securityToken)
    {
        return securityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
    }

    public string CreateSerializedToken(User user, string role)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(role, nameof(role));
        var token = CreateToken(user, role);
        JwtSecurityTokenHandler tokenHandler = new();
        return tokenHandler.WriteToken(token);
    }
    
    public JwtSecurityToken CreateToken(User user, string role)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(role, nameof(role));
        var claims = GetClaims(user, role);
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = GetExpirationTime(),
            Issuer = _jwtConfiguration.Issuer,
            Audience = _jwtConfiguration.Audience,
            SigningCredentials = GetSigningCredentials(),
        };
        JwtSecurityTokenHandler tokenHandler = new();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return (JwtSecurityToken)token;
    }
    
    private DateTime GetExpirationTime()
    {
        var tokenLifeTime = _jwtConfiguration.Lifetime.Split(':');
        var hours = int.Parse(tokenLifeTime[0]);
        var minutes = int.Parse(tokenLifeTime[1]);
        var seconds = int.Parse(tokenLifeTime[2]);
        return DateTime.UtcNow
            .AddHours(hours)
            .AddMinutes(minutes)
            .AddSeconds(seconds);
    }
    
    private SigningCredentials GetSigningCredentials()
    {
        var encodedKey = Encoding.UTF8.GetBytes(_jwtConfiguration.Key);
        SymmetricSecurityKey securityKey = new(encodedKey);
        return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    }
    
    private Claim[] GetClaims(User user, string role)
    {
        Claim[] claims = [
            new(JwtRegisteredClaimNames.Sub, _jwtConfiguration.Subject),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
            new("id", user.Id.ToString()),
            new(ClaimTypes.Role, role)
        ];
        return claims;
    }
}
