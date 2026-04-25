using System.ComponentModel.DataAnnotations;

namespace Walkies.API.DTOs
{
    /// <summary>
    /// Data Transfer object for user login requests.
    /// Contains the required log in details to authenticate
    /// a user and issue a JWT token. Related to US02 - Login
    public class LoginDto
    {
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
    }
}
