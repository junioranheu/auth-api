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
    public class JwtTokenGenerator(IOptions<JwtSettings> jwtOptions) : IJwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings = jwtOptions.Value;

        public string GerarToken(string name, string email, IEnumerable<Claim>? previousClaims)
        {
            JwtSecurityTokenHandler tokenHandler = new();

            SigningCredentials signingCredentials = new(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret ?? string.Empty)),
                algorithm: SecurityAlgorithms.HmacSha256Signature
            );

            ClaimsIdentity claims;

            if (previousClaims?.Count() > 0)
            {
                claims = new ClaimsIdentity(previousClaims);
            }
            else
            {
                claims = new(
                [
                    new Claim(type: ClaimTypes.Name, name ?? string.Empty),
                    new Claim(type: ClaimTypes.Email, email ?? string.Empty),

                    // Imitando o cenário do Azure, onde só tem e-mail e nome, e não role e ID;
                    // new Claim(type: ClaimTypes.Role, usuario.UsuarioTipoId.ToString()),
                    // new Claim(type: ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString())
                ]);
            }

            DateTime date = GetDate();

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Issuer = _jwtSettings.Issuer, // Emissor do token;
                IssuedAt = date,
                Audience = _jwtSettings.Audience,
                NotBefore = date,
                Expires = date.AddMinutes(_jwtSettings.TokenExpiryMinutes),
                Subject = claims,
                SigningCredentials = signingCredentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return jwt;
        }

        public string GenerateRefreshToken()
        {
            var random = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(random);
            var refreshToken = Convert.ToBase64String(random);

            return refreshToken;
        }

        public ClaimsPrincipal? GetInfoTokenExpired(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret ?? string.Empty)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            ClaimsPrincipal? principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Token inválido");
            }

            return principal;
        }

        private static DateTime GetDate()
        {
            // Produção: +3 horas;
            DateTime horarioBrasiliaAjustado = GerarHorarioBrasilia().AddHours(+3);

#if DEBUG
            // Dev: horário normal;
            horarioBrasiliaAjustado = GerarHorarioBrasilia();
#endif

            return horarioBrasiliaAjustado;
        }
    }
}