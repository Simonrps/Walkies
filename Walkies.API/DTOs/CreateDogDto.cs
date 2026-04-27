using System.ComponentModel.DataAnnotations;

namespace Walkies.API.DTOs
{
    /// <summary>
    /// Data transfer object for creating a new dog.
    /// Contains the data required to register a dog
    /// Related to US04 - Add Dog
    /// </summary>
    public class CreateDogDto
    {
        /// <summary>
        ///  of the dog
        /// </summary>
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the breed of the dog
        /// </summary>
        [Required, MaxLength(100)]
        public string Breed { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the age value.
        /// </summary>
        /// <remarks>The age must be between 0 and 30, inclusive. Assigning a value outside this range
        /// will result in a validation error.</remarks>
        [Required, Range(0, 30)]
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets additional notes or comments associated with the dog
        /// </summary>
        [MaxLength(500)]
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the owner
        /// </summary>
        [Required]
        public int OwnerId { get; set; }
    }
}