using System.IdentityModel.Tokens.Jwt;

namespace Domain.Validation.Users.ExpiredJwt;

public sealed record ExpiredJwtValidationModel(JwtSecurityToken Jwt, bool IsAlgorithmValid);
