using Microsoft.AspNetCore.Identity;

namespace DokWokApi.DAL.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
}
