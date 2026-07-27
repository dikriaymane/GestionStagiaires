using GestionStagiaires.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestionStagiaires.Data;
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options){}
    public DbSet<Stagiaire> Stagiaires { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Stagiaire>()
            .HasOne(s => s.User)
            .WithOne()
            .HasForeignKey<Stagiaire>(s => s.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.Entity<DemandeDocument>()
            .HasOne(d => d.Stagiaire)
            .WithMany()
            .HasForeignKey(d => d.StagiaireId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<DocumentStagiaire>()
            .HasOne(d => d.Stagiaire)
            .WithMany()
            .HasForeignKey(d => d.StagiaireId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    public DbSet<DemandeDocument> DemandesDocuments { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<DocumentStagiaire> DocumentsStagiaires { get; set; } = null!;
}
