using Auth.Domain.Entities;

namespace Auth.Application.UseCases.Users.GetByUserNameOrEmail;

public interface IGetUserByUserNameOrEmail
{
    Task<(User? user, string passwordEncrypted)> Execute(string login);
}