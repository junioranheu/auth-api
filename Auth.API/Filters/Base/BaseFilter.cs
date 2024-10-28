using Auth.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Auth.API.Filters.Base;

public sealed class BaseFilter
{
#pragma warning disable CA1822
    internal (Guid? userId, string email, UserRoleEnum[] roles) GetUserInfo(dynamic context)
#pragma warning restore CA1822 
    {
        if (context is ActionExecutedContext actionExecutedContext)
        {
            return BaseGetUserInfo(actionExecutedContext);
        }
        else if (context is AuthorizationFilterContext authorizationFilterContext)
        {
            return BaseGetUserInfo(authorizationFilterContext);
        }
        else if (context is ExceptionContext exceptionContext)
        {
            return BaseGetUserInfo(exceptionContext);
        }

        return (null, string.Empty, []);

        static (Guid? userId, string email, UserRoleEnum[] roles) BaseGetUserInfo(dynamic context)
        {
            if (context.HttpContext.User.Identity!.IsAuthenticated)
            {
                var user = context.HttpContext.User; 

                string userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;
                string email = user.FindFirst(ClaimTypes.Email).Value;

                string[] rolesStr = user.Find(ClaimTypes.Role).Value;
                List<UserRoleEnum> rolesList = [];

                foreach (var item in rolesStr)
                {
                    UserRoleEnum role = (UserRoleEnum)Enum.Parse(typeof(UserRoleEnum), item);
                    rolesList.Add(role);
                }

                UserRoleEnum[] roles = [.. rolesList];

                return (Guid.Parse(userId), email, roles);
            }

            return (null, string.Empty, []);
        }
    }
}