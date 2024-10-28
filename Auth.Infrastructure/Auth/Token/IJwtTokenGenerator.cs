using Auth.Domain.Enums;
using System.Security.Claims;

namespace Auth.Infrastructure.Auth.Token
{
    public interface IJwtTokenGenerator
    {
        string GerarToken(Guid userId, string name, string email, UserRoleEnum[] roles, IEnumerable<Claim>? previousClaims);
        ClaimsPrincipal? GetInfoTokenExpired(string? token);
    }
}