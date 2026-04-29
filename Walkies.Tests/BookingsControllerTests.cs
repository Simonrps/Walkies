using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Walkies.API.Controllers;
using Walkies.API.Data;
using Walkies.API.DTOs;
using Walkies.API.Models;


namespace Walkies.Tests
{
    /// <summary>
    /// Unit tests for the BookingsController. Tests booking management
    /// functionality using an in-memory database.
    /// Related to US09 - Accept Walk Request, US10 - View Booking,
    /// US11 - View All Bookings, US12 - Check In, US13 - Check out
    /// </summary>
    public class BookingsControllerTests
    {

        /// <summary>
        /// Creates in memory database context for each test.
        /// </summary>
        private static ApplicationDbContext CreateContext()
        { 
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        /// <summary>
        /// Creates a test instance of BookingController with in memory DB.
        /// </summary>
        private static BookingsController CreateController(ApplicationDbContext context)
        {
            return new BookingsController(context);
        }

        /// <summary>
        /// Helper method to seed a standard test scenario witn an owner,
        /// walker, dog and open walk request.
        /// </summary>
        private static async Task<(User owner, User walker, Dog dog, WalkRequest walkRequest)>
            SeedTestDataAsync(ApplicationDbContext context)
        {
            var owner = new User
            {
                FirstName = "Simon",
                LastName = "Mulroy",
                Email = "simon@email.com",
                PasswordHash = "hashedPassword123!#",
                Role = "Owner"
            };
            var walker = new User
            {
                FirstName = "Simone",
                LastName = "Mulrooney",
                Email = "simone@email.com",
                PasswordHash = "HashedPassword123!#",
                Role = "Walker"
            };
            context.Users.AddRange(owner, walker);
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

            return (owner, walker, dog, walkRequest);
        }

        [Fact]
        public async Task CreateBooking_ValidRequest_Returns201WithBooking()
        {
            /// Arrange
            using var context = CreateContext();
            var (owner, walker, dog, walkRequest) = await SeedTestDataAsync(context);

            var controller = CreateController(context);
            var dto = new CreateBookingDto
            {
                WalkRequestId = walkRequest.Id,
                WalkerId = walker.Id
            };

            // Act
            var result = await controller.CreateBooking(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var booking = Assert.IsType<BookingDto>(createdResult.Value);
            Assert.Equal("Confirmed", booking.Status);
            Assert.Equal("Simone Mulrooney", booking.WalkerName);
            Assert.Equal("Dinah", booking.DogName);
        }
    }
}
