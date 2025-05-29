using Domain.Entities;
using Domain.Shared;
using Domain.Specifications.Products;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Specifications.Products;

public static class ProductSpecificationEvaluator
{
    public static IQueryable<Product> ApplySpecification(
        IQueryable<Product> initialQuery, ProductSpecification specification)
    {
        var query = initialQuery;
        query = Include(query, specification);
        query = ApplyId(query, specification.Id);
        query = ApplyCategoryId(query, specification.CategoryId);
        query = ApplyMinPrice(query, specification.MinPrice);
        query = ApplyMaxPrice(query, specification.MaxPrice);
        query = ApplyPagination(query, specification.PageInfo);
        
        return query;
    }
    
    private static IQueryable<Product> Include(IQueryable<Product> query, ProductSpecification specification)
    {
        return specification.IncludeCategory ?
            query.Include(p => p.Category) :
            query;
    }
    
    private static IQueryable<Product> ApplyPagination(IQueryable<Product> query, PageInfo? pageInfo)
    {
        if (pageInfo is null)
        {
            return query;
        }

        var itemsToSkip = (pageInfo.PageNumber - 1) * pageInfo.PageSize;
        return query
            .Skip(itemsToSkip)
            .Take(pageInfo.PageSize);
    }

    private static IQueryable<Product> ApplyId(IQueryable<Product> query, long? id)
    {
        return id.HasValue ?
            query.Where(p => p.Id == id.Value) :
            query;
    }
    
    private static IQueryable<Product> ApplyCategoryId(IQueryable<Product> query, long? categoryId)
    {
        return categoryId.HasValue ?
            query.Where(p => p.CategoryId == categoryId.Value) :
            query;
    }
    
    private static IQueryable<Product> ApplyMinPrice(IQueryable<Product> query, decimal? minPrice)
    {
        if (minPrice is null)
        {
            return query;
        }
        
        return query
            .Where(p => p.Price >= minPrice);
    }
    
    private static IQueryable<Product> ApplyMaxPrice(IQueryable<Product> query, decimal? maxPrice)
    {
        if (maxPrice is null)
        {
            return query;
        }
        
        return query
            .Where(p => p.Price <= maxPrice);
    }
}