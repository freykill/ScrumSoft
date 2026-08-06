using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScrumSoft.Domain.Common;
using ScrumSoft.Domain.Entities;
using ScrumSoft.Infrastructure.Persistence.Converters;

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

            builder.Property(c => c.Estado)
                .HasColumnName("estado")
                .HasMaxLength(1)
                .HasConversion(new ConvertidorDeEstadoRegistro())
                .IsRequired();

            builder.Property(c => c.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .IsRequired();

            builder.Property(c => c.FechaActualizacion)
                .HasColumnName("fecha_actualizacion");

            // La consulta del tablero: columnas de un proyecto, en orden.
            builder.HasIndex(c => new { c.IdProyecto, c.Orden })
                .HasDatabaseName("ix_columnas_proyecto_orden");

            builder.HasQueryFilter(c => c.Estado == EstadoRegistro.Activo);

            builder.HasData(DatosSemilla.Columnas());
        }
    }
}
