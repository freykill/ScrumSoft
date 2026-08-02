using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScrumSoft.Domain.Entities;

namespace ScrumSoft.Infrastructure.Persistence.Configurations
{
    public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.ToTable("usuarios");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasColumnName("id_usuario")
                .ValueGeneratedNever();

            builder.Property(u => u.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(u => u.CorreoElectronico)
                .HasColumnName("correo_electronico")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(u => u.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(u => u.Rol)
                .HasColumnName("rol")
                .HasMaxLength(50)
                .HasConversion<string>()
                .IsRequired();

            builder.Ignore(u => u.Proyectos);

            builder.HasMany<ProyectoUsuario>("_proyectos")
                .WithOne(m => m.Usuario)
                .HasForeignKey(m => m.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation("_proyectos").UsePropertyAccessMode(PropertyAccessMode.Field);

            // Dos usuarios no pueden compartir correo: es la llave del inicio de sesion.
            builder.HasIndex(u => u.CorreoElectronico)
                .IsUnique()
                .HasDatabaseName("ux_usuarios_correo");

            // Requisito 6.2: los dos usuarios precargados quedan dentro de la migracion,
            // asi que se crean solos al levantar la base desde cero.
            builder.HasData(DatosSemilla.Usuarios());
        }
    }
}
