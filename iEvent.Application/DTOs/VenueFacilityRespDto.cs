using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.DTOs
{
    public class VenueFacilityRespDto
    {
        public Guid FacilityId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
