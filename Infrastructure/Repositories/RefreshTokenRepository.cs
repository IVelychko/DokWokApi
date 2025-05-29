using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly StoreDbContext _context;

    public RefreshTokenRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken entity)
    {
        Ensure.ArgumentNotNull(entity);
        await _context.AddAsync(entity);
    }

    public void Delete(RefreshToken entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Remove(entity);
    }

    public async Task<IList<RefreshToken>> GetAllAsync()
    {
        return await _context.RefreshTokens.ToListAsync();
    }

    public async Task<RefreshToken?> GetByIdAsync(long id)
    {
        return await _context.FindAsync<RefreshToken>(id);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(token, nameof(token));
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
    }
    
    public async Task<RefreshToken?> GetByTokenAsNoTrackingAsync(string token)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(token, nameof(token));
        return await _context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }
    
    public async Task<bool> RefreshTokenExistsAsync(string token)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(token, nameof(token));
        return await _context.RefreshTokens.AnyAsync(x => x.Token == token);
    }

    public void Update(RefreshToken entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Update(entity);
    }
}
