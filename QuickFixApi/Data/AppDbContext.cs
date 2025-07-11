using Microsoft.EntityFrameworkCore;
using QuickFixApi.Models; // Asegurate que apunte al namespace correcto de tus modelos

namespace QuickFixApi.Data
{
    // 👇 Esta clase representa la conexión con la base de datos
    public class AppDbContext : DbContext
    {
        // 👇 Constructor que recibe las opciones (como la connection string)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // 👇 Estas propiedades representan las tablas de la base de datos
        public DbSet<User> Users { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Application> Applications { get; set; }

        // 👇 Acá podés configurar más detalles, como nombres de tablas o relaciones
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ejemplo de configuración: nombre de tabla manual
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Provider>().ToTable("Providers");
            modelBuilder.Entity<Appointment>().ToTable("Appointments");
            modelBuilder.Entity<Service>().ToTable("Services");
            modelBuilder.Entity<Availability>().ToTable("Availability");
            modelBuilder.Entity<Application>().ToTable("Applications");
        }
    }
}

