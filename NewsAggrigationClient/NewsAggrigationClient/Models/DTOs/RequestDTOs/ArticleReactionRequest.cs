using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Models.DTOs.RequestDTOs
{
    public class ArticleReactionRequest
    {
        public int UserId { get; set; } = 0;
        public int ArticleId { get; set; }
        public bool IsLiked { get; set; }
    }
}
