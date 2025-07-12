using System.Text.Json;

using StockSentinel.Interfaces;
using StockSentinel.Models;

namespace StockSentinel.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public ApiService()
    {
        _httpClient = new HttpClient();
        _apiKey = Environment.GetEnvironmentVariable("API_KEY")
                  ?? throw new InvalidOperationException("API_KEY not set.");
    }

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = Environment.GetEnvironmentVariable("API_KEY")
                  ?? throw new InvalidOperationException("API_KEY not set.");
    }

    public async Task<StockQuote?> GetStockQuoteAsync(string ticker)
    {
        string endpoint = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={ticker}&apikey={_apiKey}";

        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        var globalQuote = json.RootElement.GetProperty("Global Quote");

        string? symbol = globalQuote.GetProperty("01. symbol").GetString();
        string? priceStr = globalQuote.GetProperty("05. price").GetString();

        if (string.IsNullOrEmpty(symbol) || string.IsNullOrEmpty(priceStr))
        {
            throw new InvalidOperationException($"Invalid response for ticker: {ticker}");
        }

        decimal price;

        if (!decimal.TryParse(priceStr, out price))
        {
            throw new InvalidOperationException($"Invalid price format for ticker: {ticker}");
        }

        return new StockQuote
        {
            Symbol = symbol,
            Price = price
        };
    }
}