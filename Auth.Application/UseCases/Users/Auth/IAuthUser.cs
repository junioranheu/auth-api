using Auth.Application.UseCases.Users.Shared;

namespace Auth.Application.UseCases.Users.Auth;

public interface IAuthUser
{
    Task<UserOutput> Execute(string login, string password);
}