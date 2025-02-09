using Domain.Abstractions.Repositories;

namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly StoreDbContext _context;

    public UnitOfWork(StoreDbContext context) => _context = context;

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
