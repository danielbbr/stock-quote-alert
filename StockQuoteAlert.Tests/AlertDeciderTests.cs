using StockQuoteAlert.Monitoring;

namespace StockQuoteAlert.Tests;

public class AlertDeciderTests
{
    private static AlertDecider NewDecider() => new(sellPrice: 22.67m, buyPrice: 22.59m);

    [Theory]
    [InlineData(22.60, PriceZone.Neutral)]
    [InlineData(22.67, PriceZone.Sell)]
    [InlineData(22.59, PriceZone.Buy)]
    public void ShouldAlert_ClassifiesThePriceZone(decimal price, PriceZone expectedZone)
    {
        var decider = NewDecider();

        var alerted = decider.ShouldAlert(price);

        Assert.Equal(expectedZone, decider.CurrentZone);
        Assert.Equal(expectedZone != PriceZone.Neutral, alerted);
    }

    [Fact]
    public void ShouldAlert_WhenZoneDoesNotChange_AlertsOnlyOnce()
    {
        var decider = NewDecider();

        Assert.True(decider.ShouldAlert(23.00m));
        Assert.False(decider.ShouldAlert(23.50m));
        Assert.False(decider.ShouldAlert(22.67m));
    }
}
