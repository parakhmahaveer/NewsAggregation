using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Models.DTOs.ResponseDTOs
{
    public class NotificationResponse
    {
        public string Title { get; set; }
        public DateTime SentDate { get; set; }
        public string Source { get; set; }
        public string Url { get; set; }
    }
}
