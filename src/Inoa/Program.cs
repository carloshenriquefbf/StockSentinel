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
            IMailService mailService = new MailService();

            var stockQuote = await apiService.GetStockQuoteAsync(ticker);

            if (stockQuote != null)
            {
                string subject = $"Stock Quote for {stockQuote.Symbol}";
                string body = $"The current price of {stockQuote.Symbol} is {stockQuote.Price:C}.";
                await mailService.SendEmailAsync(subject, body);
                Console.WriteLine("Email sent successfully.");
            }
            else
            {
                throw new InvalidOperationException($"Could not retrieve stock quote for {ticker}.");
            }
        }
    }
}