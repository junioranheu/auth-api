﻿using Auth.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static junioranheu_utils_package.Fixtures.Get;

namespace Auth.Infrastructure.Data;

public class Context(DbContextOptions<Context> options, IHttpContextAccessor httpContextAccessor) : DbContext(options)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UsuariosRoles { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    #region extras
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Cascade;
        }
    }

    public Guid CurrentUserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated ?? false)
            {
                string? userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return userId;
                }
            }

            return Guid.Empty;
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Audit>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity.CreatedDate is null)
                    {
                        entry.Entity.CreatedDate = GerarHorarioBrasilia();
                        entry.Entity.CreatedBy = CurrentUserId;
                        entry.Entity.Status = true;
                    }

                    break;

                case EntityState.Modified:
                    entry.Entity.LastModificationDate = GerarHorarioBrasilia();
                    entry.Entity.LastModificationBy = CurrentUserId;

                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
    #endregion
}