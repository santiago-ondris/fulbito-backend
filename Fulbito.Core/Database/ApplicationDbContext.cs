using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Fulbito.Core.Common.Entities;

namespace Fulbito.Core.Database;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<League> Leagues { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<PlayerMatch> PlayerMatches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuraciones
        ConfigureLeague(modelBuilder);
        ConfigurePlayer(modelBuilder);
        ConfigureMatch(modelBuilder);
        ConfigurePlayerMatch(modelBuilder);
    }

    private void ConfigureLeague(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<League>(entity =>
        {
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Slug).IsUnique();
            
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Leagues)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigurePlayer(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>(entity =>
        {
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            
            entity.HasOne(e => e.League)
                  .WithMany(l => l.Players)
                  .HasForeignKey(e => e.LeagueId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureMatch(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Match>(entity =>
        {
            entity.Property(e => e.MatchDate).IsRequired();
            
            entity.HasOne(e => e.League)
                  .WithMany(l => l.Matches)
                  .HasForeignKey(e => e.LeagueId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigurePlayerMatch(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerMatch>(entity =>
        {
            entity.HasOne(e => e.Player)
                  .WithMany(p => p.PlayerMatches)
                  .HasForeignKey(e => e.PlayerId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Match)
                  .WithMany(m => m.PlayerMatches)
                  .HasForeignKey(e => e.MatchId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Índice único para evitar duplicados
            entity.HasIndex(e => new { e.PlayerId, e.MatchId }).IsUnique();
        });
    }
}