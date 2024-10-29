using Auth.Application.UseCases.Auth.Shared;
using Auth.Application.UseCases.Users.Shared;

namespace Auth.Application.UseCases.Auth.CreateTokenJWT
{
    public interface ICreateToken
    {
        Task<UserOutput> Execute(AuthInput input);
    }
}