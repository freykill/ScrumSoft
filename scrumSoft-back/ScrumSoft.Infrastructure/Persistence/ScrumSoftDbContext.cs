using Microsoft.EntityFrameworkCore;
using ScrumSoft.Domain.Entities;
using ScrumSoft.Infrastructure.Persistence.Configurations;

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

            // Una linea por entidad: se ve de un vistazo que hay cinco tablas
            // y donde esta configurada cada una.
            modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
            modelBuilder.ApplyConfiguration(new ProyectoConfiguration());
            modelBuilder.ApplyConfiguration(new ColumnaConfiguration());
            modelBuilder.ApplyConfiguration(new TareaConfiguration());
            modelBuilder.ApplyConfiguration(new ProyectoUsuarioConfiguration());
        }
    }
}
