using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.DAL;

public static class SeedIdentityData
{
    private static IdentityRole[] roles = [
            new IdentityRole { Name = "Admin" },
            new IdentityRole { Name = "Customer" }
        ];

    public static async Task SeedIdentityDatabase(IApplicationBuilder app)
    {
        var roleManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<IdentityContext>();
        await context.Database.MigrateAsync();
        if (!roleManager.Roles.Any())
        {
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }
        }
    }
}
