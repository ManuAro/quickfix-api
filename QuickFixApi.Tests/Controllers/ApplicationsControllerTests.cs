using Microsoft.AspNetCore.Mvc;
using QuickFixApi.Controllers;
using QuickFixApi.Models;
using QuickFixApi.Tests;
using Xunit;

namespace QuickFixApi.Tests
{
    public class ApplicationsControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsAllApplications()
        {
            var context = TestUtils.GetDbContext();
            TestUtils.SeedApplications(context);
            var controller = new ApplicationsController(context);

            var result = await controller.GetApplications();
            var okResult = Assert.IsType<ActionResult<IEnumerable<Application>>>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<Application>>(okResult.Value);
            Assert.Equal(2, list.Count());
        }

        [Fact]
        public async Task GetById_ReturnsCorrectApplication()
        {
            var context = TestUtils.GetDbContext();
            TestUtils.SeedApplications(context);
            var controller = new ApplicationsController(context);

            var result = await controller.GetApplication(1);
            var okResult = Assert.IsType<ActionResult<Application>>(result);
            var application = Assert.IsType<Application>(okResult.Value);
            Assert.Equal(1, application.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenInvalidId()
        {
            var context = TestUtils.GetDbContext();
            TestUtils.SeedApplications(context);
            var controller = new ApplicationsController(context);

            var result = await controller.GetApplication(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_AddsApplicationSuccessfully()
        {
            var context = TestUtils.GetDbContext();
            var controller = new ApplicationsController(context);

            var newApplication = new Application
            {
                Name = "Tania López",
                Email = "tania@example.com",
                Phone = "091234567",
                Profession = "Herrera",
                City = "San José",
                Experience = "5 años soldando estructuras metálicas",
                About = "Trabajo rápido y prolijo",
                HasCertifications = true,
                HasTools = true,
                AcceptTerms = true
            };

            var result = await controller.PostApplication(newApplication);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var application = Assert.IsType<Application>(created.Value);
            Assert.Equal("Tania López", application.Name);
        }
    }
}
