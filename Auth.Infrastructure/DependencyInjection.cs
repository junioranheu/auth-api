using Auth.Infrastructure.Auth.Models;
using Auth.Infrastructure.Auth.Token;
using Auth.Infrastructure.Data;
using Auth.Infrastructure.Factory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using static junioranheu_utils_package.Fixtures.Get;

namespace Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjectionInfrastructure(this IServiceCollection services, WebApplicationBuilder builder)
    {
        AddServices(services, builder);
        AddAuth(services, builder);
        AddFactory(services);
        AddContext(services, builder);

        return services;
    }

    private static void AddServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
    }

    private static readonly string[] onChallengeError = ["Acesso negado. Você não tem permissão para acessar este recurso ou não está autenticado."];

    private static void AddAuth(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
             .AddJwtBearer(x =>
             {
                 x.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                 x.SaveToken = true;
                 x.IncludeErrorDetails = true;
                 x.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Secret"] ?? string.Empty)),
                     ValidateIssuer = true,
                     ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                     ValidateAudience = true,
                     ValidAudience = builder.Configuration["JwtSettings:Audience"],
                     ValidateLifetime = true,
                     ClockSkew = TimeSpan.Zero
                 };

                 x.Events = new JwtBearerEvents
                 {
                     OnChallenge = context =>
                     {
                         context.HandleResponse();

                         context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                         context.Response.ContentType = "application/json";

                         var result = JsonSerializer.Serialize(new
                         {
                             Code = StatusCodes.Status401Unauthorized,
                             Date = ObterDetalhesDataHora(),
                             context.HttpContext.Request.Path,
                             Messages = onChallengeError,
                             IsError = true
                         });

                         return context.Response.WriteAsync(result);
                     }
                 };
             });
    }

    private static void AddFactory(IServiceCollection services)
    {
        services.AddSingleton<IConnectionFactory, ConnectionFactory>();
    }

    private static void AddContext(IServiceCollection services, WebApplicationBuilder builder)
    {
        string con = new ConnectionFactory(builder.Configuration).ObterStringConnection();

        services.AddDbContextPool<Context>(x =>
        {
            x.UseMySql(con, ServerVersion.AutoDetect(con));
        });
    }
}