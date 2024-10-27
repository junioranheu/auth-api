using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static junioranheu_utils_package.Fixtures.Get;

namespace Auth.Domain.Entities;

public sealed class Log
{
    [Key]
    public int LogId { get; set; }

    public string? RequestType { get; set; } = string.Empty;

    public string? Endpoint { get; set; } = string.Empty;

    public string? Parameters { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;

    public int Status { get; set; }

    public int? UsuarioId { get; set; }
    [ForeignKey(nameof(UsuarioId))]
    public User? Users { get; init; }

    public DateTime Date { get; set; } = GerarHorarioBrasilia();
}