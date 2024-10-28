using Auth.Domain.Enums;
using System.Security.Claims;

namespace Auth.Infrastructure.Auth.Token
{
    public interface IJwtTokenGenerator
    {
        string GenerateRefreshToken();
        string GerarToken(Guid id, string name, string email, UserRoleEnum role, IEnumerable<Claim>? listaClaims);
        ClaimsPrincipal? GetInfoTokenExpired(string? token);
    }
}