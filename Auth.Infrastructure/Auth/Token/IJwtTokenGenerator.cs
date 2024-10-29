using Auth.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace Auth.Infrastructure.Auth.Token;

public interface IJwtTokenGenerator
{
    (string token, RefreshToken refreshToken) GenerateToken(Guid userId, string name, string email, UserRole[]? roles);
    bool IsTokenExpiringSoonOrHasAlreadyExpired(JwtSecurityToken token, int thresholdInMinutes = 5);
}