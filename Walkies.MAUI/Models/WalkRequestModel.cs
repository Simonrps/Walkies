namespace Walkies.MAUI.Models
{
    /// <summary>
    /// Represents a walk request returned from the API
    /// </summary>
    public class WalkRequestModel
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public int DogId { get; set; }
        public string DogName { get; set; } = string.Empty;
        public DateTime RequestedDate { get; set; }
        public int DurationMinutes { get; set; }
        public string Location { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}