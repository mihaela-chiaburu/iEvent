using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iEvent.Domain.Entities
{
    public class VenueFacility
    {
        [Key]
        public Guid FacilityId { get; set; }

        [Required]
        public Guid VenueId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [ForeignKey(nameof(VenueId))]
        public Venue? Venue { get; set; }
    }
}
