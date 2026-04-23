using Microsoft.EntityFrameworkCore;
using Walkies.API.Models;

namespace Walkies.API.Data
{
    /// <summary>
    /// Representing the Entity Framework Core databse context for the Walkies application.
    /// Registers all domain models as DbSets and configures entity realtionships using the
    /// Fluent API. This class serves as the primary interface between the application and
    /// the underlying SQL database.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Initialises a new instance of the ApplicationDbContext with the specified db context options.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// gets or sets the DbSet of users registered in the application.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// gets or sets the DbSet of dogs associated with dog owner profiles
        /// </summary>
        public DbSet<Dog> Dogs { get; set; }

        /// <summary>
        /// gets or sets the DbSet of walk requests posted by dog owner
        /// </summary>
        public DbSet<WalkRequest> WalkRequests { get; set; }

        /// <summary>
        /// gets or sets the DbSet of availability slots by dog walkers.
        /// </summary>
        public DbSet<WalkerAvailability> WalkerAvailabilities { get; set; }

        /// <summary>
        /// gets or sets the DbSet of messages exchanged between dog owners and dog walkers.
        /// </summary>
        public DbSet<Message> Messages { get; set; }

        /// <summary>
        /// gets or sets the DbSet of payment confirmation records generated when booking is completed.
        /// </summary>
        public DbSet<PaymentRecord> PaymentRecords { get; set; }

        /// <summary>
        /// Configure the entity relationships and constraints using the Fluent API.
        /// This method is called by the Entity Framework when the model for the context
        /// is being created.
        /// </summary>
        /// <param name="modelBuilder">The builder used to construct the model for the context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Role).HasMaxLength(20);
            });

            // Dog configuration
            modelBuilder.Entity<Dog>(entity =>
            {
                entity.HasOne(d => d.Owner)
                      .WithMany(u => u.Dogs)
                      .HasForeignKey(d => d.OwnerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // WalkRequest configuration
            modelBuilder.Entity<WalkRequest>(entity =>
            {
                entity.HasOne(wr => wr.Owner)
                      .WithMany(u => u.WalkRequests)
                      .HasForeignKey(wr => wr.OwnerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(wr => wr.Dog)
                      .WithMany(d => d.WalkRequests)
                      .HasForeignKey(wr => wr.DogId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            //WalkBooking configuration
            modelBuilder.Entity<WalkBooking>(entity =>
            {
                entity.HasOne(wb => wb.WalkRequest)
                      .WithOne(wr => wr.WalkBooking)
                      .HasForeignKey<WalkBooking>(wb => wb.WalkRequestId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(wb => wb.Walker)
                      .WithMany(u => u.WalkBookings)
                      .HasForeignKey(wb => wb.WalkerId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // WalkerAvailability configuration
            modelBuilder.Entity<WalkerAvailability>(entity =>
            {
                entity.HasOne(wa => wa.Walker)
                      .WithMany(u => u.Availability)
                      .HasForeignKey(wa => wa.WalkerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Message configuration
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasOne(m => m.Sender)
                      .WithMany(u => u.SentMessages)
                      .HasForeignKey(m => m.SenderId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(m => m.Recipient)
                      .WithMany(u => u.ReceivedMessages)
                      .HasForeignKey(m => m.RecipientId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // PaymentRecord configuration
            modelBuilder.Entity<PaymentRecord>(entity =>
            {
                entity.HasOne(pr => pr.WalkBooking)
                      .WithOne(wb => wb.PaymentRecord)
                      .HasForeignKey<PaymentRecord>(pr => pr.WalkBookingId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(pr => pr.Amount).HasColumnType("decimal(18,2)");
            });
        }
    }
}
