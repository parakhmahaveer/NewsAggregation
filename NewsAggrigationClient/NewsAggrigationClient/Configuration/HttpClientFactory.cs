using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Configuration
{
    public static class HttpClientFactory
    {
        private static HttpClient _client;

        public static HttpClient CreateClient(string token = null)
        {
            if (_client == null)
            {
                _client = new HttpClient
                {
                    BaseAddress = new Uri("http://localhost:7822/api")
                };
            }

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            return _client;
        }
    }
}
