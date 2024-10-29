using Auth.Application.UseCases.Auth.CreateRefreshTokenJWT;
using Auth.Domain.Entities;
using Auth.Infrastructure.Auth.Token;
using System.IdentityModel.Tokens.Jwt;

namespace Auth.API.Middlewares;

public sealed class TokenRefreshMiddleware(RequestDelegate next, IJwtTokenGenerator jwtTokenGenerator, ICreateRefreshToken createRefreshToken)
{
    private readonly RequestDelegate _next = next;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly ICreateRefreshToken _createRefreshToken = createRefreshToken;

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(token))
        {
            JwtSecurityToken jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            if (_jwtTokenGenerator.IsTokenExpiringSoon(jwtToken))
            {
                Guid userId = Guid.NewGuid();

                RefreshToken newToken = _jwtTokenGenerator.GenerateRefreshToken(userId); // CHAMAR A NOVA PARADA DO CreateRefreshToken;
                context.Response.Headers.Authorization = "Bearer " + newToken.Token;

                // Update context;
                context.Items["JwtToken"] = newToken.Token;
                context.Items["RefreshToken"] = newToken.Token;
            }
        }

        await _next(context);
    }
}