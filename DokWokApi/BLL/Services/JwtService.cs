using DokWokApi.BLL.Interfaces;
using DokWokApi.DAL.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DokWokApi.BLL.Services
{
    public class JwtService : ISecurityTokenService<ApplicationUser>
    {
        private readonly IConfiguration config;

        public JwtService(IConfiguration configuration)
        {
            config = configuration;
        }

        public string CreateToken(ApplicationUser user)
        {
            const int ExpirationDays = 1;
            DateTime expiration = DateTime.UtcNow.AddDays(ExpirationDays);

            // generate token that is valid for 7 days
            var token = new JwtSecurityToken(
                config["Jwt:Issuer"]!,
                config["Jwt:Audience"]!,
                CreateClaims(user),
                expires: expiration,
                signingCredentials: CreateSigningCredentials());

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        private Claim[] CreateClaims(ApplicationUser user)
        {
            return [
                new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("id", user.Id),
                new Claim("username", user.UserName ?? string.Empty),
            ];
        }

        private SigningCredentials CreateSigningCredentials()
        {
            return new(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
                SecurityAlgorithms.HmacSha256
            );
        }
    }
}
