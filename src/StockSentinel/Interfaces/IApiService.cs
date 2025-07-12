using StockSentinel.Models;

namespace StockSentinel.Interfaces;

public interface IApiService
{
    public Task<StockQuote?> GetStockQuoteAsync(string ticker);
}