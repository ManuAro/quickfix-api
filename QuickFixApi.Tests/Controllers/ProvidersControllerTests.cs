using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickFixApi.Controllers;
using QuickFixApi.Data;
using QuickFixApi.Models;
using Xunit;

namespace QuickFixApi.Tests.Controllers
{
    public class ProvidersControllerTests
    {
        private async Task<AppDbContext> GetDbContextWithDataAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestProvidersDb_" + System.Guid.NewGuid())
                .Options;

            var context = new AppDbContext(options);

            context.Providers.Add(new Provider
            {
                Id = 1,
                Name = "Laura Rodríguez",
                Profession = "Plomera",
                Rating = 4.5,
                Reviews = 10,
                Location = "Montevideo",
                Price = "1200",
                Image = "img.jpg",
                Description = "Especialista en baños",
                Services = new[] { "cañerías", "sanitarios" },
                Phone = "099123456",
                Email = "laura@example.com",
                Availability = "Lun a Vie",
                Certifications = "Certificado A",
                Coordinates = "-34.9011,-56.1645"
            });

            await context.SaveChangesAsync();
            return context;
        }

        [Fact]
        public async Task GetProviders_ReturnsAll()
        {
            var context = await GetDbContextWithDataAsync();
            var controller = new ProvidersController(context);

            var result = await controller.GetProviders();

            var providers = Assert.IsType<List<Provider>>(result.Value);
            Assert.Single(providers);
        }

        [Fact]
        public async Task GetProvider_WithValidId_ReturnsProvider()
        {
            var context = await GetDbContextWithDataAsync();
            var controller = new ProvidersController(context);

            var result = await controller.GetProvider(1);

            var provider = Assert.IsType<Provider>(result.Value);
            Assert.Equal("Laura Rodríguez", provider.Name);
        }

        [Fact]
        public async Task GetProvider_WithInvalidId_ReturnsNotFound()
        {
            var context = await GetDbContextWithDataAsync();
            var controller = new ProvidersController(context);

            var result = await controller.GetProvider(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Post_CreatesSuccessfully()
        {
            var context = await GetDbContextWithDataAsync();
            var controller = new ProvidersController(context);

            var newProvider = new Provider
            {
                Name = "Carlos Techera",
                Profession = "Electricista",
                Rating = 5.0,
                Reviews = 20,
                Location = "Canelones",
                Price = "1500",
                Image = "carlos.jpg",
                Description = "Instalaciones eléctricas",
                Services = new[] { "cableado", "enchufes" },
                Phone = "098654321",
                Email = "carlos@example.com",
                Availability = "Lun a Sab",
                Certifications = "Certificado B",
                Coordinates = "-34.8,-56.1"
            };

            var result = await controller.Post(newProvider);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returned = Assert.IsType<Provider>(created.Value);
            Assert.Equal("Carlos Techera", returned.Name);
        }

        [Fact]
        public async Task Put_WithValidId_UpdatesSuccessfully()
        {
            var context = await GetDbContextWithDataAsync();
            var controller = new ProvidersController(context);

            var updated = new Provider
            {
                Id = 1,
                Name = "Laura Actualizada",
                Profession = "Gasista",
                Rating = 4.8,
                Reviews = 12,
                Location = "Montevideo",
                Price = "1300",
                Image = "laura_nueva.jpg",
                Description = "Servicios generales",
                Services = new[] { "gas", "agua" },
                Phone = "099000000",
                Email = "laura@actualizada.com",
                Availability = "Lun a Dom",
                Certifications = "Certificado C",
                Coordinates = "-34.9,-56.2"
            };

            var result = await controller.Put(1, updated);

            Assert.IsType<NoContentResult>(result);
            var provider = await context.Providers.FindAsync(1);
            Assert.NotNull(provider);
            Assert.Equal("Laura Actualizada", provider!.Name);
        }

        [Fact]
        public async Task Put_WithMismatchedId_ReturnsBadRequest()
        {
            var context = await GetDbContextWithDataAsync();
            var controller = new ProvidersController(context);

            var updated = new Provider { Id = 2, Name = "Error" };

            var result = await controller.Put(1, updated);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Put_WithNonExistentId_ReturnsNotFound()
        {
            var context = await GetDbContextWithDataAsync();
            var controller = new ProvidersController(context);

            var updated = new Provider
            {
                Id = 99,
                Name = "Desconocido"
            };

            var result = await controller.Put(99, updated);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_WithValidId_DeletesSuccessfully()
        {
            var context = await GetDbContextWithDataAsync();
            var controller = new ProvidersController(context);

            var result = await controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(await context.Providers.FindAsync(1));
        }

        [Fact]
        public async Task Delete_WithInvalidId_ReturnsNotFound()
        {
            var context = await GetDbContextWithDataAsync();
            var controller = new ProvidersController(context);

            var result = await controller.Delete(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
