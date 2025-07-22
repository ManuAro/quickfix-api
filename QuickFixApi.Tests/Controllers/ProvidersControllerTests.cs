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
            var controller = new ProvidersController
