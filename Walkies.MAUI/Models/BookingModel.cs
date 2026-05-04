namespace Walkies.MAUI.Models
{
    /// <summary>
    /// Represents a booking returned from the API
    /// </summary>
    public class BookingModel
    {
        public int Id { get; set; }
        public int WalkRequestId { get; set; }
        public int WalkerId { get; set; }
        public string WalkerName { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string DogName { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public int DurationMinutes { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public double? CurrentLatitude { get; set; }
        public double? CurrentLongitude { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
