using Auth.API;
using Auth.Application;
using Auth.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddDependencyInjectionAPI();
    builder.Services.AddDependencyInjectionApplication(builder);
    builder.Services.AddDependencyInjectionInfrastructure(builder);
}

WebApplication app = builder.Build();
{
    app.UseAppConfigurationAsync(builder);
    app.Run();
}