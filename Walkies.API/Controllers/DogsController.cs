using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Walkies.API.Data;
using Walkies.API.DTOs;
using Walkies.API.Models;

namespace Walkies.API.Controllers
{
    /// <summary>
    /// Handles dog profile management and provides endpoints for creating,
    /// retrieving, updating, and deleting dog profiles.
    /// Related to US04 - Add Dog and US05 - Edit Dog
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initialises a new instance of the DogsController class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public DogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new dog profile
        /// Relates to US04 - Add Dog
        /// </summary>
        /// <param name="dto">The dog data details.</param>
        /// <returns>
        /// 201 created with dog data on success.
        /// 400 bad request if input data is invalid.
        /// 404 not found if the owner doesnt exist
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> AddDog([FromBody] CreateDogDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var owner = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.OwnerId);

            if (owner == null)
            {
                return NotFound(new {message = "Owner not found."});
            }

            var dog = new Dog
            {
                Name = dto.Name,
                Breed = dto.Breed,
                Age = dto.Age,
                Notes = dto.Notes,
                OwnerId = dto.OwnerId
            };

            _context.Dogs.Add(dog);
            await _context.SaveChangesAsync();

            var dogDto = new DogDto
            {
                Id = dog.Id,
                Name = dog.Name,
                Breed = dog.Breed,
                Age = dog.Age,
                Notes = dog.Notes,
                OwnerId = dog.OwnerId,
                OwnerName = $"{owner.FirstName} {owner.LastName}"
            };

            return CreatedAtAction(nameof(GetDog), new { id = dog.Id }, dogDto);
        }

        /// <summary>
        /// Returns a dog profile by its unique identifier.
        /// Related to US04 - Add Dog
        /// </summary>
        /// <param name="id">The unique identifier of the dog.</param>
        /// <returns>
        /// 200 Ok with dog data on success.
        /// 404 Not Found if the dog does not exist.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDog(int id)
        {
            var dog = await _context.Dogs.Include(d => d.Owner)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dog == null)
            {
                return NotFound(new { message = "Dog not found." });
            }

            return Ok(new DogDto
            {
                Id = dog.Id,
                Name = dog.Name,
                Breed = dog.Breed,
                Age = dog.Age,
                Notes = dog.Notes,
                OwnerId = dog.OwnerId,
                OwnerName = $"{dog.Owner.FirstName} {dog.Owner.LastName}"
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDog(int id, [FromBody] UpdateDogDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dog = await _context.Dogs.Include(d => d.Owner)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dog == null)
            {
                return NotFound(new { message = "Dog not found." });
            }

            dog.Name = dto.Name;
            dog.Breed = dto.Breed;
            dog.Age = dto.Age;
            dog.Notes = dto.Notes;

            await _context.SaveChangesAsync();

            return Ok(new DogDto
            {
                Id = dog.Id,
                Name = dog.Name,
                Breed = dog.Breed,
                Age = dog.Age,
                Notes = dog.Notes,
                OwnerId = dog.OwnerId,
                OwnerName = $"{dog.Owner.FirstName} {dog.Owner.LastName}"
            });
        }
    }
}
