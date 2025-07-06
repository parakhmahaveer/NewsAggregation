using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.API.ServiceDTOs.RequestDTOs
{
    public class ConfigureCategoryNotificationRequest
    {
        public int UserId { get; set; } = 0;
        public Dictionary<string, bool> CategorySettings { get; set; }
    }
}
