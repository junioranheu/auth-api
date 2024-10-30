using Auth.Application.UseCases.Auth.CreateRefreshTokenJWT;
using Auth.Infrastructure.Auth.Token;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Auth.API.Middlewares;

public sealed class TokenRefreshMiddleware(RequestDelegate next, IJwtTokenGenerator jwtTokenGenerator, IServiceScopeFactory scopeFactory)
{
    private readonly RequestDelegate _next = next;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            await _next(context);
            return;
        }

        string token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(token))
        {
            JwtSecurityToken jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            (bool isTokenExpiringSoon, double _) = _jwtTokenGenerator.IsTokenExpiringSoonOrHasAlreadyExpired(jwtToken);

            if (isTokenExpiringSoon)
            {
                string userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value ?? string.Empty;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    throw new InvalidOperationException("Falha ao gerar novo Token. O parâmetro userIdClaim é inválido");
                }

                Guid userId = Guid.Parse(userIdClaim);

                using IServiceScope scope = _scopeFactory.CreateScope();
                ICreateRefreshToken createRefreshToken = scope.ServiceProvider.GetRequiredService<ICreateRefreshToken>();

                (string newJwtToken, string newRefreshToken) = await createRefreshToken.RefreshToken(userId);

                // Update context;
                context.Response.Headers.Authorization = "Bearer " + newJwtToken;
                context.Items["JwtToken"] = newJwtToken;
                context.Items["RefreshToken"] = newRefreshToken;
            }
        }

        await _next(context);
    }
}