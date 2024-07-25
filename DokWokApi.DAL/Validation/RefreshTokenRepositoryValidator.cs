using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.DAL.Validation;

public class RefreshTokenRepositoryValidator : IValidator<RefreshToken>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly StoreDbContext _context;

    public RefreshTokenRepositoryValidator(UserManager<ApplicationUser> userManager, StoreDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<ValidationResult> ValidateAddAsync(RefreshToken? model)
    {
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (model is null)
        {
            result.IsValid = false;
            result.Error = "The passed product is null";
            return result;
        }

        var userExists = await _userManager.Users.AsNoTracking().AnyAsync(u => u.Id == model.UserId);
        if (!userExists)
        {
            result.IsValid = false;
            result.IsFound = false;
            result.Error = "There is no user with the ID specified in the UserId property of the RefreshToken entity";
            return result;
        }

        return result;
    }

    public async Task<ValidationResult> ValidateUpdateAsync(RefreshToken? model)
    {
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (model is null)
        {
            result.IsValid = false;
            result.Error = "The passed product is null";
            return result;
        }

        var entityToUpdate = await _context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(rt => rt.Id == model.Id);
        if (entityToUpdate is null)
        {
            result.IsValid = false;
            result.IsFound = false;
            result.Error = "There is no refresh token with this ID in the database";
            return result;
        }

        var userExists = await _userManager.Users.AsNoTracking().AnyAsync(u => u.Id == model.UserId);
        if (!userExists)
        {
            result.IsValid = false;
            result.IsFound = false;
            result.Error = "There is no user with the ID specified in the UserId property of the RefreshToken entity";
            return result;
        }

        return result;
    }
}
