using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Walkies.API.DTOs
{
    /// <summary>
    /// Data transfer object for updating a walkers location during
    /// active walks. Used by the MAUI frontend to poll the walkers
    /// current coordinates at regular intervals.
    /// Related to US15 - GPS Tracking During Walk
    /// </summary>
    public class UpdateLocationDto
    {
        /// <summary>
        /// Gets or sets the current latitude of the walker.
        /// </summary>
        [Required, JsonRequired]
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the current longitude of the walker.
        /// </summary>
        [Required, JsonRequired]
        public double Longitude { get; set; }

    }
}