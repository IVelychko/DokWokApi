using Domain.Entities;
using Domain.Specifications.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Specifications.Users;

public static class UserSpecificationEvaluator
{
    public static IQueryable<User> ApplySpecification(
        IQueryable<User> initialQuery, UserSpecification specification)
    {
        var query = initialQuery;
        query = ApplyNoTracking(query, specification);
        query = Include(query, specification);
        query = ApplyId(query, specification.Id);
        query = ApplyUserName(query, specification.UserName);
        query = ApplyRoleId(query, specification.RoleId);
        
        return query;
    }
    
    private static IQueryable<User> Include(IQueryable<User> query, UserSpecification specification)
    {
        return specification.IncludeRole ?
            query.Include(u => u.UserRole) :
            query;
    }
    
    private static IQueryable<User> ApplyNoTracking(IQueryable<User> query, UserSpecification specification)
    {
        return specification.NoTracking ?
            query.AsNoTracking() :
            query;
    }
    
    private static IQueryable<User> ApplyId(IQueryable<User> query, long? id)
    {
        return id.HasValue ?
            query.Where(u => u.Id == id.Value) :
            query;
    }
    
    private static IQueryable<User> ApplyUserName(IQueryable<User> query, string? userName)
    {
        return string.IsNullOrWhiteSpace(userName) ?
            query :
            query.Where(u => u.UserName == userName);
    }
    
    private static IQueryable<User> ApplyRoleId(IQueryable<User> query, long? roleId)
    {
        return roleId.HasValue ?
            query.Where(u => u.UserRoleId == roleId.Value) :
            query;
    }
}