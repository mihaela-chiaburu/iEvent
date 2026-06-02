using System;
using System.Collections.Generic;
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

        public List<Event> Events { get; set; } = new();
    }
}
