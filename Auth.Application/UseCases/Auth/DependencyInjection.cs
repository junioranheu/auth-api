using Auth.Application.UseCases.Auth.CreateRefreshTokenJWT;
using Auth.Application.UseCases.Auth.CreateTokenJWT;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Application.UseCases.Auth;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateToken, CreateToken>();
        services.AddScoped<ICreateRefreshToken, CreateRefreshToken>();

        return services;
    }
}