using Auth.Application.UseCases.Auth.CreateRefreshTokenJWT;
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
        string token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(token))
        {
            JwtSecurityToken jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            if (_jwtTokenGenerator.IsTokenExpiringSoon(jwtToken))
            {
                Guid userId = Guid.NewGuid(); // TO DO: OBTER O USUÁRIO!!!!!!!!!!!
                (string newJwtToken, string newRefreshToken) = await _createRefreshToken.RefreshToken(userId);

                // Update context;
                context.Response.Headers.Authorization = "Bearer " + newJwtToken;
                context.Items["JwtToken"] = newJwtToken;
                context.Items["RefreshToken"] = newRefreshToken;
            }
        }

        await _next(context);
    }
}