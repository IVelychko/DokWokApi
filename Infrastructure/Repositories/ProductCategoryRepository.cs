using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Helpers;
using Domain.Models;
using Infrastructure.Specification;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly StoreDbContext _context;

    public ProductCategoryRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> AddAsync(ProductCategory entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("productCategory", "The passed entity is null");
            return Result<Unit>.Failure(error);
        }

        await _context.AddAsync(entity);
        return Unit.Default;
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await _context.ProductCategories.FirstAsync(c => c.Id == id);
        _context.Remove(entity);
    }

    public async Task<IEnumerable<ProductCategory>> GetAllBySpecificationAsync(Specification<ProductCategory> specification)
    {
        var query = SpecificationEvaluator.ApplySpecification(_context.ProductCategories, specification);
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<ProductCategory>> GetAllAsync(PageInfo? pageInfo = null)
    {
        IQueryable<ProductCategory> query = _context.ProductCategories;
        if (pageInfo is not null)
        {
            query = SpecificationEvaluator.ApplyPagination(query, pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<ProductCategory?> GetByIdAsync(long id)
    {
        return await _context.ProductCategories.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Result<bool>> IsNameTakenAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            var error = new ValidationError(nameof(name), "The passed name is null or empty");
            return Result<bool>.Failure(error);
        }

        var isTaken = await _context.ProductCategories.AnyAsync(c => c.Name == name);
        return isTaken;
    }

    public Result<Unit> Update(ProductCategory entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("productCategory", "The passed entity is null");
            return Result<Unit>.Failure(error);
        }

        _context.Update(entity);
        return Unit.Default;
    }
}
