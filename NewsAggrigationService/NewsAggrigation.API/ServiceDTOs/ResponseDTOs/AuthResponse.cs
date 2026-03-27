using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.API.ServiceDTOs.ResponseDTOs
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public string Username { get; set; }
    }
}
