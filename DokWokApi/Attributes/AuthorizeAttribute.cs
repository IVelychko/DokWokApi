using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using DokWokApi.BLL.Models.User;

namespace DokWokApi.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public string Role { get; set; }

    public AuthorizeAttribute(string role)
    {
        Role = role;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.Items["User"] as UserModel;
        var roles = context.HttpContext.Items["UserRoles"] as IEnumerable<string>;
        if (user is null || roles is null || !roles.Contains(Role))
        {
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}
