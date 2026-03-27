using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.API.ServiceDTOs.RequestDTOs
{
    public class ConfigureKeywordNotificationRequest
    {
        public int UserId { get; set; }
        public List<string> Keywords { get; set; }
        public bool IsEnabled { get; set; }
    }
}
