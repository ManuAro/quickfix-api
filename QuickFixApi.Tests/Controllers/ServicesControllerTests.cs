using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickFixApi.Controllers;
using QuickFixApi.Data;
using QuickFixApi.Models;
using Xunit;

namespace QuickFixApi.Tests.Controllers
{
    public class ServicesControllerTests
    {
        [Fact]
        public void GetAll_ReturnsAllServices()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("ServicesDb_GetAll")
                .Options;

            using var context = new AppDbContext(options);
            context.Services.AddRange(
                new Service
                {
                    Name = "Plomería básica",
                    Description = "Reparación de grifos y cañerías",
                    WorkerId = 1,
                    Category = "Plomería"
                },
                new Service
                {
                    Name = "Cerrajería",
                    Description = "Cambio de cerraduras",
                    WorkerId = 2,
                    Category = "Cerrajería"
                }
            );
            context.SaveChanges();

            var controller = new ServicesController(context);

            var result = controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var services = Assert.IsAssignableFrom<IEnumerable<Service>>(okResult.Value);

            Assert.Equal(2, services.Count());
        }

        [Fact]
        public void Create_AddsServiceSuccessfully()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("ServicesDb_Create")
                .Options;

            using var context = new AppDbContext(options);
            var controller = new ServicesController(context);

            var service = new Service
            {
                Name = "Electricidad",
                Description = "Instalaciones eléctricas",
                WorkerId = 3,
                Category = "Electricidad"
            };

            var result = controller.Create(service);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var createdService = Assert.IsType<Service>(createdResult.Value);

            Assert.Equal("Electricidad", createdService.Name);
            Assert.Single(context.Services);
        }

        [Fact]
        public void Update_ExistingService_UpdatesCorrectly()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("ServicesDb_Update")
                .Options;

            using var context = new AppDbContext(options);
            var existingService = new Service
            {
                Id = 1,
                Name = "Original",
                Description = "Old desc",
                WorkerId = 4,
                Category = "General"
            };
            context.Services.Add(existingService);
            context.SaveChanges();

            var controller = new ServicesController(context);

            var updatedService = new Service
            {
                Id = 1,
                Name = "Actualizado",
                Description = "Nueva descripción",
                WorkerId = 4,
                Category = "Reformas"
            };

            var result = controller.Update(1, updatedService);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var service = Assert.IsType<Service>(okResult.Value);

            Assert.Equal("Actualizado", service.Name);
            Assert.Equal("Reformas", service.Category);
        }

        [Fact]
        public void Delete_RemovesServiceSuccessfully()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("ServicesDb_Delete")
                .Options;

            using var context = new AppDbContext(options);
            var service = new Service
            {
                Id = 1,
                Name = "Eliminarme",
                Description = "Descripción",
                WorkerId = 5,
                Category = "Test"
            };
            context.Services.Add(service);
            context.SaveChanges();

            var controller = new ServicesController(context);

            var result = controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Services);
        }
    }
}
