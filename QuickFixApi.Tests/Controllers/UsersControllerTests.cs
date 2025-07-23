using Microsoft.AspNetCore.Mvc;
using QuickFixApi.Controllers;
using QuickFixApi.Models;
using QuickFixApi.Tests;
using Xunit;

namespace QuickFixApi.Tests
{
    public class UsersControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsAllUsers()
        {
            var context = TestUtils.GetDbContext();
            TestUtils.SeedUsers(context);
            var controller = new UsersController(context);

            var result = await controller.GetAll();
            var okResult = Assert.IsType<ActionResult<IEnumerable<User>>>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
            Assert.Equal(2, list.Count());
        }

        [Fact]
        public async Task GetById_ReturnsCorrectUser()
        {
            var context = TestUtils.GetDbContext();
            TestUtils.SeedUsers(context);
            var controller = new UsersController(context);

            var result = await controller.GetById(1);
            var okResult = Assert.IsType<ActionResult<User>>(result);
            var user = Assert.IsType<User>(okResult.Value);
            Assert.Equal("Juan Pérez", user.Name);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenInvalidId()
        {
            var context = TestUtils.GetDbContext();
            TestUtils.SeedUsers(context);
            var controller = new UsersController(context);

            var result = await controller.GetById(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_AddsUserSuccessfully()
        {
            var context = TestUtils.GetDbContext();
            var controller = new UsersController(context);

            var newUser = new User
            {
                Name = "Lucía Fernández",
                Email = "lucia@example.com",
                Password = "securepass",
                UserType = "client",
                Profession = "Cerrajera"
            };

            var result = await controller.Create(newUser);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var user = Assert.IsType<User>(created.Value);
            Assert.Equal("Lucía Fernández", user.Name);
        }

        [Fact]
        public async Task Update_ModifiesExistingUser()
        {
            var context = TestUtils.GetDbContext();
            TestUtils.SeedUsers(context);
            var controller = new UsersController(context);

            // ✅ Usamos la misma instancia ya trackeada
            var updatedUser = context.Users.Find(1)!;
            updatedUser.Name = "Juan Pérez Modificado";
            updatedUser.Profession = "Gasista";

            var result = await controller.Update(updatedUser.Id, updatedUser);

            Assert.IsType<NoContentResult>(result);
            var user = context.Users.Find(1);
            Assert.Equal("Juan Pérez Modificado", user!.Name);
            Assert.Equal("Gasista", user.Profession);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            var context = TestUtils.GetDbContext();
            var controller = new UsersController(context);

            var updatedUser = new User
            {
                Id = 999,
                Name = "Otro",
                Email = "otro@example.com",
                Password = "pass",
                UserType = "provider",
                Profession = "Otro"
            };

            var result = await controller.Update(1, updatedUser);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Delete_RemovesUser()
        {
            var context = TestUtils.GetDbContext();
            TestUtils.SeedUsers(context);
            var controller = new UsersController(context);

            var result = await controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(context.Users.Find(1));
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenInvalidId()
        {
            var context = TestUtils.GetDbContext();
            TestUtils.SeedUsers(context);
            var controller = new UsersController(context);

            var result = await controller.Delete(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
