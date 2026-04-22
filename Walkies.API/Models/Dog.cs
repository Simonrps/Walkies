using System.ComponentModel.DataAnnotations;

namespace Walkies.API.Models
{
    /// <summary>
    /// Represents a dog associated with a Dog Owner's profile.
    /// A dog owner may have one or more dogs registered.
    /// Dog details are viewable to dog walkers when viewing walk requests.
    /// </summary>
    public class Dog
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Breed { get; set; } = string.Empty;

        [Required]
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets optional notes or comments associated with the dog such as temperament,
        /// behavior, or special care instructions.
        /// </summary>
        [MaxLength(500)]
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets the foreign key referencing the dog owner.
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the owner of the dog.
        /// </summary>
        public User Owner { get; set; } = null!;

        /// <summary>
        /// Gets or sets the collection of walk requests associated with this dog.
        /// </summary>
        public ICollection<WalkRequest> WalkRequests { get; set; } = [];
    }
}