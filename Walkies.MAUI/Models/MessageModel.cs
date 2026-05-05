namespace Walkies.MAUI.Models
{
    /// <summary>
    /// Represents a message returned from the API.
    /// </summary>
    public class MessageModel
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public int RecipientId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }
}