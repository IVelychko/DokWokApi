using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Validation;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validation;

public class ProductRepositoryValidator : IProductRepositoryValidator
{
    private readonly StoreDbContext _context;

    public ProductRepositoryValidator(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<ValidationResult> ValidateAddAsync(Product? model)
    {
        ValidationResult result = new(true);
        if (model is null)
        {
            result.IsValid = false;
            result.Errors.Add("The passed product is null");
            return result;
        }

        var category = await _context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == model.CategoryId);
        if (category is null)
        {
            result.IsValid = false;
            result.Errors.Add("There is no product category with the ID specified in the CategoryId property of the Product entity");
        }

        if (await _context.Products.AnyAsync(p => p.Name == model.Name))
        {
            result.IsValid = false;
            result.Errors.Add("The product with the same Name value is already present in the database");
        }

        return result;
    }

    public async Task<ValidationResult> ValidateUpdateAsync(Product? model)
    {
        ValidationResult result = new(true);
        if (model is null)
        {
            result.IsValid = false;
            result.Errors.Add("The passed product is null");
            return result;
        }

        var category = await _context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == model.CategoryId);
        if (category is null)
        {
            result.IsValid = false;
            result.Errors.Add("There is no product category with the ID specified in the CategoryId property of the Product entity");
        }

        var entityToUpdate = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == model.Id);
        if (entityToUpdate is null)
        {
            result.IsValid = false;
            result.IsNotFound = true;
            result.Errors.Add("There is no entity with this ID in the database");
            return result;
        }

        if (model.Name != entityToUpdate.Name && await _context.Products.AnyAsync(p => p.Name == model.Name))
        {
            result.IsValid = false;
            result.Errors.Add("The product with the same Name value is already present in the database");
        }

        return result;
    }
}
