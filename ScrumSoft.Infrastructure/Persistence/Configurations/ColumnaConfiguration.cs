using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScrumSoft.Domain.Entities;

namespace ScrumSoft.Infrastructure.Persistence.Configurations
{
    public sealed class ColumnaConfiguration : IEntityTypeConfiguration<Columna>
    {
        public void Configure(EntityTypeBuilder<Columna> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.ToTable("columnas");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("id_columna")
                .ValueGeneratedNever();

            builder.Property(c => c.IdProyecto)
                .HasColumnName("id_proyecto")
                .IsRequired();

            builder.Property(c => c.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.Orden)
                .HasColumnName("orden")
                .IsRequired();

            // La consulta del tablero: columnas de un proyecto, en orden.
            builder.HasIndex(c => new { c.IdProyecto, c.Orden })
                .HasDatabaseName("ix_columnas_proyecto_orden");
        }
    }
}
