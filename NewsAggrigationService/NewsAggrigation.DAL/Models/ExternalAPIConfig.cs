using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.DAL.Models
{
    public class ExternalAPIConfig
    {
        [Key]
        public int ExternalAPIId { get; set; }
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string ApiUrl { get; set; }
        public bool IsEnable { get; set; } 
        public DateTime LastAccessedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
