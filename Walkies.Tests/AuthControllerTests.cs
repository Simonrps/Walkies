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
    public class AuthControllerTests
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
                    { "JwtSettings:SecretKey", "ThisIsATestSecretKeyForWalkiesApp123!#"},
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

        /// <summary>
        /// Verifies that registering with a duplicate email returns
        /// a 409 error response. Related to US01 - Registration
        /// </summary>
        [Fact]
        public async Task Register_DuplicateEmail_Returns409Error()
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
            await controller.Register(dto);
            var result = await controller.Register(dto);

            // Assert
            Assert.IsType<ConflictObjectResult>(result);
        }

        /// <summary>
        /// Verifies that a valid login request returns a 200 response with JWT token.
        /// Related to US02 - Login
        /// </summary>
        [Fact]
        public async Task Login_ValidRequest_Returns200WithToken()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);
            var registerDto = new RegisterDto
            {
                FirstName = "Simon",
                LastName = "Mulroy",
                Email = "simon@email.com",
                Password = "Password123!#",
                Role = "Owner"
            };
            await controller.Register(registerDto);

            var loginDto = new LoginDto
            {
                Email = "simon@email.com",
                Password = "Password123!#"
            };

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.NotNull(response.Token);
            Assert.Equal("Owner", response.Role);
        }

        /// <summary>
        /// Verifies that a login request with incorrect details 
        /// returns a 401 unauthorized response. Related to US02 - Login
        /// </summary>
        [Fact]
        public async Task Login_InvalidRequest_Returns401Unauthorized()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);
            var registerDto = new RegisterDto
            {
                FirstName = "Simon",
                LastName = "Mulroy",
                Email = "simon@email.com",
                Password = "Password123!#",
                Role = "Owner"
            };
            await controller.Register(registerDto);

            var loginDto = new LoginDto
            {
                Email = "simon@email.com",
                Password = "WrongPassword!#"
            };

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}