using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.DTOs
{
    public class VenueImageRespDto
    {
        public Guid ImageId { get; set; }
        public string Url { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
