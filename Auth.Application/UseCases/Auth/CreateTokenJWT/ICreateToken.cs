using Auth.Application.UseCases.Users.Shared;

namespace Auth.Application.UseCases.Auth.CreateTokenJWT
{
    public interface ICreateToken
    {
        Task<UserOutput> Execute(string login, string password);
    }
}