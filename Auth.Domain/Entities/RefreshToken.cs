﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static junioranheu_utils_package.Fixtures.Get;

namespace Auth.Domain.Entities;

public sealed class RefreshToken
{
    [Key]
    public Guid RefreshTokenId { get; set; }

    public string Token { get; set; } = string.Empty;

    public Guid UserId { get; set; }
    [JsonIgnore]
    public User? Users { get; set; }

    public DateTime? Expires { get; set; }

    public DateTime Created { get; set; } = GerarHorarioBrasilia();

    public DateTime? Revoked { get; set; }
}