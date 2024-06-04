using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.DAL.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly StoreDbContext _context;

    private readonly UserManager<ApplicationUser> _userManager;

    public OrderRepository(StoreDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Order> AddAsync(Order entity)
    {
        RepositoryHelper.ThrowIfNull(entity, "The passed entity is null.");
        if (entity.UserId is not null)
        {
            var user = await _userManager.FindByIdAsync(entity.UserId);
            RepositoryHelper.ThrowEntityNotFoundIfNull(user, "There is no user with the ID specified in the UserId property of the Order entity.");
        }

        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Order entity)
    {
        RepositoryHelper.ThrowIfNull(entity, "The passed entity is null.");
        var entityToDelete = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == entity.Id);
        RepositoryHelper.ThrowEntityNotFoundIfNull(entityToDelete, "There is no entity with this ID in the database.");

        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        entity = RepositoryHelper.ThrowEntityNotFoundIfNull(entity, "There is no entity with this ID in the database.");

        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public IQueryable<Order> GetAll()
    {
        return _context.Orders.AsNoTracking();
    }

    public IQueryable<Order> GetAllWithDetails()
    {
        return _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                    .ThenInclude(p => p!.Category).AsNoTracking();
    }

    public async Task<Order?> GetByIdAsync(long id)
    {
        return await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order?> GetByIdWithDetailsAsync(long id)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                    .ThenInclude(p => p!.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order> UpdateAsync(Order entity)
    {
        RepositoryHelper.ThrowIfNull(entity, "The passed entity is null.");
        var entityToUpdate = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == entity.Id);
        RepositoryHelper.ThrowEntityNotFoundIfNull(entityToUpdate, "There is no entity with this ID in the database.");
        if (entity.UserId is not null)
        {
            var user = await _userManager.FindByIdAsync(entity.UserId);
            RepositoryHelper.ThrowEntityNotFoundIfNull(user, "There is no user with the ID specified in the UserId property of the Order entity.");
        }

        _context.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
