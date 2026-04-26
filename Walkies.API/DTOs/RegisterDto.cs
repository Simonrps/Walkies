using System.ComponentModel.DataAnnotations;

namespace Walkies.API.DTOs
{
    /// <summary>
    /// Data Transfer object for user registration requests.
    /// Contains the data required to create a new user account
    /// in the Walkies application. Related to US01 - Registration
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// gets or sets the first name of the user
        /// </summary>
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the last name of the user
        /// </summary>
        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the email of the user
        /// Used as unique login identifier
        /// </summary>
        [Required, MaxLength(200)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the plain text password provided by user
        /// This is hashed using BCrypt before being stored in DB.
        /// </summary>
        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the users role
        /// Valid values are "Owner" and "Walker".
        /// </summary>
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}