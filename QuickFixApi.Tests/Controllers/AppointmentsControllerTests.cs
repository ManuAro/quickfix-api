using Microsoft.AspNetCore.Mvc;
using QuickFixApi.Controllers;
using QuickFixApi.Models;
using QuickFixApi.Tests;
using Xunit;

namespace QuickFixApi.Tests
{
    public class AppointmentsControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsAllAppointments()
        {
            var context = TestUtils.GetDbContext();
            TestUtils.SeedAppointments(context);
            var controller = new AppointmentsController(context);

            var result = await controller.GetAll();
            var okResult = Assert.IsType<ActionResult<IEnumerable<Appointment>>>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<Appointment>>(okResult.Value);
            Assert.Equal(2, list.Count());
        }

        [Fact]
        public async Task GetById_ReturnsCorrectAppointment()
        {
            var context = TestUtils.GetDbContext();
            TestUtils.SeedAppointments(context);
            var controller = new AppointmentsController(context);

            var result = await controller.GetById(1);
            var okResult = Assert.IsType<ActionResult<Appointment>>(result);
            var appointment = Assert.IsType<Appointment>(okResult.Value);
            Assert.Equal(1, appointment.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenInvalidId()
        {
            var context = TestUtils.GetDbContext();
            TestUtils.SeedAppointments(context);
            var controller = new AppointmentsController(context);

            var result = await controller.GetById(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_AddsAppointmentSuccessfully()
        {
            var context = TestUtils.GetDbContext();
            var controller = new AppointmentsController(context);

            var newAppointment = new Appointment
            {
                ProviderId = 13,
                ClientId = 103,
                ProviderName = "Lucía Fernández",
                ClientName = "Carlos Silva",
                ProviderProfession = "Cerrajera",
                Date = DateTime.Today.AddDays(2),
                Time = "16:30",
                Status = "pending",
                Location = "Colonia",
                Notes = "Cambiar cerradura"
            };

            var result = await controller.Create(newAppointment);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var appointment = Assert.IsType<Appointment>(created.Value);
            Assert.Equal("Lucía Fernández", appointment.ProviderName);
        }

        [Fact]
        public async Task Update_ModifiesExistingAppointment()
        {
            var context = TestUtils.GetDbContext();
            TestUtils.SeedAppointments(context);
            var controller = new AppointmentsController(context);

            var updated = new Appointment
            {
                Id = 1,
                ProviderId = 10,
                ClientId = 100,
                ProviderName = "Juan Pérez",
                ClientName = "Pedro Gómez",
                ProviderProfession = "Plomero",
                Date = DateTime.Today,
                Time = "10:00",
                Status = "confirmed",
                Location = "Montevideo",
                Notes = "Llevar herramientas"
            };

            var result = await controller.Update(1, updated);

            Assert.IsType<NoContentResult>(result);
            var appointment = context.Appointments.Find(1);
            Assert.Equal("confirmed", appointment!.Status);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            var context = TestUtils.GetDbContext();
            var controller = new AppointmentsController(context);

            var updated = new Appointment
            {
                Id = 999,
                ProviderId = 10,
                ClientId = 100,
                ProviderName = "Alguien",
                ClientName = "Otro",
                ProviderProfession = "Otro",
                Date = DateTime.Now,
                Time = "10:00",
                Status = "pending",
                Location = "Cualquier lugar",
                Notes = ""
            };

            var result = await controller.Update(1, updated);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Delete_RemovesAppointment()
        {
            var context = TestUtils.GetDbContext();
            TestUtils.SeedAppointments(context);
            var controller = new AppointmentsController(context);

            var result = await controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(context.Appointments.Find(1));
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenInvalidId()
        {
            var context = TestUtils.GetDbContext();
            TestUtils.SeedAppointments(context);
            var controller = new AppointmentsController(context);

            var result = await controller.Delete(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
