namespace Walkies.API.DTOs
{
    /// <summary>
    /// Data transfer object for successful authentication responses.
    /// Returned to the client after a successful registration or login.
    /// Contains the JWT token and user info required by Maui client to
    /// authenticate API requests and route the user to correct dashboard
    /// based on their role. Related to US01 - Registration and US02 - Login
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// gets or sets the JWT token issued when successfully authenticated.
        /// Must be included with API requests as bearer token in authorisation header.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the unique ID of authenticated user.
        /// Used by Maui client to fetch user data.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// gets or sets the role of the autheniticated user.
        /// Valid values are "Owner" and "Walker". Used by Maui
        /// client to route user to correct dashboard after login.
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the first name of the authenitcated user.
        /// Usedby Maui client to display user's name in the UI after login
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the email of the authenticated user.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}
