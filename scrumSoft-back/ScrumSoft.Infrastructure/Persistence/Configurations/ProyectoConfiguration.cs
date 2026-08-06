using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScrumSoft.Domain.Common;
using ScrumSoft.Domain.Entities;
using ScrumSoft.Infrastructure.Persistence.Converters;

namespace ScrumSoft.Infrastructure.Persistence.Configurations
{
    public sealed class ProyectoConfiguration : IEntityTypeConfiguration<Proyecto>
    {
        public void Configure(EntityTypeBuilder<Proyecto> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.ToTable("proyectos");

            builder.HasKey(p => p.Id);

            // El Guid lo genera la entidad al construirse, no la base de datos.
            builder.Property(p => p.Id)
                .HasColumnName("id_proyecto")
                .ValueGeneratedNever();

            builder.Property(p => p.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.Descripcion)
                .HasColumnName("descripcion");

            builder.Property(p => p.FechaInicio)
                .HasColumnName("fecha_inicio")
                .IsRequired();

            builder.Property(p => p.FechaFinPrevista)
                .HasColumnName("fecha_fin_prevista");

            // El enum se guarda como texto: legible en la base y estable si se
            // reordenan los valores del enum en C#.
            builder.Property(p => p.EstadoProyecto)
                .HasColumnName("estado_proyecto")
                .HasMaxLength(50)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(p => p.Estado)
                .HasColumnName("estado")
                .HasMaxLength(1)
                .HasConversion(new ConvertidorDeEstadoRegistro())
                .IsRequired();

            builder.Property(p => p.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .IsRequired();

            builder.Property(p => p.FechaActualizacion)
                .HasColumnName("fecha_actualizacion");

            // Columnas y Miembros son propiedades calculadas que devuelven una lista
            // nueva en cada lectura: EF no puede escribir en ellas. Se mapean los campos.
            builder.Ignore(p => p.Columnas);
            builder.Ignore(p => p.Miembros);

            builder.HasMany<Columna>("_columnas")
                .WithOne(c => c.Proyecto)
                .HasForeignKey(c => c.IdProyecto)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany<ProyectoUsuario>("_miembros")
                .WithOne(m => m.Proyecto)
                .HasForeignKey(m => m.IdProyecto)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation("_columnas").UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation("_miembros").UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(p => p.Nombre)
                .HasDatabaseName("ix_proyectos_nombre");

            // Lo eliminado logicamente no aparece en ninguna consulta.
            builder.HasQueryFilter(p => p.Estado == EstadoRegistro.Activo);

            builder.HasData(DatosSemilla.Proyectos());
        }
    }
}
