using Domain.Abstractions.Repositories;

namespace Infrastructure.Repositories;

public class UnitOfWork(StoreDbContext context) : IUnitOfWork
{
    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
