using Auth.Application.UseCases.Logs.Create;
using Auth.Application.UseCases.Logs.GetAll;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Application.UseCases.Logs;

public static class DependencyInjection
{
    public static IServiceCollection AddLogsApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateLog, CreateLog>();
        services.AddScoped<IGetAllLog, GetAllLog>();

        return services;
    }
}