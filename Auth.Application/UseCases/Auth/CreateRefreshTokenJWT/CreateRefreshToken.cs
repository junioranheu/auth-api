﻿using Auth.Domain.Entities;
using Auth.Infrastructure.Auth.Token;
using Auth.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static junioranheu_utils_package.Fixtures.Get;

namespace Auth.Application.UseCases.Auth.CreateRefreshTokenJWT;

public sealed class CreateRefreshToken(Context context, IJwtTokenGenerator jwtTokenGenerator) : ICreateRefreshToken
{
    private readonly Context _context = context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;

    public async Task<string> RefreshToken(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new Exception($"Parâmetro {nameof(userId)} está vazio em {nameof(RefreshToken)}");
        }

        (User user, UserRole[] userRoles) = await GetUser(userId);

        // Gere novo JWT e refresh token;
        (string newJwtToken, RefreshToken _) = _jwtTokenGenerator.GenerateToken(userId: user.UserId, name: user.FullName, email: user.Email, roles: userRoles);

        // Revogue os antigos refresh tokens inválidos;
        await Update(userId, mustCheckForValidRefreshTokens: true);

        return newJwtToken;
    }

    public async Task Save(RefreshToken newRefreshToken)
    {
        await _context.RefreshTokens.AddAsync(newRefreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Guid userId, bool mustCheckForValidRefreshTokens)
    {
        List<RefreshToken> oldRefreshTokens = await GetOldRefreshTokensAndCheckIfRefreshTokenIsValid(userId, mustCheckForValidRefreshTokens);

        if (oldRefreshTokens.Count == 0)
        {
            return;
        }

        List<Guid> oldRefreshTokenIds = [.. oldRefreshTokens.Select(y => y.RefreshTokenId)];

        await _context.RefreshTokens.
        Where(x => oldRefreshTokenIds.Contains(x.RefreshTokenId)).
        ExecuteUpdateAsync(x => x.
            // SetProperty(prop => prop.Status, false).
            SetProperty(prop => prop.Revoked, GerarHorarioBrasilia())
        );
    }

    #region extras
    private async Task<List<RefreshToken>> GetOldRefreshTokensAndCheckIfRefreshTokenIsValid(Guid userId, bool mustCheckForValidRefreshTokens)
    {
        List<RefreshToken> oldRefreshTokens = await _context.RefreshTokens.
                                              AsNoTracking().
                                              Where(x =>
                                                 x.UserId == userId &&
                                                 x.Revoked == null
                                              ).
                                              OrderByDescending(x => x.Created).
                                              ToListAsync();

        // Se mustCheckForValidRefreshTokens for false, significa que deve ser retornado todos os registros, sem validações posteriores;
        if (!mustCheckForValidRefreshTokens)
        {
            return oldRefreshTokens;
        }

        DateTime date = GerarHorarioBrasilia();
        List<RefreshToken> validRefreshTokens = [.. oldRefreshTokens.Where(x => x.Expires > date)];
        List<RefreshToken> invalidRefreshTokens = [.. oldRefreshTokens.Where(x => x.Expires <= date)];

        if (validRefreshTokens is null || validRefreshTokens.Count == 0)
        {
            throw new SecurityTokenException("Refresh token inválido, autentique-se novamente");
        }

        return invalidRefreshTokens;
    }

    private async Task<(User user, UserRole[] userRoles)> GetUser(Guid userId)
    {
        User? user = await _context.Users.
                     Include(x => x.UserRoles).
                     AsNoTracking().
                     Where(x => x.UserId == userId).
                     FirstOrDefaultAsync() ?? throw new Exception($"Usuário {userId} não encontrado");

        if (!user.Status)
        {
            throw new Exception($"O usuário {user.UserName} ({userId}) está desativado");
        }

        UserRole[] userRoles = user.UserRoles?.ToArray() ?? [];

        return (user, userRoles);
    }
    #endregion
}