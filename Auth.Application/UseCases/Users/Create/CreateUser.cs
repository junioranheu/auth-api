using Auth.Application.UseCases.Users.GetByUserNameOrEmail;
using Auth.Application.UseCases.Users.Shared;
using Auth.Domain.Entities;
using Auth.Infrastructure.Data;
using AutoMapper;
using static junioranheu_utils_package.Fixtures.Encrypt;
using static junioranheu_utils_package.Fixtures.Get;

namespace Auth.Application.UseCases.Users.Create;

public sealed class CreateUser(Context context, IMapper map, IGetUserByUserNameOrEmail getUserByUserNameOrEmail) : ICreateUser
{
    private readonly Context _context = context;
    private readonly IMapper _map = map;
    private readonly IGetUserByUserNameOrEmail _getUserByUserNameOrEmail = getUserByUserNameOrEmail;

    public async Task<UserOutput> Execute(UserInput input)
    {
        await Validations(input);
        User user = await SaveUser(input);
        await SaveUserRole(input, user.UserId);

        UserOutput? output = _map.Map<UserOutput>(user);

        return output;
    }

    private async Task Validations(UserInput input)
    {
        (User? checkUserByUserName, string _) = await _getUserByUserNameOrEmail.Execute(input.UserName);

        if (checkUserByUserName is not null)
        {
            throw new Exception("Já existe um usuário com esse nome de usuário");
        }

        (User? checkUserByEmail, string _) = await _getUserByUserNameOrEmail.Execute(input.Email);

        if (checkUserByEmail is not null)
        {
            throw new Exception("Já existe um usuário com esse e-mail");
        }
    }

    private async Task<User> SaveUser(UserInput input)
    {
        DateTime date = GerarHorarioBrasilia();

        User user = new()
        {
            FullName = input.FullName,
            UserName = input.UserName,
            Email = input.Email,
            Password = Criptografar(input.Password),
            IsVerified = false,
            VerificationCode = GerarStringAleatoria(17, false),
            VerificationCodeValidity = date.AddDays(7),
            ChangePasswordCode = GerarStringAleatoria(22, false),
            ChangePasswordCodeValidity = date.AddDays(7)
        };

        await _context.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    private async Task SaveUserRole(UserInput input, Guid userId)
    {
        UserRole userRole = new()
        {
            UserId = userId,
            Role = input.UserRole
        };

        await _context.AddAsync(userRole);
        await _context.SaveChangesAsync();
    }
}