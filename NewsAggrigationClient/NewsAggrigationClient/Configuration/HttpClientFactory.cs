using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Configuration
{
    public class HttpClientFactory
    {
        public static HttpClient CreateClient()
        {
            return new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44313/api")
            };
        }
    }
}
