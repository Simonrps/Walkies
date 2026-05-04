namespace Walkies.API.DTOs
{
    /// <summary>
    /// Data transfer object for returning booking info.
    /// Contains the public facing data for a booking.
    /// Relates to US10 - View Booking, US11 - View All Bookings,
    /// US12 - Check In, US13 - Check Out
    /// </summary>
    public class BookingDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the booking.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the walk request.
        /// </summary>
        public int WalkRequestId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the walker.
        /// </summary>
        public int WalkerId { get; set; }

        /// <summary>
        /// Gets or sets the name of the walker.
        /// </summary>
        public string WalkerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique identifier of the owner
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the name of the owner
        /// </summary>
        public string OwnerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the dog.
        /// </summary>
        public string DogName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the scheduled date and time of the walk.
        /// </summary>
        public DateTime ScheduledDate { get; set; }

        /// <summary>
        /// Gets or sets the duration of the walk in minutes.
        /// </summary>
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the location of the walk
        /// </summary>
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current status of the booking
        /// Valid values: "Confirmed", "Active", "Completed"
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the walker checks in for the walk.
        /// </summary>
        public DateTime? CheckInTime { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the check-out occurred.
        /// </summary>

        public DateTime? CheckOutTime { get; set; }

        /// <summary>
        /// Gets or sets the current latitude of the walker during a walk.
        /// </summary>
        public double? CurrentLatitude { get; set; }

        /// <summary>
        /// Gets or sets the current longitude of the walker during a walk.
        /// </summary>
        public double? CurrentLongitude { get; set; }

        /// <summary>
        /// Gets or sets the date and time the booking was created.
        /// </summary>
        public DateTime? CreatedAt { get; set; }
    }
}