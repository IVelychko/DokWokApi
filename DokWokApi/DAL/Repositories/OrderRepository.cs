using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.DAL.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly StoreDbContext context;

    private readonly UserManager<ApplicationUser> userManager;

    public OrderRepository(StoreDbContext context, UserManager<ApplicationUser> userManager)
    {
        this.context = context;
        this.userManager = userManager;
    }

    public async Task<Order> AddAsync(Order entity)
    {
        RepositoryHelper.CheckForNull(entity, "The passed entity is null.");
        if (entity.UserId is not null)
        {
            var user = await userManager.FindByIdAsync(entity.UserId);
            RepositoryHelper.CheckRetrievedEntity(user, "There is no user with the ID specified in the UserId property of the Order entity.");
        }

        await context.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Order entity)
    {
        RepositoryHelper.CheckForNull(entity, "The passed entity is null.");
        var entityToDelete = await context.FindAsync<Order>(entity.Id);
        RepositoryHelper.CheckRetrievedEntity(entityToDelete, "There is no entity with this ID in the database.");

        context.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await context.FindAsync<Order>(id);
        entity = RepositoryHelper.CheckRetrievedEntity(entity, "There is no entity with this ID in the database.");

        context.Remove(entity);
        await context.SaveChangesAsync();
    }

    public IQueryable<Order> GetAll()
    {
        return context.Orders;
    }

    public IQueryable<Order> GetAllWithDetails()
    {
        return context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderLines);
    }

    public async Task<Order?> GetByIdAsync(long id)
    {
        return await context.FindAsync<Order>(id);
    }

    public async Task<Order?> GetByIdWithDetailsAsync(long id)
    {
        return await context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                    .ThenInclude(p => p!.Category)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order> UpdateAsync(Order entity)
    {
        RepositoryHelper.CheckForNull(entity, "The passed entity is null.");
        var entityToUpdate = await context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == entity.Id);
        RepositoryHelper.CheckRetrievedEntity(entityToUpdate, "There is no entity with this ID in the database.");
        if (entity.UserId is not null)
        {
            var user = await userManager.FindByIdAsync(entity.UserId);
            RepositoryHelper.CheckRetrievedEntity(user, "There is no user with the ID specified in the UserId property of the Order entity.");
        }

        context.Update(entity);
        await context.SaveChangesAsync();
        return entity;
    }
}
