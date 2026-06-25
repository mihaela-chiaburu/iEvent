using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Domain.Entities
{
    public class VenueImage
    {
        [Key]
        public Guid ImageId { get; set; }

        [Required]
        public Guid VenueId { get; set; }

        [Required]
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;

        [Required]
        public int SortOrder { get; set; }

        [ForeignKey(nameof(VenueId))]
        public Venue? Venue { get; set; }
    }
}
