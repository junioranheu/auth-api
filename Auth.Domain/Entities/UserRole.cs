using Auth.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static junioranheu_utils_package.Fixtures.Get;

namespace Auth.Domain.Entities;

public sealed class UserRole
{
    [Key]
    public Guid UserRoleId { get; set; }

    public Guid UserId { get; set; }
    [JsonIgnore]
    public User? Users { get; set; }

    public UserRoleEnum Role { get; set; }

    public DateTime Data { get; set; } = GerarHorarioBrasilia();
}