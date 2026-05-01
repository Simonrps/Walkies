namespace Walkies.API.DTOs
{
    /// <summary>
    /// Data transfer object for returning availability slot details
    /// Contains the public facing data for a Walker availability
    /// Related to US12 - Walker Availability
    /// </summary>
    public class AvailabilityDto
    {
        /// <summary>
        /// Gets or sets the unique id of the availability slot
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// gets of sets the unique id for the walker
        /// </summary>
        public int WalkerId { get; set; }

        /// <summary>
        /// gets or sets the walkers name
        /// </summary>
        public string WalkerName { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the date and time the walker is available from
        /// </summary>
        public DateTime AvailableFrom { get; set; }

        /// <summary>
        /// gets or sets the date and time the walker is available until
        /// </summary>
        public DateTime AvailableTo { get; set; }

        /// <summary>
        /// Gets or sets a value to indicate if the availability
        /// slot is active
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// gets or sets the date and time the availability slot was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
