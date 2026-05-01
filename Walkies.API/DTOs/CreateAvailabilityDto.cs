using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Walkies.API.DTOs
{
    /// <summary>
    /// Data transfer object for creating a new availability slot.
    /// Comtains the data required for a Dog Walker to set their
    /// availability for dog walking services
    /// Related to US12 - Walker Availability
    /// </summary>
    public class CreateAvailabilityDto
    {
        /// <summary>
        /// gets or sets the unique id for the walker
        /// </summary>
        [Required, JsonRequired]
        public int WalkerId { get; set; }

        /// <summary>
        /// Gets or sets the date and time the walker is available
        /// </summary>
        [Required, JsonRequired]
        public DateTime AvailableFrom { get; set; }

        /// <summary>
        /// Gets or sets the date and time the walker is available until
        /// </summary>
        [Required, JsonRequired]
        public DateTime AvailableTo { get; set; }
    }
}