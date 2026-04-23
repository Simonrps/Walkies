using Microsoft.EntityFrameworkCore;
using Walkies.API.Data;
using Walkies.API.Models;

namespace Walkies.Tests
{

    /// <summary>
    /// Integration tests for the ApplicationDbContext.
    /// Verifies that entity relationships and constraints
    /// are correctly configured usingh the EF Core In-Memory provider
    /// </summary>
    public class ApplicationDbContextTests
    {
        /// <summary>
        /// Creates a fresh in-memory database context for each test
        /// </summary>
        private static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each test
                .Options;
            return new ApplicationDbContext(options);
        }

        /// <summary>
        /// Verifies that a User can be added and retrieved from the database
        /// Relates to US01 - Registration
        /// </summary>
        [Fact]
        public async Task AddUser_ValidUser_UserIsSavedToDatabase()
        {
            // Arrange
            using var context = CreateContext();
            var user = new User
            {
                FirstName = "Simon",
                LastName = "Mulroy",
                Email = "simon@email.com",
                PasswordHash = "hashedpassword",
                Role = "Owner"
            };

            // Act
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Assert
            var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "simon@email.com");
            Assert.NotNull(savedUser);
            Assert.Equal("Simon", savedUser.FirstName);
            Assert.Equal("Owner", savedUser.Role);
        }

        /// <summary>
        /// Verifies that a dog can be added to an owner and retrieved.
        /// Relates to US04 - Add Dog
        /// </summary>
        [Fact]
        public async Task AddDog_ValidDogWithOwner_DogIsSavedWithCorrectOwner()
        {
            // Arrange
            using var context = CreateContext();
            var owner = new User
            {
                FirstName = "Simon",
                LastName = "Mulroy",
                Email = "simon@email.com",
                PasswordHash = "hashedpassword",
                Role = "Owner"
            };
            context.Users.Add(owner);
            await context.SaveChangesAsync();

            var dog = new Dog
            {
                Name = "Dinah",
                Breed = "Boxer",
                Age = 5,
                OwnerId = owner.Id
            };

            // Act
            context.Dogs.Add(dog);
            await context.SaveChangesAsync();

            // Assert
            var savedDog = await context.Dogs
                .Include(d => d.Owner)
                .FirstOrDefaultAsync(d => d.Name == "Dinah");
            Assert.NotNull(savedDog);
            Assert.Equal("Dinah", savedDog.Name);
            Assert.Equal("simon@email.com", savedDog.Owner.Email);
        }

        /// <summary>
        /// Verifies that a WalkRequest can be created and associated with an Owner and a dog.
        /// Relates to US06 - Post Walk Request
        /// </summary>
        [Fact]
        public async Task AddWalkRequest_ValidRequest_RequestIsSavedWithCorrrectStatus()
        {
            // Arrange
            using var context = CreateContext();
            var owner = new User
            {
                FirstName = "Simon",
                LastName = "Mulroy",
                Email = "simon@email.com",
                PasswordHash = "hashedpassword",
                Role = "Owner"
            };
            context.Users.Add(owner);
            await context.SaveChangesAsync();

            var dog = new Dog
            {
                Name = "Dinah",
                Breed = "Boxer",
                Age = 5,
                OwnerId = owner.Id
            };
            context.Dogs.Add(dog);
            await context.SaveChangesAsync();

            var walkRequest = new WalkRequest
            {
                OwnerId = owner.Id,
                DogId = dog.Id,
                RequestedDate = DateTime.UtcNow.AddDays(1),
                DurationMinutes = 30,
                Latitude = 54.9966,
                Longitude = -7.3086,
                Location = "LetterKenny, Co. Donegal",
                Status = "Open"
            };

            // Act
            context.WalkRequests.Add(walkRequest);
            await context.SaveChangesAsync();

            // Assert
            var savedRequest = await context.WalkRequests
                .Include(wr => wr.Owner)
                .Include(wr => wr.Dog)
                .FirstOrDefaultAsync(wr => wr.OwnerId == owner.Id);
            Assert.NotNull(savedRequest);
            Assert.Equal("Open", savedRequest.Status);
            Assert.Equal("Dinah", savedRequest.Dog.Name);
        }

        /// <summary>
        /// Verifies that a WalkBooking is created with Accepted
        /// status when a walker accepts a walk request.
        /// Related to US09 - Accept or Decline Walk Request
        /// </summary>
        [Fact]
        public async Task AddWalkBooking_ValidBooking_BookingIsSavedWithAcceptedStatus()
        {
            // Arrange
            using var context = CreateContext();
            var owner = new User
            {
                FirstName = "Simon",
                LastName = "Mulroy",
                Email = "simon@email.com",
                PasswordHash = "hashedpassword",
                Role = "Owner"
            };
            var walker = new User
            {
                FirstName = "Doggy",
                LastName = "Walker",
                Email = "walker@email.com",
                PasswordHash = "hashedpassword",
                Role = "Walker"
            };
            context.Users.AddRange(owner, walker);
            await context.SaveChangesAsync();

            var dog = new Dog
            {
                Name = "Dinah",
                Breed = "Boxer",
                Age = 5,
                OwnerId = owner.Id
            };
            context.Dogs.Add(dog);
            await context.SaveChangesAsync();

            var walkRequest = new WalkRequest
            {
                OwnerId = owner.Id,
                DogId = dog.Id,
                RequestedDate = DateTime.UtcNow.AddDays(1),
                DurationMinutes = 30,
                Latitude = 54.9966,
                Longitude = -7.3086,
                Location = "LetterKenny, Co. Donegal",
                Status = "Accepted"
            };
            context.WalkRequests.Add(walkRequest);
            await context.SaveChangesAsync();

            var booking = new WalkBooking
            {
                WalkRequestId = walkRequest.Id,
                WalkerId = walker.Id,
                Status = "Accepted"
            };

            // Act
            context.WalkBookings.Add(booking);
            await context.SaveChangesAsync();

            // Assert
            var savedBooking = await context.WalkBookings
                .Include(wb => wb.Walker)
                .Include(wb => wb.WalkRequest)
                .FirstOrDefaultAsync(wb => wb.WalkerId == walker.Id);
            Assert.NotNull(savedBooking);
            Assert.Equal("Accepted", savedBooking.Status);
            Assert.Equal("walker@email.com", savedBooking.Walker.Email);
        }

        /// <summary>
        /// Verifies that a message can be sent between a walker and an owner.
        /// Relates to US17 - Owner Messaging.
        /// </summary>
        [Fact]
        public async Task AddMessage_ValidMessage_MessageIsSavedWithCorrectSenderAndRecipient()
        {
            // Arrange
            using var context = CreateContext();
            var owner = new User
            {
                FirstName = "Simon",
                LastName = "Mulroy",
                Email = "simon@email.com",
                PasswordHash = "hashedpassword",
                Role = "Owner"
            };
            var walker = new User
            {
                FirstName = "Doggy",
                LastName = "Walker",
                Email = "walker@email.com",
                PasswordHash = "hashedpassword",
                Role = "Walker"
            };
            context.Users.AddRange(owner, walker);
            await context.SaveChangesAsync();

            var message = new Message
            {
                SenderId = owner.Id,
                RecipientId = walker.Id,
                Content = "Hi, are you availble tomorrow?",
                IsRead = false,
                Sender = owner,
                Recipient = walker
            };

            // Act
            context.Messages.Add(message);
            await context.SaveChangesAsync();

            // Assert
            var savedMessage = await context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
                .FirstOrDefaultAsync(m => m.SenderId == owner.Id);
            Assert.NotNull(savedMessage);
            Assert.Equal("Hi, are you availble tomorrow?", savedMessage.Content);
            Assert.Equal("simon@email.com", savedMessage.Sender.Email);
            Assert.Equal("walker@email.com", savedMessage.Recipient.Email);
        }
    }
}