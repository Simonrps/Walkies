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

        /// <summary>
        /// Verifies that a valid user profile request returns a 200 OK
        /// with the correct profile data. Related to US03 - Profile Management
        /// </summary>
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

        /// <summary>
        /// Verifies that requesting a profile that doesnt exist returns
        /// a 404 Not Found response. Related to US03 - Profile Management
        /// </summary>
        [Fact]
        public async Task GetUser_InvalidId_Returns404NotFound()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);
            
            // Act
            var result = await controller.GetUser(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        /// <summary>
        /// Verifies that a valid profile update returns 200 with
        /// updated profile. Related to US03 - Profile Management
        /// </summary>
        [Fact]
        public async Task UpdateUser_ValidRequest_Returns200()
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
            var dto = new UpdateUserDto
            {
                FirstName = "Simon",
                LastName = "Mulroy",
                Phone = "0831092468",
                Address = "1 Main Street, Letterkenny",
                Latitude = 54.9966,
                Longitude = -7.3086
            };

            // Act
            var result = await controller.UpdateUser(user.Id, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var profile = Assert.IsType<UserProfileDto>(okResult.Value);
            Assert.Equal("0831092468", profile.Phone);
            Assert.Equal("1 Main Street, Letterkenny", profile.Address);
        }

        /// <summary>
        /// Verifies that attempting to update a profile that doesnt exist returns
        /// a 404 Not Found response. Related to US03 - Profile Management
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UpdateUser_InvalidId_Returns404NotFound()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);
            var dto = new UpdateUserDto
            {
                FirstName = "Simon",
                LastName = "Mulroy",
                Phone = "0831092468",
                Address = "1 Main Street, Letterkenny",
                Latitude = 54.9966,
                Longitude = -7.3086
            };
            // Act
            var result = await controller.UpdateUser(999, dto);
            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
