using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.DAL.Models
{
    public class ReportedArticle
    {
        [Key]
        public int Id { get; set; }
        public int ReportCount { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; }
    }
}
