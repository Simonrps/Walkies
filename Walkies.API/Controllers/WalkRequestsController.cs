using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Walkies.API.Data;
using Walkies.API.DTOs;
using Walkies.API.Models;
using Walkies.API.Services;

namespace Walkies.API.Controllers
{
    /// <summary>
    /// Handles walk request management for the API. It provides
    /// endpoints for posting, retrieving, and canceling walk requests.
    /// Reated to US06 - Post Walk Request, US07 - Browse Walk Requests
    /// and US08 - Cancel Walk Request.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class WalkRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initialises a new instance of the WalkRequestsController.
        /// </summary>
        /// <param name="context">The database context</param>
        public WalkRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Posts a new walk request for a dog owner
        /// Validates that the requested date is in the future
        /// and that the owner has at least one dog registered
        /// Related ti US06 - Post Walk Request
        /// </summary>
        /// <param name="dto">The walk request details</param>
        /// <returns>
        /// 201 created with the walk request details on success
        /// 400 Bad Request if the input is invalid or the requested date is in the past
        /// 404 Not Found if the specified owner or dog does not exist
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> PostWalkRequest([FromBody] CreateWalkRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dto.RequestedDate < DateTime.UtcNow)
            {
                return BadRequest(new { message = "Walk request date must be in the future" });
            }

            var owner = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.OwnerId);
            if (owner == null)
            {
                return NotFound(new { message = "Owner not found" });
            }

            var ownerHasDogs = await _context.Dogs.AnyAsync(d => d.OwnerId == dto.OwnerId);
            if (!ownerHasDogs)
            {
                return BadRequest(new { message = "Owner must have at least one dog to create a walk request" });
            }

            var dog = await _context.Dogs.FirstOrDefaultAsync(d => d.Id == dto.DogId);
            if (dog == null)
            {
                return NotFound(new { message = "Dog not found" });
            }

            var walkRequest = new WalkRequest
            {
                OwnerId = dto.OwnerId,
                DogId = dto.DogId,
                RequestedDate = dto.RequestedDate,
                DurationMinutes = dto.DurationMinutes,
                Location = dto.Location,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Status = "Open"
            };

            _context.WalkRequests.Add(walkRequest);
            await _context.SaveChangesAsync();

            var walkRequestDto = new WalkRequestDto
            {
                Id = walkRequest.Id,
                OwnerId = walkRequest.OwnerId,
                OwnerName = $"{owner.FirstName} {owner.LastName}",
                DogId = walkRequest.DogId,
                DogName = dog.Name,
                RequestedDate = walkRequest.RequestedDate,
                DurationMinutes = walkRequest.DurationMinutes,
                Location = walkRequest.Location,
                Latitude = walkRequest.Latitude,
                Longitude = walkRequest.Longitude,
                Status = walkRequest.Status
            };

            return CreatedAtAction(nameof(GetWalkRequest), new { id = walkRequest.Id }, walkRequestDto);
        }

        /// <summary>
        /// Retrieves the walk request with the specified identifier.
        /// Related to US06 - Post Walk Request
        /// </summary>
        /// <param name="id">The unique identifier of the walk request</param>
        /// <returns>
        /// 200 OK with the walk request details if found
        /// 404 Not Found if the walk request does not exist
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWalkRequest(int id)
        {
            var walkRequest = await _context.WalkRequests
                .Include(wr => wr.Owner)
                .Include(wr => wr.Dog)
                .FirstOrDefaultAsync(wr => wr.Id == id);

            if (walkRequest == null)
            {
                return NotFound(new { message = "Walk request not found" });
            }

            return Ok(new WalkRequestDto
            {
                Id = walkRequest.Id,
                OwnerId = walkRequest.OwnerId,
                OwnerName = $"{walkRequest.Owner.FirstName} {walkRequest.Owner.LastName}",
                DogId = walkRequest.DogId,
                DogName = walkRequest.Dog.Name,
                RequestedDate = walkRequest.RequestedDate,
                DurationMinutes = walkRequest.DurationMinutes,
                Location = walkRequest.Location,
                Latitude = walkRequest.Latitude,
                Longitude = walkRequest.Longitude,
                Status = walkRequest.Status
            });
        }
        /// <summary>
        /// Retrieves a list of open walk requests optionally filtered by 
        /// distance from a specified locations using the haversine formula
        /// Related to US07 - Walker Searches For Requests.
        /// </summary>
        /// <param name="latitude">Optional latitude of the wlakers location</param>
        /// <param name="longitude">Optional longitude of the walkers location</param>
        /// <param name="distanceKm">Optional search radius in kilometres</param>
        /// <returns>
        /// 200 OK with a list of walk requests matching the search criteria
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetWalkRequests(
            [FromQuery] double? latitude = null,
            [FromQuery] double? longitude = null,
            [FromQuery] double? distanceKm = null
            )
        {
            var walkRequests = await _context.WalkRequests
                .Include(wr => wr.Owner)
                .Include(wr => wr.Dog)
                .Where(wr => wr.Status == "Open")
                .ToListAsync();

            if (latitude.HasValue && longitude.HasValue && distanceKm.HasValue)
            {
                walkRequests = walkRequests
                    .Where(wr => DistanceCalculator.Calculate(
                        latitude.Value, longitude.Value,
                        wr.Latitude, wr.Longitude) <= distanceKm.Value)
                    .ToList();
            }

            var dtos = walkRequests.Select(wr => new WalkRequestDto
            {
                Id = wr.Id,
                OwnerId = wr.OwnerId,
                OwnerName = $"{wr.Owner.FirstName} {wr.Owner.LastName}",
                DogId = wr.DogId,
                DogName = wr.Dog.Name,
                RequestedDate = wr.RequestedDate,
                DurationMinutes = wr.DurationMinutes,
                Location = wr.Location,
                Latitude = wr.Latitude,
                Longitude = wr.Longitude,
                Status = wr.Status
            }).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Cancels a walk requedt by its id. Updates the status to Cancelled
        /// rather than deleting it to perserve the audit trail.
        /// Related to US08 - Cancel Walk Request
        /// </summary>
        /// <param name="id">The unique identifier of the walk request.</param>
        /// <returns>
        /// 204 No Content response on success
        /// 404 Not Found response if the walk request does not exist.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelWalkRequest(int id)
        {
            var walkRequest = await _context.WalkRequests
                .FirstOrDefaultAsync(wr => wr.Id == id);

            if (walkRequest == null)
            {
                return NotFound(new { message = "Walk request not found" });
            }

            walkRequest.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}