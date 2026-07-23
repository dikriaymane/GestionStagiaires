using GestionStagiaires.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestionStagiaires.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Stagiaire> Stagiaires { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Stagiaire>()
            .HasOne(s => s.User)
            .WithOne()
            .HasForeignKey<Stagiaire>(s => s.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
