using Microsoft.AspNetCore.Mvc;
using QuickFixApi.Controllers;
using QuickFixApi.Models;
using QuickFixApi.Tests;
using Xunit;

namespace QuickFixApi.Tests
{
    public class AvailabilityControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsAllAvailabilities()
        {
            var context = TestUtils.GetDbContext();
            TestUtils.SeedAvailability(context);
            var controller = new AvailabilityController(context);

            var result = await controller.GetAll();
            var okResult = Assert.IsType<ActionResult<IEnumerable<Availability>>>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<Availability>>(okResult.Value);
            Assert.Equal(2, list.Count());
        }

        [Fact]
        public async Task GetByProvider_ReturnsCorrectAvailabilities()
        {
            var context = TestUtils.GetDbContext();
            TestUtils.SeedAvailability(context);
            var controller = new AvailabilityController(context);

            var result = await controller.GetByProvider(100);
            var okResult = Assert.IsType<ActionResult<IEnumerable<Availability>>>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<Availability>>(okResult.Value);
            Assert.Single(list);
            Assert.Equal(100, list.First().ProviderId);
        }

        [Fact]
        public async Task Create_AddsAvailabilitySuccessfully()
        {
            var context = TestUtils.GetDbContext();
            var controller = new AvailabilityController(context);

            var newAvailability = new Availability
            {
                ProviderId = 300,
                Date = DateTime.Today.AddDays(2),
                Slots = new List<Slot>
                {
                    new Slot { Time = "15:00", Available = true, Booked = false }
                }
            };

            var result = await controller.Create(newAvailability);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var availability = Assert.IsType<Availability>(created.Value);
            Assert.Equal(300, availability.ProviderId);
        }

        [Fact]
        public async Task Update_ModifiesAvailability()
        {
            var context = TestUtils.GetDbContext();
            TestUtils.SeedAvailability(context);
            var controller = new AvailabilityController(context);

            var updated = context.Availabilities.Find(1)!;
            updated.Slots[0].Available = false;

            var result = await controller.Update(1, updated);

            Assert.IsType<NoContentResult>(result);
            var modified = context.Availabilities.Find(1);
            Assert.False(modified!.Slots[0].Available);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            var context = TestUtils.GetDbContext();
            var controller = new AvailabilityController(context);

            var fake = new Availability
            {
                Id = 999,
                ProviderId = 500,
                Date = DateTime.Today,
                Slots = new List<Slot>()
            };

            var result = await controller.Update(1, fake);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Delete_RemovesAvailability()
        {
            var context = TestUtils.GetDbContext();
            TestUtils.SeedAvailability(context);
            var controller = new AvailabilityController(context);

            var result = await controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(context.Availabilities.Find(1));
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenInvalidId()
        {
            var context = TestUtils.GetDbContext();
            TestUtils.SeedAvailability(context);
            var controller = new AvailabilityController(context);

            var result = await controller.Delete(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
