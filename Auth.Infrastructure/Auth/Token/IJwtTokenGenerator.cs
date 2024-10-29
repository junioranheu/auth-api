using Auth.Domain.Entities;
using System.Security.Claims;

namespace Auth.Infrastructure.Auth.Token
{
    public interface IJwtTokenGenerator
    {
        RefreshToken GenerateRefreshToken(Guid userId);
        (string token, RefreshToken refreshToken) GenerateToken(Guid userId, string name, string email, UserRole[]? roles, IEnumerable<Claim>? previousClaims);
    }
}