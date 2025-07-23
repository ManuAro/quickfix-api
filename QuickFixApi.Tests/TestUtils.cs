using Microsoft.EntityFrameworkCore;
using QuickFixApi.Data;
using QuickFixApi.Models;

namespace QuickFixApi.Tests
{
    public static class TestUtils
    {
        public static AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // 🧠 Nueva BD por test
                .Options;

            var context = new AppDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        public static void SeedAppointments(AppDbContext context)
        {
            context.Appointments.AddRange(
                new Appointment
                {
                    Id = 1,
                    ProviderId = 10,
                    ClientId = 100,
                    ProviderName = "Juan Pérez",
                    ClientName = "Pedro Gómez",
                    ProviderProfession = "Plomero",
                    Date = DateTime.Today,
                    Time = "10:00",
                    Status = "pending",
                    Location = "Montevideo",
                    Notes = "Llevar herramientas"
                },
                new Appointment
                {
                    Id = 2,
                    ProviderId = 11,
                    ClientId = 101,
                    ProviderName = "María López",
                    ClientName = "Ana Torres",
                    ProviderProfession = "Electricista",
                    Date = DateTime.Today.AddDays(1),
                    Time = "14:00",
                    Status = "confirmed",
                    Location = "Canelones",
                    Notes = "Revisar tablero"
                }
            );
            context.SaveChanges();
        }
        public static void SeedApplications(AppDbContext context)
        {
            context.Applications.AddRange(
                new Application
                {
                    Id = 1,
                    Name = "Laura Fernández",
                    Email = "laura@example.com",
                    Phone = "099876543",
                    Profession = "Pintor",
                    City = "Salto",
                    Experience = "2 años pintando casas",
                    About = "Trabajo prolijo y rápido",
                    HasCertifications = false,
                    HasTools = true,
                    AcceptTerms = true,
                    Status = "pending"
                },
                new Application
                {
                    Id = 2,
                    Name = "Martín Díaz",
                    Email = "martin@example.com",
                    Phone = "098765432",
                    Profession = "Gasista",
                    City = "Paysandú",
                    Experience = "10 años instalando cocinas y calefones",
                    About = "Responsable y puntual",
                    HasCertifications = true,
                    HasTools = true,
                    AcceptTerms = true,
                    Status = "confirmed"
                }
            );
            context.SaveChanges();
        }

        public static void SeedUsers(AppDbContext context)
        {
            context.Users.AddRange(
                new User
                {
                    Id = 1,
                    Name = "Juan Pérez",
                    Email = "juan@example.com",
                    Password = "1234",
                    UserType = "client",
                    Profession = "Plomero"
                },
                new User
                {
                    Id = 2,
                    Name = "Ana Torres",
                    Email = "ana@example.com",
                    Password = "abcd",
                    UserType = "provider",
                    Profession = "Electricista"
                }
            );
            context.SaveChanges();
        }

        public static void SeedAvailability(AppDbContext context)
        {
            context.Availabilities.AddRange(
                new Availability
                {
                    Id = 1,
                    ProviderId = 100,
                    Date = DateTime.Today,
                    Slots = new List<Slot>
                    {
                new Slot { Id = 1, Time = "10:00", Available = true, Booked = false },
                new Slot { Id = 2, Time = "11:00", Available = false, Booked = true }
                    }
                },
                new Availability
                {
                    Id = 2,
                    ProviderId = 200,
                    Date = DateTime.Today.AddDays(1),
                    Slots = new List<Slot>
                    {
                new Slot { Id = 3, Time = "14:00", Available = true, Booked = false }
                    }
                }
            );
            context.SaveChanges();
        }
    }
}