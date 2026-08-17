using StockQuoteAlert.Quotes;

namespace StockQuoteAlert.Tests.Fakes;


internal sealed class FakeQuoteProvider(params decimal[] prices) : IQuoteProvider
{
    private readonly Queue<decimal> _prices = new(prices);

    public Task<decimal> GetPriceAsync(string ticker, CancellationToken cancellationToken) =>
        Task.FromResult(_prices.Dequeue());
}
