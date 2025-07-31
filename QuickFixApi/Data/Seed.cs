using QuickFixApi.Models;

namespace QuickFixApi.Data
{
    public static class Seed
    {
        public static void SeedData(AppDbContext context)
        {
            if (context.Providers.Any() || context.Users.Any() || context.Appointments.Any())
                return; // Ya hay datos, no hacer nada

            // Crear un usuario cliente
            var client = new User
            {
                Name = "Juan Cliente",
                Email = "juan@ejemplo.com",
                UserType = "Client"
            };

            // Crear dos proveedores
            var provider1 = new Provider
            {
                Name = "Ana Plomera",
                Email = "ana@fix.com",
                Profession = "Plomería",
                Location = "Montevideo"
            };

            var provider2 = new Provider
            {
                Name = "Carlos Electricista",
                Email = "carlos@fix.com",
                Profession = "Electricidad",
                Location = "Canelones"
            };

            // Crear disponibilidad con slots para provider1
            var availability1 = new Availability
            {
                ProviderId = 1, // Lo corregimos después con los IDs reales
                Date = DateTime.Today.AddDays(1),
                Slots = new List<Slot>
                {
                    new Slot { Time = "10:00", Available = true, Booked = true },
                    new Slot { Time = "10:30", Available = true, Booked = true },
                    new Slot { Time = "11:00", Available = true, Booked = false }
                }
            };

            // Crear disponibilidad con slots para provider2
            var availability2 = new Availability
            {
                ProviderId = 2, // Lo corregimos después
                Date = DateTime.Today.AddDays(2),
                Slots = new List<Slot>
                {
                    new Slot { Time = "14:00", Available = true, Booked = false },
                    new Slot { Time = "14:30", Available = true, Booked = false },
                    new Slot { Time = "15:00", Available = true, Booked = false }
                }
            };

            context.Users.Add(client);
            context.Providers.AddRange(provider1, provider2);
            context.SaveChanges(); // Para que tengan IDs asignados

            // Corregir ProviderId ahora que están guardados
            availability1.ProviderId = provider1.Id;
            availability2.ProviderId = provider2.Id;
            context.Availabilities.AddRange(availability1, availability2);
            context.SaveChanges();

            // Crear dos citas
            var appointment1 = new Appointment
            {
                ProviderId = provider1.Id,
                ClientId = client.Id,
                ProviderName = provider1.Name,
                ClientName = client.Name,
                ProviderProfession = provider1.Profession,
                Date = availability1.Date.ToString("yyyy-MM-dd"),
                Time = "10:00",
                EndTime = DateTime.Today.AddDays(1).AddHours(11),
                Location = "Av. Siempreviva 123",
                Notes = "Pérdida en el baño",
                AcceptedByProvider = true,
                Status = "accepted",
                Price = "1200",
                ServiceDescription = "Reparación de cañería",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var appointment2 = new Appointment
            {
                ProviderId = provider2.Id,
                ClientId = client.Id,
                ProviderName = provider2.Name,
                ClientName = client.Name,
                ProviderProfession = provider2.Profession,
                Date = availability2.Date.ToString("yyyy-MM-dd"),
                Time = "14:00",
                Location = "Calle Falsa 456",
                Notes = "Problemas con toma corriente",
                AcceptedByProvider = null,
                Status = "pending",
                Price = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Appointments.AddRange(appointment1, appointment2);
            context.SaveChanges();
        }
    }
}
