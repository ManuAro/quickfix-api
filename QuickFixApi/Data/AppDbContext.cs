using Microsoft.EntityFrameworkCore;
using QuickFixApi.Models;

namespace QuickFixApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Review> Reviews { get; set; } // 👈 NUEVO

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🧱 Mapeo explícito de nombres de tabla (coherente con Supabase)
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Provider>().ToTable("providers");
            modelBuilder.Entity<Appointment>().ToTable("appointments");
            modelBuilder.Entity<Service>().ToTable("services");
            modelBuilder.Entity<Availability>().ToTable("availability");
            modelBuilder.Entity<Application>().ToTable("applications");
            modelBuilder.Entity<Review>().ToTable("reviews"); // 👈 NUEVO
        }
    }
}


