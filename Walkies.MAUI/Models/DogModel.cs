namespace Walkies.MAUI.Models
{
    /// <summary>
    /// Represents a dog returned from the API
    /// </summary>
    public class DogModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public int Age { get; set; }
        public string? Notes { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
    }
}
