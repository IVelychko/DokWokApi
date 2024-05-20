using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.User;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DokWokApi.BLL.Services
{
    public class JwtService : ISecurityTokenService<UserModel, JwtSecurityToken>
    {
        private readonly IConfiguration config;

        public JwtService(IConfiguration configuration)
        {
            config = configuration;
        }

        public JwtSecurityToken ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            return jwtToken;
        }

        public string CreateToken(UserModel user)
        {
            const int ExpirationDays = 1;
            DateTime expiration = DateTime.UtcNow.AddDays(ExpirationDays);

            var encodedKey = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);
            SymmetricSecurityKey securityKey = new(encodedKey);
            SigningCredentials tokenSigningCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = [
                new(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new("id", user.Id ?? string.Empty),
                new("username", user.UserName ?? string.Empty),
            ];

            var token = new JwtSecurityToken(
                config["Jwt:Issuer"]!,
                config["Jwt:Audience"]!,
                claims,
                expires: expiration,
                signingCredentials: tokenSigningCredentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}
