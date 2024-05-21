using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.User;
using DokWokApi.Extensions;
using System.IdentityModel.Tokens.Jwt;

namespace DokWokApi.Middlewares;

public class JwtMiddleware
{
    private readonly RequestDelegate next;

    private readonly ILogger logger;

    public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task Invoke(HttpContext context, IUserService userService, 
        ISecurityTokenService<UserModel, JwtSecurityToken> securityTokenService)
    {
        // Get token from Authorization header of the request
        //var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ")[^1];
        
        // Get token from session
        var token = await context.Session.GetStringAsync("userToken");
        if (token is not null)
        {
            await AttachUserToContext(context, userService, securityTokenService, token);
        }

        await next(context);
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
                var roles = await userService.GetUserRolesAsync(user);
                context.Items["User"] = user;
                context.Items["UserRoles"] = roles;
                logger.LogDebug("Jwt validation passed successfully. User '{UserName}' was added to the HTTP context items", user.UserName);
            }
        }
        catch(Exception ex)
        {
            logger.LogDebug(message: "The security token had not passed the validation", exception: ex);
        }
    }
}
