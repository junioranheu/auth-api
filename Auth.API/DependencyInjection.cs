using Auth.API.Filters;
using Auth.Domain.Consts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using System.IO.Compression;
using System.Text.Json.Serialization;

namespace Auth.API;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjectionAPI(this IServiceCollection services, WebApplicationBuilder builder)
    {
        AddSwagger(services);
        AddCors(services, builder);
        AddKestrel(services);
        AddCompression(services);
        AddControllers(services);
        AddMisc(services);

        return services;
    }

    private static void AddSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = SystemConsts.Name, Version = "v1" });

            OpenApiSecurityScheme jwtSecurityScheme = new()
            {
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "JWT Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Description = "Coloque **_apenas_** o token (JWT Bearer) abaixo!",

                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtSecurityScheme, Array.Empty<string>() }
            });
        });
    }

    private static void AddCors(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddCors(x =>
            x.AddPolicy(name: builder.Configuration["CORSSettings:Cors"] ?? string.Empty, builder =>
            {
                // TO DO: SetIsOriginAllowed((host) => true) + AllowCredentials() é inseguro;
                builder.AllowAnyHeader().
                        AllowAnyMethod().
                        SetIsOriginAllowed((host) => true).
                        AllowCredentials().

                        // Expõe o custom header para o front interceptar e atualizar o token;
                        WithExposedHeaders(SystemConsts.RefreshTokenJWTCustomHeader);
            })
        );
    }

    private static void AddKestrel(IServiceCollection services)
    {
        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(30);
        });
    }

    private static void AddCompression(IServiceCollection services)
    {
        services.AddResponseCompression(x =>
        {
            x.EnableForHttps = true;
            x.Providers.Add<BrotliCompressionProvider>();
            x.Providers.Add<GzipCompressionProvider>();
        });

        services.Configure<BrotliCompressionProviderOptions>(x =>
        {
            x.Level = CompressionLevel.Optimal;
        });

        services.Configure<GzipCompressionProviderOptions>(x =>
        {
            x.Level = CompressionLevel.Optimal;
        });
    }

    private static void AddControllers(IServiceCollection services)
    {
        services.AddControllers(x =>
        {
            x.Filters.Add<ErrorFilter>();
        }).
            AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

#if DEBUG
                x.JsonSerializerOptions.WriteIndented = true;
#else
                x.JsonSerializerOptions.WriteIndented = false;
#endif
            });
    }

    private static void AddMisc(IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddResponseCaching();
    }
}
