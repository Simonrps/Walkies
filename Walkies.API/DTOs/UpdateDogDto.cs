using System.ComponentModel.DataAnnotations;

namespace Walkies.API.DTOs
{
    /// <summary>
    /// Data transfer object for updating an existing dog.
    /// Contains the data that can be modified for a dog
    /// Relates to US05 - Edit Dog
    /// </summary>
    public class UpdateDogDto
    {
        /// <summary>
        /// Gets or sets the name of the dog
        /// </summary>
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the breed of the dog
        /// </summary>
        [Required, MaxLength(100)]
        public string Breed { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets dogs age
        /// </summary>
        [Required, Range(0, 30)]
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets additional notes or comments associated with the dog
        /// </summary>
        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
