using Auth.Domain.Entities;

namespace Auth.Application.UseCases.Auth.CreateRefreshTokenJWT;

public interface ICreateRefreshToken
{
    Task Execute(RefreshToken refreshToken);
}