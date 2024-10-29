using Auth.Domain.Entities;
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

    public async Task<(string token, string refreshToken)> Execute(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new Exception("Usuário não encontrado");
        }

        var oldRefreshTokens = await _context.RefreshTokens.
                               AsNoTracking().
                               Where(x => x.UserId == userId && x.Status == true).
                               OrderByDescending(x => x.Created).
                               ToListAsync();

        if (oldRefreshTokens is null || oldRefreshTokens.Count == 0)
        {
            throw new SecurityTokenException("Refresh token inválido ou expirado");
        }

        // Gere novo JWT e refresh token;
        (string newJwtToken, RefreshToken newRefreshToken) = _jwtTokenGenerator.GenerateToken(userId: userId, name: "xd", email: "xd", roles: [], previousClaims: []);

        // Revogue o antigo refresh token e salve o novo no banco;
        await Update(oldRefreshTokens);
        await Save(newRefreshToken);

        return (newJwtToken, newRefreshToken.Token);
    }

    public async Task Save(RefreshToken newRefreshToken)
    {
        await _context.RefreshTokens.AddAsync(newRefreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task Update(List<RefreshToken> oldRefreshTokens)
    {
        await _context.RefreshTokens.
            Where(x => oldRefreshTokens.Any(y => y.RefreshTokenId == x.RefreshTokenId)).
            ExecuteUpdateAsync(x =>
            x.SetProperty(prop => prop.Status, false).
            SetProperty(prop => prop.Revoked, GerarHorarioBrasilia())
        );
    }
}