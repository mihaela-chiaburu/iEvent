using System.ComponentModel.DataAnnotations;
using iEvent.Domain.ValueObjects;

namespace iEvent.Domain.Entities
{
    public class Venue
    {
        [Key]
        public Guid VenueId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Range(1, 1000000)]
        public int Capacity { get; set; }

        [Required]
        public MapLocation MapLocation { get; set; } = new(0, 0);

        [StringLength(2000)]
        public string? Description { get; set; }

        [Phone]
        [StringLength(30)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(200)]
        public string? Email { get; set; }

        public bool IsDraft { get; set; } = true;

        public List<Event> Events { get; set; } = new();
        public List<VenueFacility> Facilities { get; set; } = new();
        public List<VenueImage> Images { get; set; } = new();
    }
}
