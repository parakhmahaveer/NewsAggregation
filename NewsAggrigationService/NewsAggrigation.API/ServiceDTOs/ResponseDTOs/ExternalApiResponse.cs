using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.API.ServiceDTOs.ResponseDTOs
{
    public class ExternalApiResponse
    {
        public int Id { get; set; }
        public string ApiName { get; set; }
        public string BaseUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastAccessedAt { get; set; }
    }
}
