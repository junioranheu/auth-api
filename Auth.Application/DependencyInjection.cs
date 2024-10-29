using Auth.Application.AutoMapper;
using Auth.Application.UseCases.Auth;
using Auth.Application.UseCases.Logs;
using Auth.Application.UseCases.Users;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Auth.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjectionApplication(this IServiceCollection services, WebApplicationBuilder builder)
    {
        AddAutoMapper(services);
        AddLogger(builder);
        AddSignalR(services);

        AddUseCases(services);
        AddServices(services);

        return services;
    }

    private static void AddAutoMapper(IServiceCollection services)
    {
        var mapperConfig = new MapperConfiguration(x =>
        {
            x.AddProfile(new AutoMapperConfig());
        });

        IMapper mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);
    }

    private static void AddLogger(WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
    }

    private static void AddSignalR(IServiceCollection services)
    {
        services.AddSignalR();
    }

    private static void AddUseCases(IServiceCollection services)
    {
        services.AddLogsApplication();
        services.AddUsersApplication();
        services.AddAuthApplication();
    }

    private static void AddServices(IServiceCollection services)
    {
 
    }
}