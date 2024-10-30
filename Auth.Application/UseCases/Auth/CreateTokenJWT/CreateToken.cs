using Auth.Application.UseCases.Auth.CreateRefreshTokenJWT;
using Auth.Application.UseCases.Auth.Shared;
using Auth.Application.UseCases.Users.GetByUserNameOrEmail;
using Auth.Application.UseCases.Users.Shared;
using Auth.Domain.Entities;
using Auth.Infrastructure.Auth.Token;
using AutoMapper;
using static junioranheu_utils_package.Fixtures.Encrypt;

namespace Auth.Application.UseCases.Auth.CreateTokenJWT;

public sealed class CreateToken(
    IMapper map,
    IJwtTokenGenerator jwtTokenGenerator,
    ICreateRefreshToken createRefreshToken,
    IGetUserByUserNameOrEmail getUserByUserNameOrEmail) : ICreateToken
{
    private readonly IMapper _map = map;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly ICreateRefreshToken _createRefreshToken = createRefreshToken;
    private readonly IGetUserByUserNameOrEmail _getUserByUserNameOrEmail = getUserByUserNameOrEmail;

    public async Task<UserOutput> Execute(AuthInput input)
    {
        (User? user, string passwordEncrypted) = await _getUserByUserNameOrEmail.Execute(input.Login);
        UserOutput? output = _map.Map<UserOutput>(user);

        if (output is null)
        {
            throw new Exception("Usuário não encontrado");
        }

        if (!VerificarCriptografia(senha: input.Password, senhaCriptografada: passwordEncrypted))
        {
            throw new Exception("Nome de usuário ou senha incorretos");
        }

        if (!output.Status)
        {
            throw new Exception("Usuário desativado");
        }

        (string token, RefreshToken refreshToken) = _jwtTokenGenerator.GenerateToken(userId: output.UserId, name: output.FullName, email: output.Email, roles: output.UserRoles?.ToArray());

        // Atualizar token no output;
        output.Token = token;

        // Revogar todos os refresh tokens antigos, caso existam;
        await _createRefreshToken.Update(userId: output.UserId, mustCheckForValidRefreshTokens: false);

        // Salvar o refresh token no banco;
        await _createRefreshToken.Save(refreshToken);

        return output;
    }
}