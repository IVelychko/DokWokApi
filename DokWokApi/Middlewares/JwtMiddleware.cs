using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.User;
using DokWokApi.Extensions;
using System.IdentityModel.Tokens.Jwt;

namespace DokWokApi.Middlewares;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger _logger;

    public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context, IUserService userService, 
        ISecurityTokenService<UserModel, JwtSecurityToken> securityTokenService)
    {   
        var token = await context.Session.GetStringAsync("userToken");
        if (token is not null)
        {
            await AttachUserToContext(context, userService, securityTokenService, token);
        }

        await _next(context);
    }

    private async Task AttachUserToContext(HttpContext context, IUserService userService, 
        ISecurityTokenService<UserModel, JwtSecurityToken> securityTokenService, string token)
    {
        try
        {
            var jwtSecurityToken = securityTokenService.ValidateToken(token);
            var userId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
            var user = userId is not null ? await userService.GetByIdAsync(userId) : null;
            if (user is not null)
            {
                var roles = await userService.GetUserRolesAsync(user.Id!);
                context.Items["User"] = user;
                context.Items["UserRoles"] = roles;
                _logger.LogDebug("Jwt validation passed successfully. User '{UserName}' was added to the HTTP context items", user.UserName);
            }
        }
        catch(Exception ex)
        {
            _logger.LogDebug(message: "The security token had not passed the validation", exception: ex);
        }
    }
}
