using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Validation;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validation;

public class ShopRepositoryValidator : IValidator<Shop>
{
    private readonly StoreDbContext _context;

    public ShopRepositoryValidator(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<ValidationResult> ValidateAddAsync(Shop? model)
    {
        ValidationResult result = new(true);
        if (model is null)
        {
            result.IsValid = false;
            result.Errors.Add("The passed product category is null");
            return result;
        }

        if (await _context.Shops.AnyAsync(s => s.Street == model.Street && s.Building == model.Building))
        {
            result.IsValid = false;
            result.Errors.Add("The shop with the same Street and Building values is already present in the database");
        }

        return result;
    }

    public async Task<ValidationResult> ValidateUpdateAsync(Shop? model)
    {
        ValidationResult result = new(true);
        if (model is null)
        {
            result.IsValid = false;
            result.Errors.Add("The passed product category is null");
            return result;
        }

        var entityToUpdate = await _context.Shops.AsNoTracking().FirstOrDefaultAsync(s => s.Id == model.Id);
        if (entityToUpdate is null)
        {
            result.IsValid = false;
            result.Errors.Add("There is no entity with this ID in the database");
            return result;
        }

        if ((model.Street != entityToUpdate.Street || model.Building != entityToUpdate.Building) &&
            await _context.Shops.AnyAsync(s => s.Street == model.Street && s.Building == model.Building))
        {
            result.IsValid = false;
            result.Errors.Add("The shop with the same Street and Building values is already present in the database");
        }

        return result;
    }
}
