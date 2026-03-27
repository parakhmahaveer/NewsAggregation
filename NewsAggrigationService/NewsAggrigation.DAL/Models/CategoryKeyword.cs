using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.DAL.Models
{
    public class CategoryKeyword
    {
        public int CategoryKeywordId { get; set; }
        public string KeywordName { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public bool IsDeleted { get; set; }
    }
}
