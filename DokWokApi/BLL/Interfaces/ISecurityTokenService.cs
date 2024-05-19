using Microsoft.AspNetCore.Identity;

namespace DokWokApi.BLL.Interfaces;

public interface ISecurityTokenService<in TIdentity> where TIdentity : IdentityUser
{
    string CreateToken(TIdentity user);
}
