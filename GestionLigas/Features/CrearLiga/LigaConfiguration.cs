using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionLigas.Features.CrearLiga;

public class LigaConfiguration : IEntityTypeConfiguration<Liga>
{
    public void Configure(EntityTypeBuilder<Liga> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(e => e.UsuarioId).IsRequired().HasMaxLength(50);
        builder.Property(e => e.FechaCreacion).IsRequired();
        
        // Relación uno a uno con ConfiguracionLiga
        builder.HasOne(e => e.Configuracion)
               .WithOne(e => e.Liga)
               .HasForeignKey<ConfiguracionLiga>(e => e.LigaId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}