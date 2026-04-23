using System.ComponentModel.DataAnnotations;

namespace Walkies.API.Models
{
    /// <summary>
    ///  Represents a registered user of the Walkies application.
    ///  A user may be registered as a dog owner or a dog walker
    ///  with role-based access controlling availabl functionality.
    /// </summary>
    public class User
    {

        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets email used for log in and must be unique across all registered users.
        /// </summary>
        [Required, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the hashed password of the user. Plain text is never stored.
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the role of the user which determines their access level and functionality
        /// </summary>
        [Required, MaxLength(20)]
        public string Role { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }

        /// <summary>
        /// gets or sets the latitude for determining the proximity of dog walkers to dog owners
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// gets or sets the longitude for determining the proximity of dog walkers to dog owners
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// gets or sets date and time stamp of account creation.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// get and set dogs owned by the user if they are a dog owner
        /// </summary>
        public ICollection<Dog> Dogs { get; set; } = [];

        /// <summary>
        /// get and set walk requests associated with the user populated only for dog owers.
        /// </summary>
        public ICollection<WalkRequest> WalkRequests { get; set; } = [];

        /// <summary>
        /// get and set walk bookings associated with the user populated only for dog walkers.
        /// </summary>
        public ICollection<WalkBooking> WalkBookings { get; set; } = [];

        /// <summary>
        /// get and set availability slots for the user if they are a dog walker.
        /// </summary>
        public ICollection<WalkerAvailability> Availability { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of messages sent by the user.
        /// </summary>
        public ICollection<Message> SentMessages { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of messages received by the user.
        /// </summary>
        public ICollection<Message> ReceivedMessages { get; set; } = [];
    }
}