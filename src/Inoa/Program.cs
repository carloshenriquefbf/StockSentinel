using DotNetEnv;
using Inoa.Services;

namespace Inoa
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Env.Load();

            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a stock ticker symbol as an argument.");
                return;
            }

            var ticker = args[0];

            var httpClient = new HttpClient();
            var ApiService = new ApiService(httpClient);

            await ApiService.GetStockQuoteAsync(ticker);
        }
    }
}
