using Auth.Domain.Consts;
using Auth.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Auth.API.Filters.Base;

public sealed class BaseFilterAzure
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Marcar membros como estáticos", Justification = "<Pendente>")]
    internal (UserRoleEnum[] roles, Guid id) GetUserInfo(dynamic context)
    {
        if (context is ActionExecutedContext actionExecutedContext)
        {
            UserRoleEnum[] roles = NormalizeRoles(GetClaimsRoles(actionExecutedContext.HttpContext));
            Guid id = GetClaimsId(actionExecutedContext.HttpContext);

            return (roles, id);
        }
        else if (context is AuthorizationFilterContext authorizationFilterContext)
        {
            UserRoleEnum[] roles = NormalizeRoles(GetClaimsRoles(authorizationFilterContext.HttpContext));
            Guid id = GetClaimsId(authorizationFilterContext.HttpContext);

            return (roles, id);
        }
        else if (context is ExceptionContext exceptionContext)
        {
            UserRoleEnum[] roles = NormalizeRoles(GetClaimsRoles(exceptionContext.HttpContext));
            Guid id = GetClaimsId(exceptionContext.HttpContext);

            return (roles, id);
        }

        return ([], Guid.Empty);
    }

    private static string[]? GetClaimsRoles(HttpContext httpContext)
    {
        return httpContext.User.FindAll(ClaimTypes.Role)?.Select(c => c.Value).ToArray();
    }

    private static UserRoleEnum[] NormalizeRoles(string[]? roleClaims)
    {
        if (roleClaims?.Length < 1 || roleClaims is null)
        {
            return [];
        }

        List<UserRoleEnum> roleEnums = [];

        foreach (var role in roleClaims)
        {
            if (Enum.TryParse<UserRoleEnum>(role, true, out var userRole))
            {
                roleEnums.Add(userRole);
            }
        }

        return [.. roleEnums];
    }

    private static Guid GetClaimsId(HttpContext httpContext)
    {
        string id = httpContext.User.FindFirstValue(SystemConsts.AzureUserIdClaims) ?? string.Empty;

        if (string.IsNullOrEmpty(id))
        {
            return Guid.Empty;
        }

        Guid guid = Guid.Parse(id);

        return guid;
    }
}