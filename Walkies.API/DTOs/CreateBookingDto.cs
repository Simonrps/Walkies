using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Walkies.API.DTOs
{
    /// <summary>
    /// Data transfer object for creating a new booking.
    /// A booking is created when a walker accepts an open request.
    /// Related to US09 - Accept Walk Request
    /// </summary>
    public class CreateBookingDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the walk request.
        /// </summary>
        [Required, JsonRequired]
        public int WalkRequestId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the walker.
        /// </summary>
        [Required, JsonRequired]
        public int WalkerId { get; set; }
    }
}