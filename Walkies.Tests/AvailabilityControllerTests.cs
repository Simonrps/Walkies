using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Walkies.API.Controllers;
using Walkies.API.Data;
using Walkies.API.DTOs;
using Walkies.API.Models;

namespace Walkies.Tests
{
    /// <summary>
    /// Unit tests for the availabilityController
    /// Tests walker availability management functionality using in-memory DB
    /// Relate to US12 - Walker Availability
    /// </summary>
    public class AvailabilityControllerTests
    {
        /// <summary>
        /// Creates an in-memory database context for each test.
        /// </summary>
        private static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        /// <summary>
        /// Creates an instance of the AvailabilityController for testing.
        /// </summary>
        private static AvailabilityController CreateController(ApplicationDbContext context)
        {
            return new AvailabilityController(context);
        }

        /// <summary>
        /// Verifies that setting a valid availability slot returns
        /// 201 created with the correct availability data
        /// Relates to US12 - Walker Availability
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetAvailability_ValidRequest_Returns201WithAvailability()
        {
            // Arrange
            using var context = CreateContext();
            var walker = new User
            {
                FirstName = "Simone",
                LastName = "Mulrooney",
                Email = "simone@email.com",
                PasswordHash = "PasswordHashed123!!#",
                Role = "Walker"
            };
            context.Users.Add(walker);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var controller = CreateController(context);
            var dto = new CreateAvailabilityDto
            {
                WalkerId = walker.Id,
                AvailableFrom = DateTime.UtcNow.AddDays(1).AddHours(9),
                AvailableTo = DateTime.UtcNow.AddDays(1).AddHours(17)
            };

            // Act
            var result = await controller.SetAvailability(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var availability = Assert.IsType<AvailabilityDto>(createdResult.Value);
            Assert.Equal(dto.WalkerId, availability.WalkerId);
            Assert.True(availability.IsAvailable);
            Assert.Equal("Simone Mulrooney", availability.WalkerName);
        }

        /// <summary>
        /// Verifies that retrieving availability slots for a valid walker
        /// Returns 200 OK with a list of availability slots
        /// Relates to US12 - Walker Availability
        /// </summary>
        [Fact]
        public async Task GetAvailability_ValidWalkerId_Returns200WithList()
        {
            // Arrange
            using var context = CreateContext();
            var walker = new User
            {
                FirstName = "Simone",
                LastName = "Mulrooney",
                Email = "simone@email.com",
                PasswordHash = "PasswordHashed123!!#",
                Role = "Walker"
            };
            context.Users.Add(walker);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            context.WalkerAvailabilities.Add(new WalkerAvailability
            {
                WalkerId = walker.Id,
                AvailableFrom = DateTime.UtcNow.AddDays(1).AddHours(9),
                AvailableTo = DateTime.UtcNow.AddDays(1).AddHours(17),
                IsAvailable = true
            });
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var controller = CreateController(context);

            // Act
            var result = await controller.GetAvailability(walker.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var slots = Assert.IsType<List<AvailabilityDto>>(okResult.Value);
            Assert.Single(slots);
            Assert.Equal(walker.Id, slots[0].WalkerId);
        }

        /// <summary>
        /// Verifies that deleting an availability slot with no
        /// associated bookings returns 204 No Content
        /// Related to US12 - Walker Availability
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeleteAvailability_NoBooking_Retrns204NoContent()
        {
            // Arrange
            using var context = CreateContext();
            var walker = new User
            {
                FirstName = "Simone",
                LastName = "Mulrooney",
                Email = "simone@email.com",
                PasswordHash = "PasswordHashed123!!#",
                Role = "Walker"
            };
            context.Users.Add(walker);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var slot = new WalkerAvailability
            {
                WalkerId = walker.Id,
                AvailableFrom = DateTime.UtcNow.AddDays(1).AddHours(9),
                AvailableTo = DateTime.UtcNow.AddDays(1).AddHours(17),
                IsAvailable = true
            };
            context.WalkerAvailabilities.Add(slot);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var controller = CreateController(context);

            // Act
            var result = await controller.DeleteAvailability(slot.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verify the slot is deleted
            var deleted = await context.WalkerAvailabilities
                .FirstOrDefaultAsync(a => a.Id == slot.Id,
                TestContext.Current.CancellationToken);
            Assert.Null(deleted);
        }
    }
}
