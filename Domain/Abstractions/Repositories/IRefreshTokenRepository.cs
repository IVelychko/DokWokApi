using Domain.Entities;

namespace Domain.Abstractions.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token);

    Task<RefreshToken?> GetByTokenAsNoTrackingAsync(string token);

    Task<bool> RefreshTokenExistsAsync(string token);
}
