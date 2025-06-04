using Domain.DTOs.Requests.Users;
using Domain.DTOs.Responses.Users;

namespace Domain.Abstractions.Services;

public interface IAuthService
{
    Task<AuthorizedUserResponse> LoginUserAsync(LoginUserRequest request, bool isAdmin);

    Task<AuthorizedUserResponse> RegisterAsync(RegisterUserRequest request);

    Task LogOutAsync(string refreshToken);
    
    Task<AuthorizedUserResponse> RefreshTokenAsync(RefreshUserTokenRequest request);
}