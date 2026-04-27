namespace Walkies.API.DTOs
{
    /// <summary>
    /// Data transfer object for user profile information.
    /// Contains the public-facing profiledata for a registered user.
    /// Related to US03 - Profile Management
    /// </summary>
    public class UserProfileDto
    {
        /// <summary>
        /// gets or sets the unique identifier of the user profile.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// gets or sets the firstname of the user
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the lsstname of the user
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the users email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the users role
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the users phone number
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// gets or sets the users address
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// gets or sets the latitude of the user
        /// Calculates the proximity between owners and walkers
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// gets or sets the Longitude of the user
        /// Calculates the proximity between owners and walkers
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// gets or set the date and time the account was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}