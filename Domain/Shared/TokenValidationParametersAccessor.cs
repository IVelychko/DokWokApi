using Microsoft.IdentityModel.Tokens;
using System.Text;
using Domain.DTOs.Models.Jwt;

namespace Domain.Shared;

public class TokenValidationParametersAccessor
{
    private readonly JwtConfiguration _jwtConfiguration;
    
    public TokenValidationParametersAccessor(JwtConfiguration jwtConfiguration)
    {
        _jwtConfiguration = jwtConfiguration;
    }

    public TokenValidationParameters Regular => new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = _jwtConfiguration.Issuer,
        ValidAudience = _jwtConfiguration.Audience,
        IssuerSigningKey = GetSymmetricSecurityKey(),
        ClockSkew = TimeSpan.Zero,
    };
    
    public TokenValidationParameters Refresh => new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = _jwtConfiguration.Issuer,
        ValidAudience = _jwtConfiguration.Audience,
        IssuerSigningKey = GetSymmetricSecurityKey(),
        ClockSkew = TimeSpan.Zero,
    };
    
    private SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        var encodedKey = Encoding.UTF8.GetBytes(_jwtConfiguration.Key);
        return new SymmetricSecurityKey(encodedKey);
    }
}
