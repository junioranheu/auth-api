using Auth.Domain.Entities;
using Auth.Infrastructure.Data;

namespace Auth.Application.UseCases.Auth.CreateRefreshTokenJWT;

public sealed class CreateRefreshToken(Context context) : ICreateRefreshToken
{
    private readonly Context _context = context;

    public async Task Execute(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }
}