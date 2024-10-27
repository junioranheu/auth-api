using Auth.Domain.Entities;

namespace Auth.Application.UseCases.Logs.Create;

public interface ICreateLog
{
    Task Execute(Log input);
}