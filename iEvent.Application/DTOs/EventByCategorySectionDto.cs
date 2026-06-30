using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.DTOs
{
    public class EventByCategorySectionDto
    {
        public string Category { get; set; } = string.Empty;
        public int TotalEvents { get; set; }
        public List<EventRespDto> PreviewEvents { get; set; } = new();
    }
}
