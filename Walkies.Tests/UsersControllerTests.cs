using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Walkies.API.Controllers;
using Walkies.API.Data;
using Walkies.API.DTOs;
using Walkies.API.Models;

namespace Walkies.Tests
{
    /// <summary>
    /// Unit tests for the UsersController. Tests profile retrieval
    /// and profile update functionality. Uses an in-memory database.
    /// Related to US03 - Profile Management
    /// </summary>
    public class UsersControllerTests
    {
        /// <summary>
        /// Creates a new in-memory database context for testing.
        /// </summary>
        private static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        /// <summary>
        /// Creates a test instance of UsersController with in-memory database.
        /// </summary>
        private static UsersController CreateController(ApplicationDbContext context)
        {
            return new UsersController(context);
        }

        [Fact]
        public async Task GetUser_ValidId_Returns200WithProfile()
        {
            // Arrange
            using var context = CreateContext();
            var user = new User
            {
                FirstName = "Simon",
                LastName = "Mulroy",
                Email = "simon@email.com",
                PasswordHash = "hashedpassword123!#",
                Role = "Owner"
            };
            context.Users.Add(user);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var controller = CreateController(context);

            // Act
            var result = await controller.GetUser(user.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var profile = Assert.IsType<UserProfileDto>(okResult.Value);
            Assert.Equal("Simon", profile.FirstName);
            Assert.Equal("simon@email.com", profile.Email);
            Assert.Equal("Owner", profile.Role);
        }
    }
}
