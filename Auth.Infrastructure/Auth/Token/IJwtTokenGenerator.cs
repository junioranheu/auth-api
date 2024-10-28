using Auth.Domain.Entities;
using System.Security.Claims;

namespace Auth.Infrastructure.Auth.Token
{
    public interface IJwtTokenGenerator
    {
        Task<string> GenerateRefreshToken(Guid userId);
        Task<(string token, string refreshToken)> GenerateToken(Guid userId, string name, string email, UserRole[]? roles, IEnumerable<Claim>? previousClaims);
    }
}