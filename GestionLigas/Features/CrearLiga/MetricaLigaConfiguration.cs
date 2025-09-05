using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionLigas.Features.CrearLiga;

public class MetricaLigaConfiguration : IEntityTypeConfiguration<MetricaLiga>
{
    public void Configure(EntityTypeBuilder<MetricaLiga> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.LigaId).IsRequired();
        builder.Property(e => e.Tipo).IsRequired();
        builder.Property(e => e.Puntos).IsRequired();
        builder.Property(e => e.EsObligatoria).IsRequired();
        builder.Property(e => e.ParametroNumerico);
        
        // Relación con Liga
        builder.HasOne(e => e.Liga)
               .WithMany(e => e.Metricas)
               .HasForeignKey(e => e.LigaId)
               .OnDelete(DeleteBehavior.Cascade);
               
        // Índice compuesto para evitar métricas duplicadas en la misma liga
        builder.HasIndex(e => new { e.LigaId, e.Tipo })
               .IsUnique();
    }
}