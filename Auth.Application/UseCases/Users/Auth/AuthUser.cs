using Auth.Application.UseCases.Users.GetByUserNameOrEmail;
using Auth.Application.UseCases.Users.Shared;
using Auth.Domain.Entities;
using Auth.Infrastructure.Auth.Token;
using AutoMapper;
using static junioranheu_utils_package.Fixtures.Encrypt;

namespace Auth.Application.UseCases.Users.Auth;

public sealed class AuthUser() : IAuthUser
{
    private readonly IMapper _map;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IGetUserByUserNameOrEmail _getUserByUserNameOrEmail;
    private readonly ICriarRefreshTokenUseCase _criarRefreshTokenUseCase;

    public AuthUser(
        IMapper map,
        IJwtTokenGenerator jwtTokenGenerator,
        IGetUserByUserNameOrEmail getUserByUserNameOrEmail,
        ICriarRefreshTokenUseCase criarRefreshTokenUseCase)
    {
        _map = map;
        _jwtTokenGenerator = jwtTokenGenerator;
        _getUserByUserNameOrEmail = getUserByUserNameOrEmail;
        _criarRefreshTokenUseCase = criarRefreshTokenUseCase;
    }

    public async Task<UserOutput> Execute(string login, string password)
    {
        (User? user, string passwordEncrypted) = await _getUserByUserNameOrEmail.Execute(login);
        UserOutput? output = _map.Map<UserOutput>(user);

        if (output is null)
        {
            throw new Exception("Usuário não encontrado");
        }

        if (!VerificarCriptografia(senha: password, senhaCriptografada: passwordEncrypted))
        {
            throw new Exception("Nome de usuário ou senha incorretos");
        }

        if (!output.Status)
        {
            throw new Exception("Usuáriod desativado");
        }

        output.Token = _jwtTokenGenerator.GenerateToken(userId: output.UserId, name: output.FullName, email: output.Email, roles: output.UserRoles?.ToArray(), previousClaims: []);
        output = await GerarRefreshToken(_jwtTokenGenerator, _criarRefreshTokenUseCase, output, output.UsuarioId);

        return output;
    }
}