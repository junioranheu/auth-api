using Auth.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Auth.Domain.Entities;

public sealed class UserRole : Audit
{
    [Key]
    public Guid UserRoleId { get; set; }

    public Guid UserId { get; set; }
    [JsonIgnore]
    public User? Users { get; set; }

    public UserRoleEnum Role { get; set; }
}