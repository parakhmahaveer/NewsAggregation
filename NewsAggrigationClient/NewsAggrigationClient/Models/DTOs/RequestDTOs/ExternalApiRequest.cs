using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Models.DTOs.RequestDTOs
{
    public class ExternalApiRequest
    {
        public string ApiName { get; set; }
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
    }
}
