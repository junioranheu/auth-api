﻿using Auth.Domain.Entities;
using Auth.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.UseCases.Users.GetByUserNameOrEmail;

public sealed class GetUserByUserNameOrEmail(Context context) : IGetUserByUserNameOrEmail
{
    private readonly Context _context = context;

    public async Task<(User? user, string passwordEncrypted)> Execute(string login)
    {
        var user = await _context.Users.
                   AsNoTracking().
                   Where(x => (x.Email == login || x.UserName == login)).
                   Include(x => x.UserRoles).
                   FirstOrDefaultAsync();

        if (user is null)
        {
            return (null, string.Empty);
        }

        return (user, user.Password);
    }
}