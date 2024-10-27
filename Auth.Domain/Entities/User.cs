using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static junioranheu_utils_package.Fixtures.Get;

namespace Auth.Domain.Entities;

[Index(nameof(Email))]
[Index(nameof(UserName))]
public sealed class User
{
    [Key]
    public int UserId { get; set; }

    [MaxLength(255)]
    public string FullName { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool IsVerified { get; set; } = false;

    public string? VerificationCode { get; set; } = string.Empty;

    public DateTime? VerificationCodeValidity { get; set; }

    public string? ChangePasswordCode { get; set; } = null;

    public DateTime? ChangePasswordCodeValidity { get; set; }

    public bool Status { get; set; } = true;

    public DateTime Data { get; set; } = GerarHorarioBrasilia();

    public IEnumerable<UserRole>? UsuarioRoles { get; init; }
}