using Domain.Entities;

namespace Domain.Abstractions.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<IEnumerable<RefreshToken>> GetAllWithDetailsAsync();

    Task<RefreshToken?> GetByTokenAsync(string token);

    Task<RefreshToken?> GetByJwtIdAsync(string jwtId);

    Task<RefreshToken?> GetByUserIdAsync(string userId);

    Task<RefreshToken?> GetByIdWithDetailsAsync(long id);

    Task<RefreshToken?> GetByTokenWithDetailsAsync(string token);

    Task<RefreshToken?> GetByJwtIdWithDetailsAsync(string jwtId);

    Task<RefreshToken?> GetByUserIdWithDetailsAsync(string userId);
}
