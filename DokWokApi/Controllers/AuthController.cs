using DokWokApi.Constants;
using Domain.Abstractions.Services;
using Domain.DTOs.Requests.Users;
using Domain.DTOs.Responses.Users;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route(ApiRoutes.Auth.Group)]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost(ApiRoutes.Auth.Login)]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest request, [FromQuery] bool admin = false)
    {
        var user = await _authService.LoginUserAsync(request, admin);
        SetRefreshTokenCookie(user);
        return Ok(user);
    }
    
    [HttpPost(ApiRoutes.Auth.RegisterUser)]
    public async Task<IActionResult> RegisterUser(RegisterUserRequest request)
    {
        var user = await _authService.RegisterAsync(request);
        SetRefreshTokenCookie(user);
        return CreatedAtRoute("GetUserById", new { id = user.Id }, user);
    }
    
    [HttpPost(ApiRoutes.Auth.RefreshToken)]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
    {
        var refreshToken = GetRefreshTokenFromCookie();
        RefreshUserTokenRequest refreshUserTokenRequest = new(request.Token, refreshToken);
        var user = await _authService.RefreshTokenAsync(refreshUserTokenRequest);
        SetRefreshTokenCookie(user);
        return Ok(user);
    }
    
    [HttpPost(ApiRoutes.Auth.LogOut)]
    public async Task<IActionResult> LogOutUser()
    {
        var refreshToken = GetRefreshTokenFromCookie();
        await _authService.LogOutAsync(refreshToken);
        return Ok();
    }

    private void SetRefreshTokenCookie(AuthorizedUserResponse user)
    {
        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Expires = new DateTimeOffset(user.RefreshToken!.ExpiryDate),
            IsEssential = true,
            Path = "/api/auth",
            SameSite = SameSiteMode.None,
            Secure = true,
        };
        HttpContext.Response.Cookies.Append(CookieNames.RefreshToken, user.RefreshToken!.Token, cookieOptions);
    }

    private string GetRefreshTokenFromCookie()
    {
        var refreshToken = HttpContext.Request.Cookies[CookieNames.RefreshToken];
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ValidationException(CookieNames.RefreshToken, "There was no refresh token provided");
        }
        
        return refreshToken;
    }
}