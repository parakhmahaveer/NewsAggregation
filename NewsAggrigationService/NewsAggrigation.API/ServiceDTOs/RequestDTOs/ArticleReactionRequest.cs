using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.API.ServiceDTOs.RequestDTOs
{
    public class ArticleReactionRequest
    {
        public int ArticleId { get; set; }
        public string Username { get; set; }
        public bool IsLiked { get; set; }
    }
}
