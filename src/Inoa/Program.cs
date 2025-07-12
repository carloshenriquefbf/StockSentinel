using DotNetEnv;
using Inoa.Interfaces;
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

            IApiService apiService = new ApiService();

            await apiService.GetStockQuoteAsync(ticker);
        }
    }
}
