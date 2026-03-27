using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Models.DTOs.ResponseDTOs
{
    public class ExternalApiResponse
    {
        public int Id { get; set; }
        public string ApiName { get; set; }
        public string ApiKey { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastAccessedAt { get; set; }
    }
}
