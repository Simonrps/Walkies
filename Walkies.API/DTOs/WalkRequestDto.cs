namespace Walkies.API.DTOs
{
    /// <summary>
    /// Data transfer object for walk request details.
    /// Comtains the public-facing data for walk requests.
    /// Related to US06 - Post Walk Request and US07 Browse Walk Requests
    /// </summary>
    public class WalkRequestDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the walk request
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the dog owner
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the name of the owner
        /// </summary>
        public string OwnerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique identifier for the dog.
        /// </summary>
        public int DogId { get; set; }

        /// <summary>
        /// Gets or sets the name of the dog.
        /// </summary>
        public string DogName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date when the request was made.
        /// </summary>
        public DateTime RequestedDate { get; set; }

        /// <summary>
        /// Gets or sets the duration of the event, in minutes.
        /// </summary>
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the location associated with the object.
        /// </summary>
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the latitude coordinate value.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude coordinate value.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the current status as a string value.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
