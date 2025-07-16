using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.API.ServiceDTOs.RequestDTOs
{
    public class ExternalApiUpdateRequest
    {
        public int ApiId { get; set; }
        public string? ApiName { get; set; }
        public string? BaseUrl { get; set; }
        public string? ApiKey { get; set; }
    }
}
