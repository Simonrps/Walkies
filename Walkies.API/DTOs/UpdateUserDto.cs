using System.ComponentModel.DataAnnotations;

namespace Walkies.API.DTOs
{
    /// <summary>
    /// Data Transfer object for updating user information.
    /// Comtains the fields the user can modify on their profile
    /// Related to US03 - Profile Management
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>
        /// getss or sets the first name of the user.
        /// </summary>
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// getss or sets the users last name.
        /// </summary>
        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// getss or sets the users phone number.
        /// </summary>
        [MaxLength(20)]
        public string? Phone { get; set; }

        /// <summary>
        /// getss or sets the users address.
        /// </summary>
        [MaxLength(300)]
        public string? Address { get; set; }

        /// <summary>
        /// gets or set the latitude of the user
        /// used to calculate the distance between users
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// gets or set the Longitude of the user
        /// used to calculate the distance between users
        /// </summary>
        public double? Longitude { get; set; }
    }
}