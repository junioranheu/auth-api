﻿using Auth.Application.UseCases.Users.GetByUserNameOrEmail;
using Auth.Application.UseCases.Users.Shared;
using Auth.Domain.Entities;
using Auth.Infrastructure.Auth.Token;
using AutoMapper;
using static junioranheu_utils_package.Fixtures.Encrypt;

namespace Auth.Application.UseCases.Auth.CreateTokenJWT;

public sealed class CreateToken(
    IMapper map,
    IJwtTokenGenerator jwtTokenGenerator,
    IGetUserByUserNameOrEmail getUserByUserNameOrEmail) : ICreateToken
{
    private readonly IMapper _map = map;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IGetUserByUserNameOrEmail _getUserByUserNameOrEmail = getUserByUserNameOrEmail;

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
            throw new Exception("Usuário desativado");
        }

        (string token, RefreshToken refreshToken) = _jwtTokenGenerator.GenerateToken(userId: output.UserId, name: output.FullName, email: output.Email, roles: output.UserRoles?.ToArray(), previousClaims: []);

        return output;
    }
}