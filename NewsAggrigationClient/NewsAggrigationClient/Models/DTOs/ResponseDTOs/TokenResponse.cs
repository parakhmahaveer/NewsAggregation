using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Models.DTOs.ResponseDTOs
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public string Username { get; set; }
    }
}
