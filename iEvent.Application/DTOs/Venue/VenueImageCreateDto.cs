using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.DTOs.Venue
{
    public class VenueImageCreateDto
    {
        public string Url { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
