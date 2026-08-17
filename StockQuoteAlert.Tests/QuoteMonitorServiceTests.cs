using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using StockQuoteAlert.CLI;
using StockQuoteAlert.Monitoring;
using StockQuoteAlert.Notifications;
using StockQuoteAlert.Tests.Fakes;

namespace StockQuoteAlert.Tests;

public class QuoteMonitorServiceTests
{
    private const decimal SellPrice = 22.67m;
    private const decimal BuyPrice = 22.59m;

    private static QuoteMonitorService NewService(FakeQuoteProvider quotes, FakeNotifier notifier) =>
        new(new CommandLineArgs("PETR4", SellPrice, BuyPrice),
            quotes,
            new AlertDecider(SellPrice, BuyPrice),
            notifier,
            Options.Create(new MonitoringOptions()),
            NullLogger<QuoteMonitorService>.Instance);

    [Fact]
    public async Task CheckQuoteAsync_WhenPriceReachesSellPrice_SendsSellAlert()
    {
        var notifier = new FakeNotifier();

        await NewService(new FakeQuoteProvider(22.80m), notifier).CheckQuoteAsync(CancellationToken.None);

        Assert.Equal(TradeAdvice.Sell, Assert.Single(notifier.Sent).Advice);
    }

    [Fact]
    public async Task CheckQuoteAsync_WhenPriceReachesBuyPrice_SendsBuyAlert()
    {
        var notifier = new FakeNotifier();

        await NewService(new FakeQuoteProvider(22.50m), notifier).CheckQuoteAsync(CancellationToken.None);

        Assert.Equal(TradeAdvice.Buy, Assert.Single(notifier.Sent).Advice);
    }

    [Fact]
    public async Task CheckQuoteAsync_WhenPriceIsInTheNeutralZone_SendsNothing()
    {
        var notifier = new FakeNotifier();

        await NewService(new FakeQuoteProvider(22.62m), notifier).CheckQuoteAsync(CancellationToken.None);

        Assert.Empty(notifier.Sent);
    }
}
