using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validation;

public class RefreshTokenRepositoryValidator : IRefreshTokenRepositoryValidator
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
        ValidationResult result = new(true);
        if (model is null)
        {
            result.IsValid = false;
            result.Errors.Add("The passed product is null");
            return result;
        }

        var userExists = await _userManager.Users.AsNoTracking().AnyAsync(u => u.Id == model.UserId);
        if (!userExists)
        {
            result.IsValid = false;
            result.Errors.Add("There is no user with the ID specified in the UserId property of the RefreshToken entity");
        }

        return result;
    }

    public async Task<ValidationResult> ValidateUpdateAsync(RefreshToken? model)
    {
        ValidationResult result = new(true);
        if (model is null)
        {
            result.IsValid = false;
            result.Errors.Add("The passed product is null");
            return result;
        }

        var entityToUpdate = await _context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(rt => rt.Id == model.Id);
        if (entityToUpdate is null)
        {
            result.IsValid = false;
            result.IsNotFound = true;
            result.Errors.Add("There is no refresh token with this ID in the database");
            return result;
        }

        var userExists = await _userManager.Users.AsNoTracking().AnyAsync(u => u.Id == model.UserId);
        if (!userExists)
        {
            result.IsValid = false;
            result.Errors.Add("There is no user with the ID specified in the UserId property of the RefreshToken entity");
        }

        return result;
    }
}
