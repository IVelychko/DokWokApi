using Domain.Abstractions.Services;
using Domain.DTOs.Requests.Users;
using Domain.DTOs.Responses.Users;
using Domain.Shared;

namespace Infrastructure.Cache;

public class CacheAuthService : IAuthService
{
    private readonly IAuthService _authService;
    private readonly ICacheService _cacheService;

    public CacheAuthService(IAuthService authService, ICacheService cacheService)
    {
        _authService = authService;
        _cacheService = cacheService;
    }
    
    public async Task<AuthorizedUserResponse> RegisterAsync(RegisterUserRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var response = await _authService.RegisterAsync(request);
        await _cacheService.RemoveByPrefixAsync("allUsers");
        await _cacheService.RemoveAsync("allCustomers");
        return response;
    }

    public async Task<AuthorizedUserResponse> RefreshTokenAsync(RefreshUserTokenRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var response = await _authService.RefreshTokenAsync(request);
        return response;
    }

    public async Task LogOutAsync(string refreshToken)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(refreshToken, nameof(refreshToken));
        await _authService.LogOutAsync(refreshToken);
    }
    
    public async Task<AuthorizedUserResponse> LoginUserAsync(LoginUserRequest request, bool isAdmin)
    {
        Ensure.ArgumentNotNull(request);
        var response = await _authService.LoginUserAsync(request, isAdmin);
        return response;
    }
}