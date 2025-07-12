using DotNetEnv;

using StockSentinel.Interfaces;
using StockSentinel.Models;
using StockSentinel.Services;

namespace StockSentinel
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Env.Load();

            if (args.Length != 3)
            {
                Console.WriteLine("Invalid number of arguments.");
                Console.WriteLine("Usage: <ticker> <sellPrice> <buyPrice>");
                Console.WriteLine("Example: PETR4.SA 22.67 22.59");
                return;
            }

            if (string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine("Ticker symbol cannot be empty.");
                return;
            }

            string ticker = args[0].ToUpperInvariant();

            if (!decimal.TryParse(args[1], out decimal sellPrice))
            {
                Console.WriteLine("Please provide a valid sell price as the second argument.");
                return;
            }

            if (!decimal.TryParse(args[2], out decimal buyPrice))
            {
                Console.WriteLine("Please provide a valid buy price as the third argument.");
                return;
            }

            if (buyPrice >= sellPrice)
            {
                Console.WriteLine("Buy price must be less than sell price.");
                return;
            }

            if (buyPrice < 0 || sellPrice < 0)
            {
                Console.WriteLine("Prices must be non-negative.");
                return;
            }

            IApiService apiService = new ApiService();
            IMailService mailService = new MailService();

            int delayTimeSpan = 5;

            while (true)
            {
                Console.WriteLine($"[Requesting] Ticker: {ticker}...");
                StockQuote? stockQuote = await apiService.GetStockQuoteAsync(ticker);

                if (stockQuote == null)
                {
                    throw new Exception($"Failed to retrieve stock quote for {ticker}.");
                }

                decimal currentPrice = stockQuote.Price;

                if (currentPrice <= buyPrice)
                {
                    Console.WriteLine($"[Sending Email] Current price of {ticker} is at or below buy price: {currentPrice:F2} <= {buyPrice:F2}");
                    string subject = $"Stock Alert: {ticker} Buy Price Alert";
                    string body = $"The current price of {ticker} is {currentPrice:F2}. It is at or below your buy price of {buyPrice:F2}. Consider buying.";
                    await mailService.SendEmailAsync(subject, body);
                    Console.WriteLine($"[Email Sent] Subject: {subject}");
                }
                else if (currentPrice >= sellPrice)
                {
                    Console.WriteLine($"[Sending Email] Current price of {ticker} is at or above sell price: {currentPrice:F2} >= {sellPrice:F2}");
                    string subject = $"Stock Alert: {ticker} Sell Price Alert";
                    string body = $"The current price of {ticker} is {currentPrice:F2}. It is at or above your sell price of {sellPrice:F2}. Consider selling.";
                    await mailService.SendEmailAsync(subject, body);
                    Console.WriteLine($"[Email Sent] Subject: {subject}");
                }
                else
                {
                    Console.WriteLine($"[No Action] Current price of {ticker} is {currentPrice:F2}, within range ({buyPrice:F2} - {sellPrice:F2}).");
                }

                Console.WriteLine($"[Waiting] Next check in {delayTimeSpan} minutes...");

                await Task.Delay(TimeSpan.FromMinutes(delayTimeSpan));
            }
        }
    }
}