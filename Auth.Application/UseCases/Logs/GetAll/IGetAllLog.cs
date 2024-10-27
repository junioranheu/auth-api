using Auth.Application.UseCases.Shared;
using Auth.Domain.Entities;

namespace Auth.Application.UseCases.Logs.GetAll;

public interface IGetAllLog
{
    Task<(IEnumerable<Log> linq, int count)> Execute(PaginationInput pagination, Guid? userId);
}