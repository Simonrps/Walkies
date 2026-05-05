namespace Walkies.MAUI.Models
{
    /// <summary>
    /// Represents a payment record returned from the API.
    /// </summary>
    public class PaymentModel
    {
        public int Id { get; set; }
        public int WalkBookingId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string WalkerName { get; set; } = string.Empty;
        public string DogName { get; set; } = string.Empty;
        public DateTime WalkDate { get; set; }
        public int DurationMinutes { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}