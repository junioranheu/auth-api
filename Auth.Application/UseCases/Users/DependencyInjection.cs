using Microsoft.Extensions.DependencyInjection;

namespace Auth.Application.UseCases.Users;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthUser, AuthUser>();

        return services;
    }
}