using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScrumSoft.Domain.Common;
using ScrumSoft.Domain.Entities;
using ScrumSoft.Infrastructure.Persistence.Converters;

namespace ScrumSoft.Infrastructure.Persistence.Configurations
{
    public sealed class TareaConfiguration : IEntityTypeConfiguration<Tarea>
    {
        public void Configure(EntityTypeBuilder<Tarea> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.ToTable("tareas");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .HasColumnName("id_tarea")
                .ValueGeneratedNever();

            builder.Property(t => t.Titulo)
                .HasColumnName("titulo")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(t => t.Descripcion)
                .HasColumnName("descripcion");

            builder.Property(t => t.Prioridad)
                .HasColumnName("prioridad")
                .HasMaxLength(50)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(t => t.IdResponsable)
                .HasColumnName("id_responsable");

            builder.Property(t => t.IdColumna)
                .HasColumnName("id_columna")
                .IsRequired();

            builder.Property(t => t.Orden)
                .HasColumnName("orden")
                .IsRequired();

            builder.Property(t => t.Estado)
                .HasColumnName("estado")
                .HasMaxLength(1)
                .HasConversion(new ConvertidorDeEstadoRegistro())
                .IsRequired();

            builder.Property(t => t.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .IsRequired();

            builder.Property(t => t.FechaActualizacion)
                .HasColumnName("fecha_actualizacion");

            // Restrict: la regla de "no eliminar columna con tareas" vive en el dominio,
            // pero la base tambien la respalda.
            builder.HasOne(t => t.Columna)
                .WithMany()
                .HasForeignKey(t => t.IdColumna)
                .OnDelete(DeleteBehavior.Restrict);

            // Si se borra el usuario, la tarea queda sin responsable, no se borra.
            builder.HasOne(t => t.Responsable)
                .WithMany()
                .HasForeignKey(t => t.IdResponsable)
                .OnDelete(DeleteBehavior.SetNull);

            // La consulta principal del tablero.
            builder.HasIndex(t => new { t.IdColumna, t.Orden })
                .HasDatabaseName("ix_tareas_columna_orden");

            builder.HasIndex(t => t.IdResponsable)
                .HasDatabaseName("ix_tareas_responsable");

            builder.HasQueryFilter(t => t.Estado == EstadoRegistro.Activo);

            builder.HasData(DatosSemilla.Tareas());
        }
    }
}
