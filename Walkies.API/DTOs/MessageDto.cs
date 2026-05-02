namespace Walkies.API.DTOs
{
    /// <summary>
    /// Data transfer object for returning message details.
    /// Contains the public facing data for a message.
    /// Related to US17 - Owner Messaging and US18 - Walker Messaging
    /// </summary>
    public class MessageDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the message
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the sender
        /// </summary>
        public int SenderId { get; set; }

        /// <summary>
        /// Gets or sets the display name of the sender
        /// </summary>
        public string SenderName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique identifier of the recipient
        /// </summary>
        public int RecipientId { get; set; }

        /// <summary>
        /// Gets or sets the name of the recipient
        /// </summary>
        public string RecipientName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the content of the message
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the message was sent
        /// </summary>
        public DateTime SentAt { get; set; }
    }
}