using Domain.Abstractions.Services;
using Domain.Exceptions;
using Domain.Models.User;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Domain.Services;

public class JwtService : ISecurityTokenService<UserModel, JwtSecurityToken>
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public JwtSecurityToken ValidateToken(string token, TokenValidationParameters tokenValidationParameters)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        return jwtToken;
    }

    public bool IsTokenSecurityAlgorithmValid(JwtSecurityToken securityToken)
    {
        return securityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
    }

    public string CreateSerializedToken(UserModel user, string role)
    {
        var token = CreateToken(user, role);

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Blocker Vulnerability", "S6781:JWT secret keys should not be disclosed", Justification = "The secret is not visible")]
    public JwtSecurityToken CreateToken(UserModel user, string role)
    {
        var errorMessage = "Unable to get data from configuration";
        var tokenLifeTime = _configuration["Jwt:TokenLifeTime"]?.Split(':')
            ?? throw new ConfigurationException(errorMessage);
        var hours = int.Parse(tokenLifeTime[0]);
        var minutes = int.Parse(tokenLifeTime[1]);
        var seconds = int.Parse(tokenLifeTime[2]);
        DateTime expiration = DateTime.UtcNow
            .AddHours(hours)
            .AddMinutes(minutes)
            .AddSeconds(seconds);

        var key = _configuration["Jwt:Key"] ?? throw new ConfigurationException(errorMessage);
        var encodedKey = Encoding.UTF8.GetBytes(key);
        SymmetricSecurityKey securityKey = new(encodedKey);
        SigningCredentials tokenSigningCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

        var subject = _configuration["Jwt:Subject"] ?? throw new ConfigurationException(errorMessage);
        Claim[] claims = [
            new(JwtRegisteredClaimNames.Sub, subject),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
            new("id", user.Id.ToString()),
            new(ClaimTypes.Role, role)
        ];

        var issuer = _configuration["Jwt:Issuer"] ?? throw new ConfigurationException(errorMessage);
        var audience = _configuration["Jwt:Audience"] ?? throw new ConfigurationException(errorMessage);
        JwtSecurityToken token = new(
            issuer,
            audience,
            claims,
            expires: expiration,
            signingCredentials: tokenSigningCredentials
        );

        return token;
    }
}
