using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ScrumSoft.Domain.Common;
using ScrumSoft.Domain.Entities;
using ScrumSoft.Infrastructure.Persistence.Converters;

namespace ScrumSoft.Infrastructure.Persistence
{
    public sealed class ScrumSoftDbContext(DbContextOptions<ScrumSoftDbContext> opciones)
        : DbContext(opciones)
    {
        public DbSet<Proyecto> Proyectos => Set<Proyecto>();

        public DbSet<Columna> Columnas => Set<Columna>();

        public DbSet<Tarea> Tareas => Set<Tarea>();

        public DbSet<Usuario> Usuarios => Set<Usuario>();

        public DbSet<ProyectoUsuario> ProyectoUsuarios => Set<ProyectoUsuario>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            base.OnModelCreating(modelBuilder);

            // Toma todas las clases *Configuration de este ensamblado.
            // Al agregar una entidad nueva no hay que tocar este archivo.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ScrumSoftDbContext).Assembly);

            ConfigurarColumnasComunes(modelBuilder);
            AplicarFiltroDeBorradoLogico(modelBuilder);
        }

        // Estado y fechas de auditoria los tienen las cinco tablas. Se configuran
        // una vez recorriendo el modelo, en vez de repetirlo en cada Configuration.
        private static void ConfigurarColumnasComunes(ModelBuilder modelBuilder)
        {
            var convertidor = new ConvertidorDeEstadoRegistro();

            foreach (var tipo in modelBuilder.Model.GetEntityTypes())
            {
                if (!typeof(BaseEntity).IsAssignableFrom(tipo.ClrType))
                    continue;

                var entidad = modelBuilder.Entity(tipo.ClrType);

                entidad.Property(nameof(BaseEntity.Estado))
                    .HasColumnName("estado")
                    .HasMaxLength(1)
                    .HasConversion(convertidor)
                    .IsRequired();

                entidad.Property(nameof(BaseEntity.FechaCreacion))
                    .HasColumnName("fecha_creacion")
                    .IsRequired();

                entidad.Property(nameof(BaseEntity.FechaActualizacion))
                    .HasColumnName("fecha_actualizacion");
            }
        }

        // Sin esto habria que escribir .Where(x => x.Estado == Activo) en cada consulta
        // del proyecto, y el dia que se olvide una, esa pantalla muestra datos eliminados.
        private static void AplicarFiltroDeBorradoLogico(ModelBuilder modelBuilder)
        {
            foreach (var tipo in modelBuilder.Model.GetEntityTypes())
            {
                if (!typeof(BaseEntity).IsAssignableFrom(tipo.ClrType))
                    continue;

                var parametro = Expression.Parameter(tipo.ClrType, "e");

                var comparacion = Expression.Equal(
                    Expression.Property(parametro, nameof(BaseEntity.Estado)),
                    Expression.Constant(EstadoRegistro.Activo));

                modelBuilder.Entity(tipo.ClrType)
                    .HasQueryFilter(Expression.Lambda(comparacion, parametro));
            }
        }
    }
}
