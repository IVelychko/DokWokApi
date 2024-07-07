using Microsoft.IdentityModel.Tokens;

namespace DokWokApi.BLL.Interfaces;

public interface ISecurityTokenService<in TUser, out IToken> 
    where TUser : class
    where IToken : SecurityToken
{
    string CreateToken(TUser user, IEnumerable<string> roles);

    IToken ValidateToken(string token);
}
