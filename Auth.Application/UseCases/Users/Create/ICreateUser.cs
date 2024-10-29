using Auth.Application.UseCases.Users.Shared;

namespace Auth.Application.UseCases.Users.Create;

public interface ICreateUser
{
    Task<UserOutput> Execute(UserInput input);
}