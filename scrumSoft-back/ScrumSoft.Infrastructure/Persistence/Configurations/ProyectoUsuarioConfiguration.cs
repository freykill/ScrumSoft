using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScrumSoft.Domain.Common;
using ScrumSoft.Domain.Entities;
using ScrumSoft.Infrastructure.Persistence.Converters;

namespace ScrumSoft.Infrastructure.Persistence.Configurations
{
    public sealed class ProyectoUsuarioConfiguration : IEntityTypeConfiguration<ProyectoUsuario>
    {
        public void Configure(EntityTypeBuilder<ProyectoUsuario> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.ToTable("proyecto_usuarios");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .HasColumnName("id_proyecto_usuario")
                .ValueGeneratedNever();

            builder.Property(m => m.IdProyecto)
                .HasColumnName("id_proyecto")
                .IsRequired();

            builder.Property(m => m.IdUsuario)
                .HasColumnName("id_usuario")
                .IsRequired();

            builder.Property(m => m.FechaAsignacion)
                .HasColumnName("fecha_asignacion")
                .IsRequired();

            builder.Property(m => m.Estado)
                .HasColumnName("estado")
                .HasMaxLength(1)
                .HasConversion(new ConvertidorDeEstadoRegistro())
                .IsRequired();

            builder.Property(m => m.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .IsRequired();

            builder.Property(m => m.FechaActualizacion)
                .HasColumnName("fecha_actualizacion");

            // Un usuario no puede estar dos veces en el mismo proyecto. El indice es
            // parcial a proposito: al sacar a alguien la fila no se borra, se marca
            // como eliminada, y sin el filtro no se le podria volver a agregar nunca.
            // Las bajas anteriores quedan como historial de quien estuvo en el equipo.
            builder.HasIndex(m => new { m.IdProyecto, m.IdUsuario })
                .IsUnique()
                .HasFilter("estado = 'A'")
                .HasDatabaseName("ux_proyecto_usuarios");

            // Consulta "mis proyectos".
            builder.HasIndex(m => m.IdUsuario)
                .HasDatabaseName("ix_proyecto_usuarios_usuario");

            builder.HasQueryFilter(m => m.Estado == EstadoRegistro.Activo);

            builder.HasData(DatosSemilla.Miembros());
        }
    }
}
