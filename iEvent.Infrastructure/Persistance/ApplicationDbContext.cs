using iEvent.Domain.Entities;
using iEvent.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace iEvent.Infrastructure.Persistance
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingTicket> BookingTickets { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<EventDate> EventDates { get; set; }
        public DbSet<EventTimeSlot> EventTimeSlots { get; set; }
        public DbSet<VenueFacility> VenueFacilities { get; set; }
        public DbSet<VenueImage> VenueImages { get; set; }
        public DbSet<EventImage> EventImages { get; set; }

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

            modelBuilder.Entity<Customer>()
                .HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<Customer>(c => c.IdentityUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.IdentityUserId)
                .IsUnique();

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
                .HasForeignKey(l => l.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdminUser>()
                .HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<AdminUser>(a => a.IdentityUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdminUser>()
                .HasIndex(a => a.IdentityUserId)
                .IsUnique();

            modelBuilder.Entity<Event>()
                .HasMany(e => e.EventDates)
                .WithOne(ed => ed.Event)
                .HasForeignKey(ed => ed.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventDate>()
                .HasMany(ed => ed.TimeSlots)
                .WithOne(ts => ts.EventDate)
                .HasForeignKey(ts => ts.EventDateId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventDate>()
                .Property(ed => ed.Date)
                .HasColumnType("date");

            modelBuilder.Entity<EventTimeSlot>()
                .Property(ts => ts.StartTime)
                .HasColumnType("time");

            modelBuilder.Entity<EventTimeSlot>()
                .Property(ts => ts.EndTime)
                .HasColumnType("time");

            modelBuilder.Entity<Venue>()
                .HasMany(v => v.Facilities)
                .WithOne(f => f.Venue)
                .HasForeignKey(f => f.VenueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Venue>()
                .HasMany(v => v.Images)
                .WithOne(i => i.Venue)
                .HasForeignKey(i => i.VenueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Images)
                .WithOne(i => i.Event)
                .HasForeignKey(i => i.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.BookingTimeSlot)
                .WithMany()
                .HasForeignKey(b => b.BookingTimeSlotId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventImage>()
                .HasIndex(x => new { x.EventId, x.IsBanner })
                .IsUnique()
                .HasFilter("[IsBanner] = 1");
        }
    }
}
