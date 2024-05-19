using DokWokApi.BLL.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DokWokApi.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate next;

        private readonly IConfiguration config;

        private readonly ILogger logger;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<JwtMiddleware> logger)
        {
            this.next = next;
            config = configuration;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context, IUserService userService)
        {
            var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ")[^1];
            if (token is not null)
            {
                await AttachUserToContext(context, userService, token);
            }

            await next(context);
        }

        private async Task AttachUserToContext(HttpContext context, IUserService userService, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = config["Jwt:Audience"],
                    ValidIssuer = config["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
                if (userId is not null)
                {
                    // attach user to context on successful jwt validation
                    var user = await userService.GetByIdAsync(userId);
                    if (user is not null)
                    {
                        context.Items["User"] = user;
                        var roles = await userService.GetUserRolesAsync(user);
                        context.Items["UserRoles"] = roles;
                        logger.LogDebug("Jwt validation passed successfully. User '{UserName}' was added to the HTTP context items", user.UserName);
                    }
                }
            }
            catch(Exception ex)
            {
                logger.LogDebug(message: "The security token had not passed the validation", exception: ex);
            }
        }
    }
}
