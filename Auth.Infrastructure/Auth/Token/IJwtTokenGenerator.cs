using System.Security.Claims;

namespace Auth.Infrastructure.Auth.Token
{
    public interface IJwtTokenGenerator
    {
        string GenerateRefreshToken();
        string GerarToken(string nomeCompleto, string email, IEnumerable<Claim>? listaClaims);
        ClaimsPrincipal? GetInfoTokenExpired(string? token);
    }
}