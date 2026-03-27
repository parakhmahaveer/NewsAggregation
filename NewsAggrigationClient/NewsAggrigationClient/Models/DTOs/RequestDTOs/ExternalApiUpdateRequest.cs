using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Models.DTOs.RequestDTOs
{
    public class ExternalApiUpdateRequest
    {
        public int ApiId { get; set; }
        public string? ApiName { get; set; }
        public string? ApiUrl { get; set; }
        public string? ApiKey { get; set; }
    }
}
