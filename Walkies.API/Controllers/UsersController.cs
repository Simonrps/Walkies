using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Walkies.API.Data;
using Walkies.API.DTOs;

namespace Walkies.API.Controllers
{
    /// <summary>
    /// Handles user profile management for the Walkies API.
    /// Provides endpints for retrieving and updating user profiles
    /// Related to US03 - Profile Management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the UsersController.
        /// </summary>
        /// <param name="context">The database context.</param>
        public UsersController(ApplicationDbContext context)
        {  
            _context = context; 
        }

        /// <summary>
        /// Retrieves the profile information for a user with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user to retrieve.</param>
        /// <returns>
        /// 200 OK with user profile data on success
        /// 404 Not Found if the user does not exist.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            return Ok(new UserProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                Phone = user.Phone,
                Address = user.Address,
                Latitude = user.Latitude,
                Longitude = user.Longitude,
                CreatedAt = user.CreatedAt
            });
        }
    }
}
