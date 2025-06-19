using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.API.ServiceDTOs.ResponseDTOs
{
    public class CategoryResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<string> Keywords { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
