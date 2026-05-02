using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Walkies.API.DTOs
{
    /// <summary>
    /// Data transfer object for sending a new message. COntains
    /// the data required for a user to send a message to another
    /// user in the application. Related to US17 - Owner Messaging
    /// and US19 - Walker Messaging
    /// </summary>
    public class CreateMessageDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the message sender
        /// </summary>
        [Required, JsonRequired]
        public int SenderId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the message recipient
        /// </summary>
        [Required, JsonRequired]
        public int RecipientId { get; set; }

        [Required, MaxLength(1000)]
        public string Content { get; set; } = string.Empty;
    }
}