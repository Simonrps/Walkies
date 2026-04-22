using System.ComponentModel.DataAnnotations;

namespace Walkies.API.Models
{
    /// <summary>
    /// Represents a confirmed booking between a dog owner and walker.
    /// A booking is created when a dog walker accepts an open request.
    /// It tracks the full lifecycle of a walk from acceptance to completion,
    /// including check-in, check-out and payment confirmation.
    /// </summary>
    public class WalkBooking
    {
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the foreign key to the associated walk request.
        /// </summary>
        public int WalkRequestId { get; set; }

        /// <summary>
        /// Gets or sets the walk request associated with the booking
        /// </summary>
        public WalkRequest WalkRequest { get; set; } = null!;

        /// <summary>
        /// Gets or sets the foreign key to the dog walker associated with the booking.
        /// </summary>
        public int WalkerId { get; set; }

        /// <summary>
        /// Gets or sets the dog walker associated with the booking
        /// </summary>
        public User Walker { get; set; } = null!;

        /// <summary>
        /// Gets or sets the current status of the booking.
        /// Valid values are "Accepted", "Cancelled", "InProgress", and "Completed".
        /// </summary>
        [Required, MaxLength(20)]
        public string Status { get; set; } = "Accepted";

        /// <summary>
        /// Gets or sets the date and time when the check-in occurred.
        /// </summary>
        public DateTime? CheckInTime { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the check-out occurred.
        /// </summary>
        public DateTime? CheckOutTime { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the booking was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the payment record associated with the booking.
        /// Is Null until the walk is completed and payment is confirmed.
        /// </summary>
        public PaymentRecord? PaymentRecord { get; set; }
    }
}