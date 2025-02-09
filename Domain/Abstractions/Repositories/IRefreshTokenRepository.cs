using Domain.Entities;
using Domain.Models;

namespace Domain.Abstractions.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<IList<RefreshToken>> GetAllWithDetailsAsync(PageInfo? pageInfo = null);

    Task<RefreshToken?> GetByTokenAsync(string token);

    Task<RefreshToken?> GetByJwtIdAsync(string jwtId);

    Task<RefreshToken?> GetByUserIdAsync(long userId);

    Task<RefreshToken?> GetByIdWithDetailsAsync(long id);

    Task<RefreshToken?> GetByTokenWithDetailsAsync(string token);

    Task<RefreshToken?> GetByJwtIdWithDetailsAsync(string jwtId);

    Task<RefreshToken?> GetByUserIdWithDetailsAsync(long userId);
}
