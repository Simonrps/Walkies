namespace Walkies.API.DTOs
{
    /// <summary>
    /// Data transfer object for a dog.
    /// Comtains the public-facing data 
    /// for a registered dog in the app.
    /// Relates to US04 - Add Dog and US05 - Edit Dog.
    /// </summary>
    public class DogDto
    {
        /// <summary>
        /// gets or sets the unique identifier for the dog.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the dog.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the breed of the dog
        /// </summary>
        public string Breed { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the age of the dog
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// gets or sets any additional notes about the dog
        /// like temmperament, health conditions, or special needs
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the owner 
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the name of the owner
        /// </summary>
        public string OwnerName { get; set; } = string.Empty;
    }
}