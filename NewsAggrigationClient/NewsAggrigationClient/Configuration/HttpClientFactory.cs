using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Configuration
{
    public static class HttpClientFactory
    {
        private static HttpClient _client;
        private static string _currentToken;

        public static HttpClient CreateClient(string token = null)
        {
            if (_client == null)
            {
                _client = new HttpClient
                {
                    BaseAddress = new Uri("http://localhost:7822/api")
                };
            }

            if (!string.IsNullOrEmpty(token) && token != _currentToken)
            {
                _currentToken = token;
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _currentToken);
            }

            return _client;
        }

        public static void UpdateToken(string token)
        {
            _currentToken = token;
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _currentToken);
        }

        public static string GetToken() => _currentToken;
    }
}
