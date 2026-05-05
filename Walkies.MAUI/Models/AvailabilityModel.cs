namespace Walkies.MAUI.Models
{
    /// <summary>
    /// Represents the availability of a walker returned from the API
    /// </summary>
    public class AvailabilityModel
    {
        public int Id { get; set; }
        public int WalkerId { get; set; }
        public string WalkerName { get; set; } = string.Empty;
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}