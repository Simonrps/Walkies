using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Walkies.API.Controllers;
using Walkies.API.Data;
using Walkies.API.DTOs;
using Walkies.API.Models;

namespace Walkies.Tests
{
    /// <summary>
    /// Unit tests for the messagesController. Tests in app messaging
    /// functionality using an in memory database. Related to
    /// US17 - Owner Messaging and US18 - Walker Messaging
    /// </summary>
    public class MessagesControllerTests
    {
        /// <summary>
        /// Creates a in memory databse context for tests
        /// </summary>
        private static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        /// <summary>
        /// Creates a test instance of messagesController with in memory db
        /// </summary>
        private static MessagesController CreateController(ApplicationDbContext context)
        {
            return new MessagesController(context);
        }

        /// <summary>
        /// Verifies that sending a valid message returns a 201 created
        /// with the correct message data. Related to US17 - Owner Messaging
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SendMessage_ValidRequest_Returns201WithMessage()
        {
            // Arrange
            using var context = CreateContext();
            var owner = new User
            {
                FirstName = "Simon",
                LastName = "Mulroy",
                Email = "simon@email.com",
                PasswordHash = "PasswordHash123!#",
                Role = "Owner"
            };
            var walker = new User
            {
                FirstName = "Simone",
                LastName = "Mulrooney",
                Email = "simone@email.com",
                PasswordHash = "PasswordHashed123!##",
                Role = "Walker"
            };
            context.Users.AddRange(owner, walker);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var controller = CreateController(context);
            var dto = new CreateMessageDto
            {
                SenderId = owner.Id,
                RecipientId = walker.Id,
                Content = "Hi Simone, can you walk Dinah tomorrow?"
            };

            // Act
            var result = await controller.SendMessage(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            var message = Assert.IsType<MessageDto>(createdResult.Value);
            Assert.Equal("Simon Mulroy", message.SenderName);
            Assert.Equal("Simone Mulrooney", message.RecipientName);
            Assert.Equal("Hi Simone, can you walk Dinah tomorrow?", message.Content);
        }

        /// <summary>
        /// Verifies that sending a message with empty content
        /// returns a 400 Bad Request response.
        /// Related to US17 - Owner Messaging
        /// </summary>
        [Fact]
        public async Task SendMessage_EmptyContent_Returns400BadRequest()
        {
            // Arrange
            using var context = CreateContext();
            var owner = new User
            {
                FirstName = "Simon",
                LastName = "Mulroy",
                Email = "simon@email.com",
                PasswordHash = "PasswordHash123!#",
                Role = "Owner"
            };
            var walker = new User
            {
                FirstName = "Simone",
                LastName = "Mulrooney",
                Email = "simone@email.com",
                PasswordHash = "PasswordHashed123!##",
                Role = "Walker"
            };
            context.Users.AddRange(owner, walker);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var controller = CreateController(context);
            var dto = new CreateMessageDto
            {
                SenderId = owner.Id,
                RecipientId = walker.Id,
                Content = string.Empty
            };

            // Simulate model validation failure
            controller.ModelState.AddModelError("Content", "Cannot send a blank message.");

            // Act
            var result = await controller.SendMessage(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        /// <summary>
        /// Verifies that the getting messages for a valid user
        /// returns 200 with a list of messages.
        /// Related to US17 - Owner Messaging and US18 - Walker Messaging
        /// </summary>
        [Fact]
        public async Task GetMessages_ValidUserId_Returns200WithList()
        {
            // Arrange
            using var context = CreateContext();
            var owner = new User
            {
                FirstName = "Simon",
                LastName = "Mulroy",
                Email = "simon@email.com",
                PasswordHash = "PasswordHash123!#",
                Role = "Owner"
            };
            var walker = new User
            {
                FirstName = "Simone",
                LastName = "Mulrooney",
                Email = "simone@email.com",
                PasswordHash = "PasswordHashed123!##",
                Role = "Walker"
            };
            context.Users.AddRange(owner, walker);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            context.Messages.Add(new Message
            {
                SenderId = owner.Id,
                Sender = owner,
                RecipientId = walker.Id,
                Recipient = walker,
                Content = "Hi Simone, can you walk Dinah tomorrow?",
                SentAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var controller = CreateController(context);

            // Act
            var result = await controller.GetMessages(owner.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var messages = Assert.IsType<List<MessageDto>>(okResult.Value);
            Assert.Single(messages);
            Assert.Equal("Simon Mulroy", messages[0].SenderName);
        }
    }
}