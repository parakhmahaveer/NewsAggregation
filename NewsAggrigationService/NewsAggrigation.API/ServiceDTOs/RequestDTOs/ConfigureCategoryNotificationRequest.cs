using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.API.ServiceDTOs.RequestDTOs
{
    public class ConfigureCategoryNotificationRequest
    {
        public string Username { get; set; }
        public Dictionary<string, bool> CategorySettings { get; set; }
    }
}
