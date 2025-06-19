using NewsAggrigationClient.Configuration;
using NewsAggrigationClient.Services;
using NewsAggrigationClient.Services.Interfaces;
using NewsAggrigationClient.Views;

namespace NewsAggrigationClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var httpClient = HttpClientFactory.CreateClient();
            IAuthService authService = new AuthService(httpClient);
            var menu = new MenuService(authService);

            menu.ShowMainMenuAsync().GetAwaiter().GetResult();
        }
    }
}
