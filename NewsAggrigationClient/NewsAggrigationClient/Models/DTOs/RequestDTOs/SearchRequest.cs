using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Models.DTOs.RequestDTOs
{
    public class SearchRequest
    {
        public string Query { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string SortBy { get; set; }
    }
}
