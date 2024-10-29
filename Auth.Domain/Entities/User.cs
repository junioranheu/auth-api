﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static junioranheu_utils_package.Fixtures.Get;

namespace Auth.Domain.Entities;

[Index(nameof(Email))]
[Index(nameof(UserName))]
public sealed class User
{
    [Key]
    public Guid UserId { get; set; }

    [MaxLength(255)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(30)]
    public string UserName { get; set; } = string.Empty;

    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Password { get; set; } = string.Empty;

    public bool IsVerified { get; set; } = false;

    public string? VerificationCode { get; set; } = string.Empty;

    public DateTime? VerificationCodeValidity { get; set; }

    public string? ChangePasswordCode { get; set; } = null;

    public DateTime? ChangePasswordCodeValidity { get; set; }

    public bool Status { get; set; } = true;

    public DateTime Date { get; set; } = GerarHorarioBrasilia();

    public IEnumerable<UserRole>? UserRoles { get; init; }
}