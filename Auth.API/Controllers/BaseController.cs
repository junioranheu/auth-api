using Auth.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using static junioranheu_utils_package.Fixtures.Get;

namespace Auth.API.Controllers;

public abstract class BaseController<T> : Controller
{
    protected Guid? GetUserId()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Guid.Empty;
        }

        string id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        if (string.IsNullOrEmpty(id))
        {
            return null;
        }

        Guid guid = Guid.Parse(id);

        return guid;
    }

    protected string GetUserName()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return string.Empty;
        }

        string name = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        return name;
    }

    protected string GetUserEmail()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return string.Empty;
        }

        string email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        return email;
    }

    protected (UserRoleEnum[] userRolesEnum, string[] userRolesStr) GetUserRoles()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return ([], []);
        }

        string[]? roleClaims = User.FindAll(ClaimTypes.Role)?.Select(c => c.Value).ToArray();

        if (roleClaims?.Length < 1 || roleClaims is null)
        {
            return ([], []);
        }

        List<UserRoleEnum> roleEnums = [];
        List<string> roleEnumsStr = [];

        foreach (var role in roleClaims)
        {
            if (Enum.TryParse<UserRoleEnum>(role, true, out var userRole))
            {
                roleEnums.Add(userRole);
                roleEnumsStr.Add(ObterDescricaoEnum(userRole));
            }
        }

        return ([.. roleEnums], [.. roleEnumsStr]);
    }
}