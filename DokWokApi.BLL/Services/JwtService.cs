﻿using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.User;
using Microsoft.Extensions.Configuration;
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

        private const int ExpirationMinutes = 1;

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

        public string CreateSerializedToken(UserModel user, IEnumerable<string> roles)
        {
            var token = CreateToken(user, roles);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public JwtSecurityToken CreateToken(UserModel user, IEnumerable<string> roles)
        {
            DateTime expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);

            var encodedKey = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
            SymmetricSecurityKey securityKey = new(encodedKey);
            SigningCredentials tokenSigningCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = [
                new(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
                new("id", user.Id),
                new("username", user.UserName ?? string.Empty),
                new("role", JsonSerializer.Serialize(roles), JsonClaimValueTypes.JsonArray),
            ];

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"]!,
                _configuration["Jwt:Audience"]!,
                claims,
                expires: expiration,
                signingCredentials: tokenSigningCredentials);

            return token;
        }
    }
}
