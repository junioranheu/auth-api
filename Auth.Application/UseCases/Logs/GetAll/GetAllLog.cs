﻿using Auth.Application.UseCases.Shared;
using Auth.Domain.Entities;
using Auth.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.UseCases.Logs.GetAll;

public sealed class GetAllLog(Context context) : IGetAllLog
{
    private readonly Context _context = context;

    public async Task<(IEnumerable<Log> linq, int count)> Execute(PaginationInput pagination, Guid? userId)
    {
        var query = _context.Logs.
                    Include(x => x.Users).
                    AsNoTracking().
                    Where(x =>
                       (userId == null || x.Users!.UserId == userId)
                    ).OrderByDescending(x => x.Date);

        (IEnumerable<Log> linq, int count) = await PagedQuery.Execute(query, pagination);

        return (linq, count);
    }
}