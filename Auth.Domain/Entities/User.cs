using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Auth.Domain.Entities;

[Index(nameof(Email))]
[Index(nameof(UserName))]
public sealed class User : Audit
{
    [Key]
    public Guid UserId { get; set; }

    [MaxLength(255)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(30)]
    public string UserName { get; set; } = string.Empty;

    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool IsVerified { get; set; } = false;

    public string? VerificationCode { get; set; } = string.Empty;

    public DateTime? VerificationCodeValidity { get; set; }

    public string? ChangePasswordCode { get; set; } = null;

    public DateTime? ChangePasswordCodeValidity { get; set; }

    public IEnumerable<UserRole>? UserRoles { get; init; }
}