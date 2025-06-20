using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.API.ServiceDTOs.ResponseDTOs
{
    public class GetCategoriesResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
