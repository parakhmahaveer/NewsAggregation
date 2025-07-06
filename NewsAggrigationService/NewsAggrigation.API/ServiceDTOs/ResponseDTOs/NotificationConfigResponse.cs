using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.API.ServiceDTOs.ResponseDTOs
{
    public class NotificationConfigResponse
    {
        public List<string> Categories { get; set; }
        public List<string> Keywords { get; set; }
    }
}
