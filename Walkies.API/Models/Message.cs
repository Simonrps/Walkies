using System.ComponentModel.DataAnnotations;

namespace Walkies.API.Models
{
    /// <summary>
    /// Represents a message sent between users through the Walkies in app
    /// messaging service. Messages are stored against both the sender and
    /// recipient to support conversation thread display for both user types.
    /// </summary>
    public class Message
    {

        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the foreign key of the user who is the sender of the message.
        /// </summary>
        public int SenderId { get; set; }

        /// <summary>
        /// Gets or sets the user who sent the message.
        /// </summary>
        public User Sender { get; set; }

        /// <summary>
        /// Gets or sets the foreign key of the user who is the recipient of the message.
        /// </summary>
        public int RecipientId { get; set; }

        /// <summary>
        /// Gets or sets the user who is the recipient of the message.
        /// </summary>
        public User Recipient { get; set; }

        /// <summary>
        /// Gets or sets the text content of the message.
        /// Empty messages are not allowed and are rejectedat validation layer.
        /// </summary>
        [Required, MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the message has been read
        /// by the recipient.
        /// </summary>
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Gets or sets the date and time when the message was sent.
        /// </summary>
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
