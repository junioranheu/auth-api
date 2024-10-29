using Auth.Domain.Entities;

namespace Auth.Application.UseCases.Auth.CreateRefreshTokenJWT;

public interface ICreateRefreshToken
{
    Task<(string newJwtToken, string newRefreshToken)> RefreshToken(Guid userId);
    Task Save(RefreshToken newRefreshToken);
    Task Update(List<RefreshToken> oldRefreshTokens);
}