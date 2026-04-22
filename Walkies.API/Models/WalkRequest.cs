using System.ComponentModel.DataAnnotations;

namespace Walkies.API.Models
{
    /// <summary>
    /// Represents a walk request posted by a dog owner.
    /// A walk request specifies the date, duration and location for a walk
    /// and is visible to dog walkers who can search for open requests in their area.
    /// </summary>
    public class WalkRequest
    {
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the foreign key to the dog owner who posted the walk request.
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        /// Gets or sets details of person who posted the walk request.
        /// </summary>
        public User Owner { get; set; } = null!;

        /// <summary>
        /// Gets or sets the foreign key to the dog associated with the walk request.
        /// </summary>
        public int DogId { get; set; }

        /// <summary>
        /// Gets or sets the dog associated with the walk request.
        /// </summary>
        public Dog Dog { get; set; } = null!;

        /// <summary>
        /// Gets or sets the date and time when the request was made.
        /// Must be a future date & time.
        /// </summary>
        [Required]
        public DateTime RequestedDate { get; set; }

        /// <summary>
        /// Gets or sets the duration of the walk in minutes.
        /// </summary>
        [Required]
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the latitude of the location for the walk request.
        /// </summary>
        [Required]
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the Longitude of the location for the walk request.
        /// </summary>
        [Required]
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the human readable address of the walk location.
        /// </summary>
        [Required, MaxLength(300)]
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current status of the walk request.
        /// Valid values are Open, Accepted, Declined, Cancelled.
        /// </summary>
        [Required, MaxLength(20)]
        public string Status { get; set; } = "Open";

        /// <summary>
        /// Gets or sets the date and time when the walk request was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the booking associated with the walk request.
        /// Null until a Dog walker accepts the request.
        /// </summary>
        public WalkBooking? WalkBooking { get; set; }
    }
}
