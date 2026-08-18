using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using StockQuoteAlert.Monitoring;
using StockQuoteAlert.Notifications;
using StockQuoteAlert.Tests.Fakes;

namespace StockQuoteAlert.Tests;

public class QuoteMonitorServiceTests
{
    private const decimal SellPrice = 22.67m;
    private const decimal BuyPrice = 22.59m;

    private static QuoteMonitorService NewService(FakeQuoteProvider quotes, FakeNotifier notifier) =>
        new(new MonitoredAsset("PETR4", SellPrice, BuyPrice),
            quotes,
            new PriceZoneClassifier(SellPrice, BuyPrice),
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

    [Fact]
    public async Task CheckQuoteAsync_WhileThePriceStaysInTheSameZone_AlertsOnlyOnce()
    {
        var notifier = new FakeNotifier();
        var service = NewService(new FakeQuoteProvider(22.80m, 22.90m), notifier);

        await service.CheckQuoteAsync(CancellationToken.None);
        await service.CheckQuoteAsync(CancellationToken.None);

        Assert.Single(notifier.Sent);
    }

    [Fact]
    public async Task CheckQuoteAsync_WhenSendingTheAlertFails_RetriesOnTheNextCycle()
    {
        var notifier = new FakeNotifier { FailNextSend = true };
        var service = NewService(new FakeQuoteProvider(22.80m, 22.85m), notifier);

        await service.CheckQuoteAsync(CancellationToken.None);
        await service.CheckQuoteAsync(CancellationToken.None);

        Assert.Equal(TradeAdvice.Sell, Assert.Single(notifier.Sent).Advice);
    }
}
