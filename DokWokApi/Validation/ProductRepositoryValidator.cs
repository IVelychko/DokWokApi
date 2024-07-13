using DokWokApi.DAL;
using DokWokApi.DAL.Entities;
using DokWokApi.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.Validation;

public class ProductRepositoryValidator : IValidator<Product>
{
    private readonly StoreDbContext _context;

    public ProductRepositoryValidator(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<ValidationResult> ValidateAddAsync(Product model)
    {
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (model is null)
        {
            result.IsValid = false;
            result.Error = "The passed product is null.";
            return result;
        }

        var category = await _context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == model.CategoryId);
        if (category is null)
        {
            result.IsValid = false;
            result.IsFound = false;
            result.Error = "There is no product category with the ID specified in the CategoryId property of the Product entity.";
            return result;
        }

        if (await _context.Products.AnyAsync(p => p.Name == model.Name))
        {
            result.IsValid = false;
            result.Error = "The product with the same Name value is already present in the database.";
            return result;
        }

        return result;
    }

    public async Task<ValidationResult> ValidateUpdateAsync(Product model)
    {
        ValidationResult result = new()
        {
            IsValid = true,
        };
        if (model is null)
        {
            result.IsValid = false;
            result.Error = "The passed product is null.";
            return result;
        }

        var entityToUpdate = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == model.Id);
        if (entityToUpdate is null)
        {
            result.IsValid = false;
            result.IsFound = false;
            result.Error = "There is no entity with this ID in the database.";
            return result;
        }

        var category = await _context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == model.CategoryId);
        if (category is null)
        {
            result.IsValid = false;
            result.IsFound = false;
            result.Error = "There is no product category with the ID specified in the CategoryId property of the Product entity.";
            return result;
        }

        if (model.Name != entityToUpdate!.Name && await _context.Products.AnyAsync(p => p.Name == model.Name))
        {
            result.IsValid = false;
            result.Error = "The product with the same Name value is already present in the database.";
            return result;
        }

        return result;
    }
}
