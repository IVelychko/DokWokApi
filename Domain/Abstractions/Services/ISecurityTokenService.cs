using Microsoft.IdentityModel.Tokens;

namespace Domain.Abstractions.Services;

public interface ISecurityTokenService<in TUser, TToken>
    where TUser : class
    where TToken : SecurityToken
{
    string CreateSerializedToken(TUser user, string role);

    TToken CreateToken(TUser user, string role);

    TToken ValidateToken(string token, TokenValidationParameters tokenValidationParameters);

    bool IsTokenSecurityAlgorithmValid(TToken securityToken);
}
