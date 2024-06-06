using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using DokWokApi.BLL.Models.User;

namespace DokWokApi.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute(params string[] roles) : Attribute, IAuthorizationFilter
{
    public string[] Roles { get; set; } = roles;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        UserModel? user = context.HttpContext.Items["User"] as UserModel;
        IEnumerable<string>? userRoles = context.HttpContext.Items["UserRoles"] as IEnumerable<string>;
        if (user is null || userRoles is null || !userRoles.Any(ur => Roles.Contains(ur)))
        {
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}
