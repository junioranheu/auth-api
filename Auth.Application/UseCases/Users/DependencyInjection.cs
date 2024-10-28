﻿using Auth.Application.UseCases.Users.Auth;
using Auth.Application.UseCases.Users.GetByUserNameOrEmail;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Application.UseCases.Users;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthUser, AuthUser>();
        services.AddScoped<IGetUserByUserNameOrEmail, GetUserByUserNameOrEmail>();

        return services;
    }
}