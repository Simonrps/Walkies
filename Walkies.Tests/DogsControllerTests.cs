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

        /// <summary>
        /// Verifies that adding a dog with valid data returns
        /// a 201 Created response. Related to US04 - Add Dog
        /// </summary>
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

        [Fact]
        public async Task GetDog_ValidId_Returns200WithDog()
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

            var dog = new Dog
            {
                Name = "Dinah",
                Breed = "Boxer",
                Age = 5,
                OwnerId = owner.Id
            };
            context.Dogs.Add(dog);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var controller = CreateController(context);

            // Act
            var result = await controller.GetDog(dog.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dogDto = Assert.IsType<DogDto>(okResult.Value);
            Assert.Equal("Dinah", dogDto.Name);
            Assert.Equal("Boxer", dogDto.Breed);
            Assert.Equal(owner.Id, dogDto.OwnerId);
        }

        /// <summary>
        /// Verifies that requesting a dog that doesnt exist returns
        /// a 404 Not Found response. Related to US04 - Add Dog
        /// </summary>
        [Fact]
        public async Task GetDog_InvalidId_Returns404NotFound()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            // Act
            var result = await controller.GetDog(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateDog_ValidRequest_Returns200()
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

            var dog = new Dog
            {
                Name = "Dinah",
                Breed = "Boxer",
                Age = 5,
                OwnerId = owner.Id
            };
            context.Dogs.Add(dog);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var controller = CreateController(context);
            var dto = new UpdateDogDto
            {
                Name = "Dinah",
                Breed = "Boxer",
                Age = 6,
                Notes = "Updated notes"
            };

            // Act
            var result = await controller.UpdateDog(dog.Id, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dogDto = Assert.IsType<DogDto>(okResult.Value);
            Assert.Equal(6, dogDto.Age);
            Assert.Equal("Updated notes", dogDto.Notes);
        }

        [Fact]
        public async Task DeleteDog_ValidId_Returns204NoCentent()
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

            var dog = new Dog
            {
                Name = "Dinah",
                Breed = "Boxer",
                Age = 5,
                OwnerId = owner.Id
            };
            context.Dogs.Add(dog);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var controller = CreateController(context);

            // Act
            var result = await controller.DeleteDog(dog.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verify the dog was deleted
            var deletedDog = await context.Dogs.FirstOrDefaultAsync(d => d.Id == dog.Id);
            Assert.Null(deletedDog);
        }
    }
}
