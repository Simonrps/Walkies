using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Walkies.API.Controllers;
using Walkies.API.Data;
using Walkies.API.DTOs;
using Walkies.API.Models;

namespace Walkies.Tests
{
    /// <summary>
    /// Unit tests for the paymentsController. Tests simulated
    /// payment confirmation functionality using in memory database.
    /// Related to US19 - Payment Confirmation Owner and
    /// US20 - Payment Confirmation Walker.
    /// </summary>
    public class PaymentsControllerTests
    {
        /// <summary>
        /// Creates an in memory database context for testing
        /// </summary>
        private static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            return new ApplicationDbContext(options);
        }

        /// <summary>
        /// Creates a test instance of paymentsController with in memory database context
        /// </summary>
        private static PaymentsController CreateController(ApplicationDbContext context)
        {
            return new PaymentsController(context);
        }

        /// <summary>
        /// Helper method to seed a completed booking with payment record
        /// </summary>
        private static async Task<(User owner, User walker, WalkBooking booking)>
            SeedCompletedBookingAsync(ApplicationDbContext context)
        {
            var owner = new User
            {
                FirstName = "Simon",
                LastName = "Mulroy",
                Email = "simon@email.com",
                PasswordHash = "hashedPassword123!#",
                Role = "Owner",
            };
            var walker = new User
            {
                FirstName = "Simone",
                LastName = "Mulrooney",
                Email = "simone@email.com",
                PasswordHash = "hashedPassword123!#!",
                Role = "Walker",
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
                Status = "Completed"
            };
            context.WalkRequests.Add(walkRequest);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var booking = new WalkBooking
            {
                WalkRequestId = walkRequest.Id,
                WalkerId = walker.Id,
                Status = "Completed",
                CheckInTime = DateTime.UtcNow.AddDays(-1).AddHours(9),
                CheckOutTime = DateTime.UtcNow.AddDays(-1).AddHours(10),
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };
            context.WalkBookings.Add(booking);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            return (owner, walker, booking);
        }

        /// <summary>
        /// Verifies that retreiving a payment record for a valid booking
        /// returns 200 with the correct payment data.
        /// Related to US19 - Payment Confirmation Owner
        /// </summary>
        [Fact]
        public async Task GetPaymentRecordByBooking_ValidBookingId_Returns200WithPayment()
        {
            // Arrange
            using var context = CreateContext();
            var (_, _, booking) = await SeedCompletedBookingAsync(context);

            var payment = new PaymentRecord
            {
                WalkBookingId = booking.Id,
                Amount = 15.00m,
                Status = "Completed",
                CreatedAt = DateTime.UtcNow
            };
            context.PaymentRecords.Add(payment);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var controller = CreateController(context);

            // Act
            var result = await controller.GetPaymentByBooking(booking.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var paymentDto = Assert.IsType<PaymentRecordDto>(okResult.Value);
            Assert.Equal("Completed", paymentDto.Status);
            Assert.Equal(15.00m, paymentDto.Amount);
            Assert.Equal("Simon Mulroy", paymentDto.OwnerName);
            Assert.Equal("Simone Mulrooney", paymentDto.WalkerName);
        }

        /// <summary>
        /// Verifies that retrieving payment recrods for a valid walker
        /// returns 200 with a list of payment records.
        /// Related to US20 - Payment Confirmation Walker
        /// </summary>
        [Fact]
        public async Task GetPaymentByWalker_ValidWalkerId_Returns200WithList()
        {
            // Arrange
            using var context = CreateContext();
            var (_, walker, booking) = await SeedCompletedBookingAsync(context);

            var payment = new PaymentRecord
            {
                WalkBookingId = booking.Id,
                Amount = 15.00m,
                Status = "Confirmed",
                CreatedAt = DateTime.UtcNow
            };
            context.PaymentRecords.Add(payment);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var controller = CreateController(context);

            // Act
            var result = await controller.GetPaymentsByWalker(walker.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var payments = Assert.IsType<List<PaymentRecordDto>>(okResult.Value);
            Assert.Single(payments);
            Assert.Equal("Confirmed", payments[0].Status);
            Assert.Equal("Simone Mulrooney", payments[0].WalkerName);
        }
    }
}