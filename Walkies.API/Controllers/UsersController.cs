using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Walkies.API.Data;
using Walkies.API.DTOs;
using Walkies.API.Services;

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

        /// <summary>
        /// Updates the profile of a user by their unique identifier.
        /// Related to US03 - Profile Management
        /// </summary>
        /// <param name="id">The unique identifier of the user</param>
        /// <param name="dto">The updated profile details</param>
        /// <returns>
        /// 200 ok with updated data on success
        /// 404 not found if user does not exist
        /// 400 bad request if the request is invalid
        /// </returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Phone = dto.Phone;
            user.Address = dto.Address;
            user.Latitude = dto.Latitude;
            user.Longitude = dto.Longitude;

            await _context.SaveChangesAsync();

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

        /// <summary>
        /// Retrieves all available Dog Walkers within a specified distance
        /// of the given coordinates for a specified date.
        /// Uses the Haversine formula to calculate distance between coordinates.
        /// Relates to US08 - Owner Searche for Walkers
        /// </summary>
        /// <param name="latitude">The latitude of the search origin</param>
        /// <param name="longitude">The longitude of the search origin</param>
        /// <param name="distanceKm">The search radius in kilometres</param>
        /// <param name="date">The date to check walker availability against</param>
        /// <returns>
        /// 200 ok with a list of availabile walkers within the radiius
        /// 400 Bad Request if the required parameters are missing
        /// </returns>
        [HttpGet("walkers")]
        public async Task<IActionResult> GetWalkers(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double distanceKm,
            [FromQuery] DateTime date
            )
        {
            if (distanceKm <= 0)
            {
                return BadRequest(new { Message = "Distance must be greater than zero" });
            }

            var walkers = await _context.Users
                .Where(u => u.Role == "Walker"
                && u.Latitude != null
                && u.Longitude != null
                && u.Availability.Any(
                    a => a.IsAvailable
                    && a.AvailableFrom.Date <= date.Date.AddHours(23).AddMinutes(59)
                    && a.AvailableTo.Date >= date.Date)
                )
                .ToListAsync();
            var nearbyWalkers = walkers
                .Where(w => DistanceCalculator.Calculate(
                    latitude, longitude,
                    w.Latitude!.Value, w.Longitude!.Value) <= distanceKm)
                .Select(w => new UserProfileDto
                {
                    Id = w.Id,
                    FirstName = w.FirstName,
                    LastName = w.LastName,
                    Email = w.Email,
                    Role = w.Role,
                    Phone = w.Phone,
                    Address = w.Address,
                    Latitude = w.Latitude,
                    Longitude = w.Longitude,
                    CreatedAt = w.CreatedAt
                }).ToList();

            return Ok(nearbyWalkers);
        }
    }
}