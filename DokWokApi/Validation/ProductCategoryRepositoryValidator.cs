﻿using DokWokApi.DAL;
using DokWokApi.DAL.Entities;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.Validation;

public class ProductCategoryRepositoryValidator : IValidator<ProductCategory>
{
    private readonly StoreDbContext _context;

    public ProductCategoryRepositoryValidator(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<ValidationResult> ValidateAddAsync(ProductCategory model)
    {
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (model is null)
        {
            result.IsValid = false;
            result.Error = "The passed product category is null.";
            return result;
        }

        if (await _context.ProductCategories.AnyAsync(c => c.Name == model.Name))
        {
            result.IsValid = false;
            result.Error = "The product category with the same Name value is already present in the database.";
            return result;
        }

        return result;
    }

    public async Task<ValidationResult> ValidateUpdateAsync(ProductCategory model)
    {
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (model is null)
        {
            result.IsValid = false;
            result.Error = "The passed product category is null.";
            return result;
        }

        var entityToUpdate = await _context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == model.Id);
        if (entityToUpdate is null)
        {
            result.IsValid = false;
            result.IsFound = false;
            result.Error = "There is no entity with this ID in the database.";
            return result;
        }

        if (model.Name != entityToUpdate.Name && await _context.ProductCategories.AnyAsync(c => c.Name == model.Name))
        {
            result.IsValid = false;
            result.Error = "The product category with the same Name value is already present in the database.";
            return result;
        }

        return result;
    }
}
