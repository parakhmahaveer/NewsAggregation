using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.API.ServiceDTOs.ResponseDTOs
{
    public class NewsResponse
    {
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
        public string Source { get; set; }
        public string Category { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
    }
}
