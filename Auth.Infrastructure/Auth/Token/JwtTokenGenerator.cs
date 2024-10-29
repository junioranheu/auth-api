using Auth.Domain.Entities;
using Auth.Infrastructure.Auth.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static junioranheu_utils_package.Fixtures.Get;

namespace Auth.Infrastructure.Auth.Token
{
    public sealed class JwtTokenGenerator(IOptions<JwtSettings> jwtOptions) : IJwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings = jwtOptions.Value;

        public (string token, RefreshToken refreshToken) GenerateToken(Guid userId, string name, string email, UserRole[]? roles, IEnumerable<Claim>? previousClaims)
        {
            JwtSecurityTokenHandler tokenHandler = new();

            SigningCredentials signingCredentials = new(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret ?? string.Empty)),
                algorithm: SecurityAlgorithms.HmacSha256Signature
            );

            List<Claim> claimList = previousClaims?.ToList() ??
            [
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Name, name),
                new(ClaimTypes.Email, email)
            ];

            if (roles is not null && roles.Length > 0)
            {
                IEnumerable<Claim> x = roles.Select(x => new Claim(ClaimTypes.Role, x.Role.ToString()));
                claimList.AddRange(x);
            }

            ClaimsIdentity claims = new(claimList);

            DateTime date = GetDate();

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Issuer = _jwtSettings.Issuer,
                IssuedAt = date,
                Audience = _jwtSettings.Audience,
                NotBefore = date,
                Expires = date.AddMinutes(_jwtSettings.TokenExpiryMinutes),
                Subject = claims,
                SigningCredentials = signingCredentials
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string jwt = tokenHandler.WriteToken(token);
            RefreshToken refreshToken = GenerateRefreshToken(userId);

            return (jwt, refreshToken);
        }

        private RefreshToken GenerateRefreshToken(Guid userId)
        {
            string token = GenerateRefreshTokenStr();

            RefreshToken refreshToken = new()
            {
                Token = token,
                UserId = userId,
                Created = GetDate(),
                Expires = GetDate().AddMinutes(_jwtSettings.RefreshTokenExpiryMinutes),
                Status = true
            };

            return refreshToken;
        }

        #region extras
        private static string GenerateRefreshTokenStr()
        {
            var random = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(random);
            var refreshToken = Convert.ToBase64String(random);

            return refreshToken;
        }

        public bool IsTokenExpiringSoon(JwtSecurityToken token, int thresholdInMinutes = 5)
        {
            DateTime date = GetDate();
            DateTime dateThreshold = date.AddMinutes(thresholdInMinutes);
            bool isTokenExpiringSoon = token.ValidTo < dateThreshold;

            return isTokenExpiringSoon;
        }

        private static DateTime GetDate()
        {
            // Prod: +3h;
            DateTime date = GerarHorarioBrasilia().AddHours(3);

#if DEBUG
            // Dev;
            date = GerarHorarioBrasilia();
#endif

            return date;
        }
        #endregion
    }
}