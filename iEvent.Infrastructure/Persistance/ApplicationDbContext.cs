using iEvent.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace iEvent.Infrastructure.Persistance
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingTicket> BookingTickets { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Venue>()
                .OwnsOne(v => v.MapLocation);

            modelBuilder.Entity<Venue>()
                .HasMany(v => v.Events)
                .WithOne(e => e.Venue)
                .HasForeignKey(e => e.VenueId);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Tickets)
                .WithOne(t => t.Event)
                .HasForeignKey(t => t.EventId);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Bookings)
                .WithOne(b => b.Customer)
                .HasForeignKey(b => b.CustomerId);

            modelBuilder.Entity<Event>()
                .HasMany<Booking>()
                .WithOne(b => b.Event)
                .HasForeignKey(b => b.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.BookingTickets)
                .WithOne(bt => bt.Booking)
                .HasForeignKey(bt => bt.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TicketType>()
                .HasMany(t => t.Bookings)
                .WithOne(bt => bt.TicketType)
                .HasForeignKey(bt => bt.TicketTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdminUser>()
                .HasMany(a => a.AuditLogs)
                .WithOne(l => l.AdminUser)
                .HasForeignKey(l => l.AdminId);
        }
    }
}
