using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Walkies.API.Controllers;
using Walkies.API.Data;
using Walkies.API.DTOs;

namespace Walkies.Tests
{
    /// <summary>
    /// Unit tests for AuthController. Tests registration and
    /// login functionality using an in-memory database  and 
    /// test JWT configuration.
    /// Relates to US01 - Registration and US02 - Login
    /// </summary>
    internal class AuthControllerTests
    {
        /// <summary>
        /// creates a freash in-memory database context for each test.
        /// </summary>
        private static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        /// <summary>
        /// Creates an instance of AuthController with in memory database
        /// and test JWT configuration.
        /// </summary>
        private static AuthController CreateController(ApplicationDbContext context)
        {
            var configuration = new Microsoft.Extensions.Configuration
                .ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "JwtSettings:SecretKey", "ThisIsATestSecretKey123!#"},
                    { "JwtSettings:Issuer", "WalkiesAPI"},
                    { "JwtSettings:Audience", "WalkiesClient"},
                    { "JwtSettings:ExpirationMinutes", "60"}
                })
                .Build();
            return new AuthController(context, configuration);
        }

        /// <summary>
        /// Verifies that a valid registration request creates a user
        /// and returns a 200 response with JWT token. Related to US01 - Registration
        /// </summary>
        [Fact]
        public async Task Register_ValidRequest_Returns200WithToken()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);
            var dto = new RegisterDto
            {
                FirstName = "Simon",
                LastName = "Mulroy",
                Email = "simon@email.com",
                Password = "Password123!#",
                Role = "Owner"
            };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.NotNull(response.Token);
            Assert.Equal("Owner", response.Role);
            Assert.Equal("Simon", response.FirstName);
        }
    }
}