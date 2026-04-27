using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Walkies.API.Controllers;
using Walkies.API.Data;
using Walkies.API.DTOs;
using Walkies.API.Models;

namespace Walkies.Tests
{
    /// <summary>
    /// Unit tests for the WalkRequestsController class,
    /// Tests walk request functionality using an in memory
    /// database. Relates to US06 - Post Walk Request and
    /// US07 - Browse Walk Requests and US08 - Cancel Walk Request
    /// </summary>
    public class WalkRequestsControllerTests
    {
        /// <summary>
        /// Creates a new in memory databse context for testing.
        /// </summary>
        private static ApplicationDbContext CreateContext()
        { 
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        /// <summary>
        /// creates a test instance of WalkRequestController
        /// with in memory database.
        /// </summary>
        private static WalkRequestsController CreateController(ApplicationDbContext context)
        {
            return new WalkRequestsController(context);
        }

        [Fact]
        public async Task PostWalkRequest_ValidRequest_Returns201WithWalkRequest()
        {
            // Arrange
            using var context = CreateContext();
            var owner = new User
            {
                FirstName = "Simon",
                LastName = "Mulroy",
                Email = "simon@email.com",
                PasswordHash = "hashedPassword123!##",
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
            var dto = new CreateWalkRequestDto
            {
                OwnerId = owner.Id,
                DogId = dog.Id,
                RequestedDate = DateTime.UtcNow.AddDays(1),
                DurationMinutes = 30,
                Location = "Letterkenny, Co. Donegal",
                Latitude = 54.9966,
                Longitude = -7.3086
            };

            // Act
            var result = await controller.PostWalkRequest(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var walkRequest = Assert.IsType<WalkRequestDto>(createdResult.Value);
            Assert.Equal("Open", walkRequest.Status);
            Assert.Equal("Dinah", walkRequest.DogName);
            Assert.Equal(owner.Id, walkRequest.OwnerId);
        }

        [Fact]
        public async Task GetWalkRequest_ValidId_Return200WithWalkRequest()
        {
            // Arrange
            using var context = CreateContext();
            var owner = new User
            {
                FirstName = "Simon",
                LastName = "Mulroy",
                Email = "simon@email.com",
                PasswordHash = "hashedPassword123!##",
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

            var walkRequest = new WalkRequest
            {
                OwnerId = owner.Id,
                DogId = dog.Id,
                RequestedDate = DateTime.UtcNow.AddDays(1),
                DurationMinutes = 30,
                Location = "Letterkenny, Co. Donegal",
                Latitude = 54.9966,
                Longitude = -7.3086,
                Status = "Open"
            };
            context.WalkRequests.Add(walkRequest);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var controller = CreateController(context);

            // Act
            var result = await controller.GetWalkRequest(walkRequest.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<WalkRequestDto>(okResult.Value);
            Assert.Equal("Open", dto.Status);
            Assert.Equal("Dinah", dto.DogName);
            Assert.Equal(owner.Id, dto.OwnerId);
        }
    }
}
