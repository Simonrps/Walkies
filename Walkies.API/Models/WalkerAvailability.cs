using System.ComponentModel.DataAnnotations;

namespace Walkies.API.Models
{
    /// <summary>
    /// Represents an availability slot set by a dog walker.
    /// Walkers define the dates and times they are available.
    /// Availaility slots are used to filter search results when a owners search
    /// for walkers by date ensuring only available walkers are returned
    /// </summary>
    public class WalkerAvailability
    {
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the foreign key referencing the walker the availability belongs to.
        /// </summary>
        public int WalkerId { get; set; }

        /// <summary>
        /// Gets or sets the walker associated with the availability slot.
        /// </summary>
        public User Walker { get; set; } = null!;

        /// <summary>
        /// Gets or sets the date and time from which the walker's availablity starts.
        /// </summary>
        [Required]
        public DateTime AvailableFrom { get; set; }

        /// <summary>
        /// Gets or sets the date and time from which the walker's availablity ends.
        /// </summary>
        [Required]
        public DateTime AvailableTo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the availability slot
        /// is currently active. Slots associated with a confirmed booking
        /// should be marked unavailable.
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Gets or sets the date and time when the availability slot was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
