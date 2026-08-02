using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScrumSoft.Domain.Entities;

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

            // Un usuario no puede estar dos veces en el mismo proyecto.
            builder.HasIndex(m => new { m.IdProyecto, m.IdUsuario })
                .IsUnique()
                .HasDatabaseName("ux_proyecto_usuarios");

            // Consulta "mis proyectos".
            builder.HasIndex(m => m.IdUsuario)
                .HasDatabaseName("ix_proyecto_usuarios_usuario");
        }
    }
}
