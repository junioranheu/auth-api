using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Auth.API.Filters.Base;

public sealed class BaseFilter
{
    public BaseFilter() { }

    #region usuario_id
    internal async Task<Guid?> BaseGetUserId(ActionExecutedContext context)
    {
        return await BaseGetUserId(context.HttpContext.RequestServices.GetService<IObterUsuarioCacheService>(), BaseGetUserEmail(context));
    }

    internal async Task<Guid?> BaseGetUserId(AuthorizationFilterContext context)
    {
        return await BaseGetUserId(context.HttpContext.RequestServices.GetService<IObterUsuarioCacheService>(), BaseGetUserEmail(context));
    }

    internal async Task<Guid?> BaseGetUserId(ExceptionContext context)
    {
        return await BaseGetUserId(context.HttpContext.RequestServices.GetService<IObterUsuarioCacheService>(), BaseGetUserEmail(context));
    }

    private static async Task<Guid?> BaseGetUserId(IObterUsuarioCacheService? service, string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return 0;
        }

        UsuarioOutput? usuario = await service!.Execute(email);
        int usuarioId = usuario is not null ? usuario.UsuarioId : 0;

        return usuarioId;
    }
    #endregion

    #region usuario_email
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Marcar membros como estáticos", Justification = "<Pendente>")]
    internal string BaseGetUserEmail(dynamic context)
    {
        if (context is ActionExecutedContext actionExecutedContext)
        {
            return BaseGetUserEmail(actionExecutedContext);
        }
        else if (context is AuthorizationFilterContext authorizationFilterContext)
        {
            return BaseGetUserEmail(authorizationFilterContext);
        }
        else if (context is ExceptionContext exceptionContext)
        {
            return BaseGetUserEmail(exceptionContext);
        }

        return string.Empty;

        static string BaseGetUserEmail(dynamic context)
        {
            if (context.HttpContext.User.Identity!.IsAuthenticated)
            {
                string email = context.HttpContext.User.FindFirst(ClaimTypes.Email)!.Value;
                return email ?? string.Empty;
            }

            return string.Empty;
        }
    }
    #endregion
}