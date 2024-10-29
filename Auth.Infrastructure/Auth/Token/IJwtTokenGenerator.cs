using Auth.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Auth.Infrastructure.Auth.Token;

public interface IJwtTokenGenerator
{
    (string token, RefreshToken refreshToken) GenerateToken(Guid userId, string name, string email, UserRole[]? roles, IEnumerable<Claim>? previousClaims);
    bool IsTokenExpiringSoon(JwtSecurityToken token, int thresholdInMinutes = 5);
}