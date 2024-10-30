﻿using Auth.Domain.Entities;
using Auth.Domain.Enums;
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
        private const int thresholdTimeZoneInHours = 3;

        public (string token, RefreshToken refreshToken) GenerateToken(Guid userId, string name, string email, UserRole[]? roles)
        {
            JwtSecurityTokenHandler tokenHandler = new();

            SigningCredentials signingCredentials = new(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret ?? string.Empty)),
                algorithm: SecurityAlgorithms.HmacSha256Signature
            );

            List<Claim> claimList = [
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Name, name),
                new(ClaimTypes.Email, email)
            ];

            if (roles is not null && roles.Length > 0)
            {
                IEnumerable<Claim> rolesClaim = roles.Select(x => new Claim(ClaimTypes.Role, x.Role.ToString()));
                claimList.AddRange(rolesClaim);

                bool alreadyHasComum = roles.Any(x => x.Role == UserRoleEnum.Comum);

                if (!alreadyHasComum)
                {
                    Claim roleComum = new(ClaimTypes.Role, UserRoleEnum.Comum.ToString());
                    claimList.Add(roleComum);
                }
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

        #region extras
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

        private static string GenerateRefreshTokenStr()
        {
            var random = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(random);
            var refreshToken = Convert.ToBase64String(random);

            return refreshToken;
        }

        public (bool isTokenExpiringSoonOrHasAlreadyExpired, double differenceInMinutes) IsTokenExpiringSoonOrHasAlreadyExpired(JwtSecurityToken token, int thresholdInMinutes = 0)
        {
            DateTime date = GetDate().AddHours(thresholdTimeZoneInHours); // A data de validade do Token é ToUniversalTime, portanto deliberadamente deve ser adicionado tempo aqui, sempre;
            DateTime dateThreshold = date.AddMinutes(thresholdInMinutes);

            double differenceInMinutes = (token.ValidTo - dateThreshold).TotalMinutes;
            bool isTokenExpiringSoonOrHasAlreadyExpired = differenceInMinutes <= 0;

            return (isTokenExpiringSoonOrHasAlreadyExpired, differenceInMinutes);
        }

        private static DateTime GetDate()
        {
            // Prod: +3h;
            DateTime date = GerarHorarioBrasilia().AddHours(thresholdTimeZoneInHours);

#if DEBUG
            // Dev;
            date = GerarHorarioBrasilia();
#endif

            return date;
        }
        #endregion
    }
}