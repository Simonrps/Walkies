using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Walkies.API.Controllers;
using Walkies.API.Data;
using Walkies.API.DTOs;
using Walkies.API.Models;

namespace Walkies.Tests
{
    /// <summary>
    /// Unit tests for the DogsController, tests dog management
    /// functionality using an in-memory database.
    /// Related to US04 - Add Dog and US05 - Edit Dog
    /// </summary>
    public class DogsControllerTests
    {
        /// <summary>
        /// Creates an in-memory database context for each test
        /// </summary>
        private static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        /// <summary>
        /// Creates a test instance of DogsController with an in-memory database
        /// </summary>
        private static DogsController CreateController(ApplicationDbContext context)
        {
            return new DogsController(context);
        }

        [Fact]
        public async Task AddDog_ValidRequest_Returns201WithDog()
        {
            // Arrange
            using var context = CreateContext();
            var owner = new User
            {
                FirstName = "Simon",
                LastName = "Mulroy",
                Email = "simon@email.com",
                PasswordHash = "hashedPassword!##123",
                Role = "Owner"
            };
            context.Users.Add(owner);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var controller = CreateController(context);
            var dto = new CreateDogDto
            {
                Name = "Dinah",
                Breed = "Boxer",
                Age = 5,
                OwnerId = owner.Id
            };

            // Act
            var result = await controller.AddDog(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var dog = Assert.IsType<DogDto>(createdResult.Value);
            Assert.Equal("Dinah", dog.Name);
            Assert.Equal("Boxer", dog.Breed);
            Assert.Equal(owner.Id, dog.OwnerId);
        }
    }
}
