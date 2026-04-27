using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Walkies.API.DTOs
{
    /// <summary>
    /// Data transfer object for creating a new walk request.
    /// Contains the data required for a dog owner to post a
    /// walk request. Related to US06 - Post Walk Request
    /// </summary>
    public class CreateWalkRequestDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the owner
        /// associated with the dog.
        /// </summary>
        [Required, JsonRequired]
        public int OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the dog.
        /// </summary>
        [Required, JsonRequired]
        public int DogId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the request was made.
        /// </summary>
        [Required]
        public DateTime RequestedDate { get; set; }

        /// <summary>
        /// Gets or sets the duration of the walk in minutes.
        /// </summary>
        [Required, Range(15, 180), JsonRequired]
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the location of the walk.
        /// </summary>
        [Required, MaxLength(300)]
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the latitude coordinate in degrees.
        /// </summary>
        [Required]
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude coordinate value
        /// </summary>
        [Required]
        public double Longitude { get; set; }
    }
}