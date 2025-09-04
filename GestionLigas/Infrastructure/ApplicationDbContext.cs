using GestionLigas.Features.CrearLiga;
using Microsoft.EntityFrameworkCore;

namespace GestionLigas.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Liga> Ligas { get; set; }
    public DbSet<ConfiguracionLiga> ConfiguracionesLiga { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

    base.OnModelCreating(modelBuilder);
  }
}