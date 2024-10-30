using Auth.Domain.Entities;

namespace Auth.Application.UseCases.Auth.CreateRefreshTokenJWT;

public interface ICreateRefreshToken
{
    Task<string> RefreshToken(Guid userId);
    Task Save(RefreshToken newRefreshToken);
    Task Update(Guid userId, bool mustCheckForValidRefreshTokens);
}