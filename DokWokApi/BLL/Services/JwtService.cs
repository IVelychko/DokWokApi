using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.User;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace DokWokApi.BLL.Services
{
    public class JwtService : ISecurityTokenService<UserModel, JwtSecurityToken>
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JwtSecurityToken ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            return jwtToken;
        }

        public string CreateToken(UserModel user, IEnumerable<string> roles)
        {
            const int ExpirationDays = 1;
            DateTime expiration = DateTime.UtcNow.AddDays(ExpirationDays);

            var encodedKey = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
            SymmetricSecurityKey securityKey = new(encodedKey);
            SigningCredentials tokenSigningCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = [
                new(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
                new("id", user.Id ?? string.Empty),
                new("username", user.UserName ?? string.Empty),
                new("role", JsonSerializer.Serialize(roles), JsonClaimValueTypes.JsonArray),
            ];

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"]!,
                _configuration["Jwt:Audience"]!,
                claims,
                expires: expiration,
                signingCredentials: tokenSigningCredentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}
