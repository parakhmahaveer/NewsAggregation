using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Configuration
{
    public static class SessionContext
    {
        public static string? JwtToken { get; set; }
        public static string? Username { get; set; }
        public static string? Role { get; set; }
    }
}
