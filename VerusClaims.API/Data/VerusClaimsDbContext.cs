using Microsoft.EntityFrameworkCore;
using VerusClaims.API.Models;

namespace VerusClaims.API.Data;

public class VerusClaimsDbContext : DbContext
{
    public VerusClaimsDbContext(DbContextOptions<VerusClaimsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Claim> Claims { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Claim>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClaimNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ClaimantName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ClaimantEmail).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ClaimantPhone).HasMaxLength(20);
            entity.Property(e => e.ClaimAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.HasIndex(e => e.ClaimNumber).IsUnique();
            
            // Soft delete query filter
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ChangedBy).HasMaxLength(200);
            entity.HasIndex(e => new { e.EntityType, e.EntityId });
            entity.HasIndex(e => e.ChangedAt);
        });
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Claim && (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted));

        foreach (var entry in entries)
        {
            var claim = (Claim)entry.Entity;
            var now = DateTime.UtcNow;
            var currentUser = "System"; // TODO: Get from IHttpContextAccessor or IUserService

            if (entry.State == EntityState.Added)
            {
                claim.CreatedAt = now;
                claim.CreatedBy = currentUser;
            }
            else if (entry.State == EntityState.Modified)
            {
                claim.UpdatedAt = now;
                claim.UpdatedBy = currentUser;
            }
            else if (entry.State == EntityState.Deleted)
            {
                // Soft delete
                entry.State = EntityState.Modified;
                claim.IsDeleted = true;
                claim.DeletedAt = now;
                claim.DeletedBy = currentUser;
            }
        }
    }
}

