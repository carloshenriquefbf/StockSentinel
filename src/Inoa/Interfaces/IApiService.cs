using Inoa.Models;

namespace Inoa.Interfaces;

public interface IApiService
{
    public Task<StockQuote?> GetStockQuoteAsync(string ticker);
}