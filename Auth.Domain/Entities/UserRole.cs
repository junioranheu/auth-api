using Auth.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static junioranheu_utils_package.Fixtures.Get;

namespace Auth.Domain.Entities;

public sealed class UsuarioRole
{
    [Key]
    public int UserRoleId { get; set; }

    public int UserId { get; set; }
    [JsonIgnore]
    public User? Users { get; set; }

    public UserRoleEnum UserRole { get; set; }

    public DateTime Data { get; set; } = GerarHorarioBrasilia();
}