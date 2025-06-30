using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.DAL.Models
{
    public class KeywordNotificationSetting
    {
        [Key]
        public int KeywordNotificationSettingId { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsDeleted { get; set; }

        public int KeywordId { get; set; }
        public Keyword Keyword { get; set; }
    }
}
