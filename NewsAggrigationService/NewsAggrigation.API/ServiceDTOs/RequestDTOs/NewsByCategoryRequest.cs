using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.API.ServiceDTOs.RequestDTOs
{
    public class NewsByCategoryRequest
    {
        public string Category { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
