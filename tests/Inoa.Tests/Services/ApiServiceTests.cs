using System.Net;

using Inoa.Models;
using Inoa.Services;

using Moq;
using Moq.Protected;

using Xunit;

namespace Inoa.Tests;

public class InoaTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly ApiService _apiService;

    public InoaTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);

        Environment.SetEnvironmentVariable("API_KEY", "test-api-key");

        _apiService = new ApiService(_httpClient);
    }

    [Fact]
    public void Constructor_WithoutApiKey_ThrowsInvalidOperationException()
    {
        Environment.SetEnvironmentVariable("API_KEY", null);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            new ApiService(new HttpClient()));

        Assert.Equal("API_KEY not set.", exception.Message);

        Environment.SetEnvironmentVariable("API_KEY", "test-api-key");
    }

    [Fact]
    public async Task GetStockQuoteAsync_ValidResponse_ReturnsStockQuote()
    {
        string symbol = "PETR4.SA";
        string price = "150.00";

        SetupHttpResponse(HttpStatusCode.OK, price, symbol);

        var result = await _apiService.GetStockQuoteAsync(symbol);

        var expectedQuote = new StockQuote
        {
            Symbol = symbol,
            Price = decimal.Parse(price)
        };

        Assert.NotNull(result);
        Assert.Equal(expectedQuote.Symbol, result.Symbol);
        Assert.Equal(expectedQuote.Price, result.Price);
    }

    [Fact]
    public async Task GetStockQuoteAsync_InvalidPriceResponse_ThrowsInvalidOperationException()
    {
        string symbol = "PETR4.SA";
        string price = "";

        SetupHttpResponse(HttpStatusCode.OK, price, symbol);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _apiService.GetStockQuoteAsync(symbol));

        Assert.Equal($"Invalid response for ticker: {symbol}", exception.Message);
    }

    [Fact]
    public async Task GetStockQuoteAsync_InvalidSymbolResponse_ThrowsInvalidOperationException()
    {
        string symbol = "";
        string price = "150.00";

        SetupHttpResponse(HttpStatusCode.OK, price, symbol);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _apiService.GetStockQuoteAsync(symbol));

        Assert.Equal($"Invalid response for ticker: {symbol}", exception.Message);
    }

    [Fact]
    public async Task GetStockQuoteAsync_InvalidPriceFormat_ThrowsInvalidOperationException()
    {
        string symbol = "PETR4.SA";
        string price = "invalid-price";

        SetupHttpResponse(HttpStatusCode.OK, price, symbol);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _apiService.GetStockQuoteAsync(symbol));

        Assert.Equal($"Invalid price format for ticker: {symbol}", exception.Message);
    }

    private void SetupHttpResponse(HttpStatusCode statusCode, string? price, string? symbol)
    {
        var responseContent = new Dictionary<string, object>
        {
            ["Global Quote"] = new Dictionary<string, string>
            {
                ["01. symbol"] = symbol ?? "",
                ["05. price"] = price ?? "",
            }
        };

        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(responseContent);

        var response = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response)
            .Verifiable();
    }
}