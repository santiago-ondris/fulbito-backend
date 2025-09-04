using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionLigas.Features.CrearLiga;

public class ConfiguracionLigaConfiguration : IEntityTypeConfiguration<ConfiguracionLiga>
{
    public void Configure(EntityTypeBuilder<ConfiguracionLiga> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Formato).IsRequired().HasMaxLength(10);
        builder.Property(e => e.ArqueroObligatorio).IsRequired();
        builder.Property(e => e.CantidadMinimaArqueros).IsRequired();
        
        // Para las listas, las guardamos como JSON
        builder.Property(e => e.PosicionesDisponibles)
               .HasConversion(
                   v => string.Join(',', v),
                   v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
               .HasMaxLength(500);
               
        builder.Property(e => e.DiasJuego)
               .HasConversion(
                   v => string.Join(',', v),
                   v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
               .HasMaxLength(200);
               
        builder.Property(e => e.Horarios).HasMaxLength(100);
        builder.Property(e => e.ReglasAdicionales).HasMaxLength(1000);
    }
}