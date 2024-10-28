using Auth.Domain.Entities;
using System.Security.Claims;

namespace Auth.Infrastructure.Auth.Token
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid userId, string name, string email, UserRole[]? roles, IEnumerable<Claim>? previousClaims);
        ClaimsPrincipal? GetInfoTokenExpired(string? token);
    }
}