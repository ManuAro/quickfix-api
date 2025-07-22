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
    public class ApplicationsControllerTests
    {
        private async Task<AppDbContext> GetDbContextWithDataAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
                .Options;

            var context = new AppDbContext(options);

            context.Applications.Add(new Application
            {
                Id = 1,
                Name = "Juan Pérez",
                Email = "juan@example.com",
                Phone = "099111222",
                Profession = "Electricista",
                City = "Montevideo"
            });

            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task GetApplications_ReturnsAll()
        {
            var context = await GetDbContextWithDataAsync();
            var controller = new ApplicationsController(context);
            var result = await controller.GetApplications();

            var apps = Assert.IsType<List<Application>>(result.Value);
            Assert.Single(apps);
        }

        [Fact]
        public async Task GetApplication_WithValidId_ReturnsApplication()
        {
            var context = await GetDbContextWithDataAsync();
            var controller = new ApplicationsController(context);
            var result = await controller.GetApplication(1);

            var okResult = Assert.IsType<ActionResult<Application>>(result);
            var app = Assert.IsType<Application>(okResult.Value);
            Assert.Equal(1, app.Id);
        }

        [Fact]
        public async Task GetApplication_WithInvalidId_ReturnsNotFound()
        {
            var context = await GetDbContextWithDataAsync();
            var controller = new ApplicationsController(context);
            var result = await controller.GetApplication(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostApplication_CreatesSuccessfully()
        {
            var context = await GetDbContextWithDataAsync();
            var controller = new ApplicationsController(context);

            var newApp = new Application
            {
                Name = "Ana Torres",
                Email = "ana@example.com",
                Phone = "098765432",
                Profession = "Plomera",
                City = "Canelones"
            };

            var result = await controller.PostApplication(newApp);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returned = Assert.IsType<Application>(created.Value);
            Assert.Equal("Ana Torres", returned.Name);
        }

        [Fact]
        public async Task PutApplication_WithValidId_UpdatesSuccessfully()
        {
            var context = await GetDbContextWithDataAsync();
            var controller = new ApplicationsController(context);

            var updated = new Application
            {
                Id = 1,
                Name = "Juan Actualizado",
                Email = "juan_nuevo@example.com",
                Phone = "099111222",
                Profession = "Electricista",
                City = "Montevideo"
            };

            var result = await controller.PutApplication(1, updated);

            Assert.IsType<NoContentResult>(result);
            var app = await context.Applications.FindAsync(1);
            Assert.Equal("juan_nuevo@example.com", app!.Email);
        }

        [Fact]
        public async Task PutApplication_WithMismatchedId_ReturnsBadRequest()
        {
            var context = await GetDbContextWithDataAsync();
            var controller = new ApplicationsController(context);

            var updated = new Application
            {
                Id = 2,
                Name = "Error"
            };

            var result = await controller.PutApplication(1, updated);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutApplication_WithNonExistentId_ReturnsNotFound()
        {
            var context = await GetDbContextWithDataAsync();
            var controller = new ApplicationsController(context);

            var updated = new Application
            {
                Id = 99,
                Name = "Inexistente"
            };

            var result = await controller.PutApplication(99, updated);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteApplication_WithValidId_RemovesApplication()
        {
            var context = await GetDbContextWithDataAsync();
            var controller = new ApplicationsController(context);

            var result = await controller.DeleteApplication(1);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(await context.Applications.FindAsync(1));
        }

        [Fact]
        public async Task DeleteApplication_WithInvalidId_ReturnsNotFound()
        {
            var context = await GetDbContextWithDataAsync();
            var controller = new ApplicationsController(context);

            var result = await controller.DeleteApplication(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
