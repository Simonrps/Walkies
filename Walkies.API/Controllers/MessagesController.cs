using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Walkies.API.Data;
using Walkies.API.DTOs;
using Walkies.API.Models;

namespace Walkies.API.Controllers
{
    /// <summary>
    /// Handles in app messaging for the API. Provides endpoints
    /// for sending and retrieving messages between users.
    /// Related to US17 - Owner Messaging and US18 - Walker Messaging
    /// </summary>
    /// <remarks>
    /// Initialises a new instance of the MessagesController.
    /// </remarks>
    /// <param name="context">The database context</param>
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        /// <summary>
        /// Sends a new message from one user to another.
        /// Related to US17 - Owner Messaging and US18 - Walker Messaging
        /// </summary>
        /// <param name="dto">The message details</param>
        /// <returns>
        /// 201 Created with message data on success
        /// 400 Bad Request if the message content is empty
        /// 404 Not Found if either the sender or recipient does not exist
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] CreateMessageDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sender = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.SenderId);
            if (sender == null)
            {
                return NotFound("Sender not found");
            }

            var recipient = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.RecipientId);
            if (recipient == null)
            {
                return NotFound("Recipient not found");
            }

            var message = new Message
            {
                SenderId = dto.SenderId,
                Sender = sender,
                RecipientId = dto.RecipientId,
                Recipient = recipient,
                Content = dto.Content,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var messageDto = new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderName = $"{sender.FirstName} {sender.LastName}",
                RecipientId = message.RecipientId,
                RecipientName = $"{recipient.FirstName} {recipient.LastName}",
                Content = message.Content,
                SentAt = message.SentAt
            };
            return Created(string.Empty, messageDto);
        }

        /// <summary>
        /// Retrieves all messages sent to or from a specific user.
        /// Related to US17 - Owner Messaging and US18 - Walker Messaging
        /// </summary>
        /// <param name="userid">The ID of the user</param>
        /// <returns>
        /// 200 OK with a list of messages on success
        /// </returns>
        [HttpGet("{userid}")]
        public async Task<IActionResult> GetMessages(int userid)
        {
            var messages = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
                .Where(m => m.SenderId == userid || m.RecipientId == userid)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            var dtos = messages.Select(m => new MessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderName = $"{m.Sender.FirstName} {m.Sender.LastName}",
                RecipientId = m.RecipientId,
                RecipientName = $"{m.Recipient.FirstName} {m.Recipient.LastName}",
                Content = m.Content,
                SentAt = m.SentAt
            }).ToList();

            return Ok(dtos);
        }
    }
}