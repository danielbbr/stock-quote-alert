using System.Net;
using StockQuoteAlert.Quotes;
using StockQuoteAlert.Tests.Fakes;

namespace StockQuoteAlert.Tests;

public class BrapiQuoteProviderTests
{
    private const string Payload = """{"results":[{"data":{"regularMarketPrice":22.67}}]}""";

    private static BrapiQuoteProvider NewProvider(FakeHttpMessageHandler handler) =>
        new(new HttpClient(handler),
            new BrapiOptions { BaseUrl = "https://brapi.dev/api/", Token = "token-de-teste" });

    [Fact]
    public async Task GetPriceAsync_ReturnsTheQuotedPrice()
    {
        var provider = NewProvider(new FakeHttpMessageHandler(HttpStatusCode.OK, Payload));

        var price = await provider.GetPriceAsync("PETR4", CancellationToken.None);

        Assert.Equal(22.67m, price);
    }

    [Fact]
    public async Task GetPriceAsync_RequestsTheTickerWithTheConfiguredToken()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, Payload);

        await NewProvider(handler).GetPriceAsync("PETR4", CancellationToken.None);

        Assert.Equal("https://brapi.dev/api/v2/stocks/quote?symbols=PETR4", handler.RequestUrl);
        Assert.Equal("Bearer token-de-teste", handler.Authorization);
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.NotFound)]
    public async Task GetPriceAsync_WhenTheApiFails_Throws(HttpStatusCode statusCode)
    {
        var provider = NewProvider(new FakeHttpMessageHandler(statusCode));

        await Assert.ThrowsAsync<HttpRequestException>(
            () => provider.GetPriceAsync("PETR4", CancellationToken.None));
    }

    [Fact]
    public async Task GetPriceAsync_WhenTheResponseHasNoPrice_Throws()
    {
        var provider = NewProvider(new FakeHttpMessageHandler(HttpStatusCode.OK, """{"results":[]}"""));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => provider.GetPriceAsync("PETR4", CancellationToken.None));
    }
}
