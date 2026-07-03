using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iEvent.Domain.Entities
{
    public class EventImage
    {
        [Key]
        public Guid ImageId { get; set; }

        [Required]
        public Guid EventId { get; set; }

        [Required]
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string CloudinaryPublicId { get; set; } = string.Empty;

        [Required]
        public int SortOrder { get; set; }

        [ForeignKey(nameof(EventId))]
        public Event? Event { get; set; }
    }
}
