﻿using Auth.API.Middlewares;
using Auth.Domain.Consts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Diagnostics;

namespace Auth.API;

public static class DependencyAppConfiguration
{
    public static WebApplication UseAppConfiguration(this WebApplication app, WebApplicationBuilder builder)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        AddMiddleware(app);
        AddSwagger(app);
        AddHttpsRedirection(app);
        AddCors(app, builder);
        AddCompression(app);
        AddAuth(app);
        AddObservability(app);
        AddCaching(app);

        return app;
    }

    private static void AddMiddleware(WebApplication app)
    {
        app.UseMiddleware<TokenRefreshMiddleware>();
    }

    private static void AddSwagger(WebApplication app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", SystemConsts.Name);
            c.DocExpansion(DocExpansion.None);
        });

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
    }

    private static void AddHttpsRedirection(WebApplication app)
    {
        if (app.Environment.IsProduction())
        {
            app.UseHttpsRedirection();
        }
    }

    private static void AddCors(WebApplication app, WebApplicationBuilder builder)
    {
        app.UseCors(builder.Configuration["CORSSettings:Cors"] ?? string.Empty);
    }

    private static void AddCompression(WebApplication app)
    {
        /// <summary>
        /// O trecho "app.UseWhen" abaixo é necessário quando a API tem uma resposta IAsyncEnumerable/Yield;
        /// O "UseResponseCompression" conflita com esse tipo de requisição, portanto é obrigatória a verificação abaixo;
        /// Caso não existam requisições desse tipo na API, é apenas necessário o trecho "app.UseResponseCompression()";
        /// </summary>
        app.UseWhen(context => !IsStreamingRequest(context), x =>
        {
            x.UseResponseCompression();
        });

        static bool IsStreamingRequest(HttpContext context)
        {
            Endpoint? endpoint = context.GetEndpoint();

            if (endpoint is RouteEndpoint routeEndpoint)
            {
                ControllerActionDescriptor? acao = routeEndpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

                if (acao is not null)
                {
                    Type? tipo = acao.MethodInfo.ReturnType;

                    if (tipo.IsGenericType && tipo.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>))
                    {
                        return true;
                    }

                    return false;
                }
            }

            return false;
        }
    }

    private static void AddAuth(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }

    private static void AddObservability(WebApplication app)
    {
        ActivitySource activitySource = new(SystemConsts.Name);

        ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();

        ActivityListener listener = new()
        {
            ShouldListenTo = source => source.Name == "Microsoft.AspNetCore" || source.Name == SystemConsts.Name,
            Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity =>
            {
                // Filtra métodos irrelevantes;
                string? method = activity.Tags.FirstOrDefault(t => t.Key == "http.request.method").Value?.ToString();
                string? path = activity.Tags.FirstOrDefault(t => t.Key == "http.route").Value?.ToString();

                if (method == "OPTIONS" || string.IsNullOrEmpty(path) || path.Contains("favicon") || path.Contains("health"))
                {
                    return;
                }

                string? statusCodeStr = activity.GetTagItem("http.response.status_code")?.ToString();

                logger.LogInformation("[Observability] {Tags}, [duration, {Duration}ms], [status, {Status}]",
                    string.Join(", ", activity.Tags.Where(x => x.Key.StartsWith("http") || x.Key == "server.address")),
                    activity.Duration.TotalMilliseconds,
                    statusCodeStr
                );
            }
        };

        ActivitySource.AddActivityListener(listener);
    }

    private static void AddCaching(WebApplication app)
    {
        app.UseResponseCaching();
    }
}