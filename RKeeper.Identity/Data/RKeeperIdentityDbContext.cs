using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RKeeper.Identity.Models;

namespace RKeeper.Identity.Data;

public class RKeeperIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public static readonly string ConfigurationName = "IdentityDbContext";

    public RKeeperIdentityDbContext(DbContextOptions<RKeeperIdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("account");
        // each User can have many entries in the UserRole join table
        builder.Entity<ApplicationUser>(b =>
            b.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired());

        // each Role can have many entries in the UserRole join table
        builder.Entity<ApplicationRole>(b =>
            b.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired());
    }
}
