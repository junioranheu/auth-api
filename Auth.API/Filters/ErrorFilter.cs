using Auth.API.Filters.Base;
using Auth.Application.UseCases.Logs.Create;
using Auth.Domain.Entities;
using Auth.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using static junioranheu_utils_package.Fixtures.Get;

namespace Auth.API.Filters;

public sealed class ErrorFilter(ILogger<ErrorFilter> logger, ICreateLog createLog) : ExceptionFilterAttribute
{
    private readonly ILogger _logger = logger;
    private readonly ICreateLog _createLog = createLog;

    public override async Task OnExceptionAsync(ExceptionContext context)
    {
        Exception ex = context.Exception;
        string error = $"Ocorreu um erro ao processar sua requisição. Data: {ObterDetalhesDataHora()}. Caminho: {context.HttpContext.Request.Path}. {(!string.IsNullOrEmpty(ex.InnerException?.Message) ? $"Mais informações: {ex.InnerException.Message}" : $"Mais informações: {ex.Message}")}";
        string errorSimple = !string.IsNullOrEmpty(ex.InnerException?.Message) ? ex.InnerException.Message : ex.Message;

        var detalhes = new BadRequestObjectResult(new
        {
            Codigo = StatusCodes.Status500InternalServerError,
            Data = ObterDetalhesDataHora(),
            Caminho = context.HttpContext.Request.Path,
            Mensagens = new string[] { errorSimple },
            IsErro = true
        });

        (UserRoleEnum[] _, Guid userId) = new BaseFilter().BaseGetUserId(context);
        await CreateLog(context, error, userId);
        Logger(ex, error);

        context.Result = detalhes;
        context.ExceptionHandled = true;
    }

    private async Task CreateLog(ExceptionContext context, string error, Guid? userId)
    {
        Log log = new()
        {
            RequestType = context.HttpContext.Request.Method ?? string.Empty,
            Endpoint = context.HttpContext.Request.Path.ToString() ?? string.Empty,
            Parameters = string.Empty,
            Description = error,
            Status = StatusCodes.Status500InternalServerError,
            UserId = userId is null || userId == Guid.Empty ? null : userId
        };

        await _createLog.Execute(log);
    }

    private void Logger(Exception ex, string error)
    {
        _logger.LogError(ex, "{error}", error);
    }
}