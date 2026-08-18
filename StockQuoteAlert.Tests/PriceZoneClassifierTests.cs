using StockQuoteAlert.Monitoring;

namespace StockQuoteAlert.Tests;

public class PriceZoneClassifierTests
{
    [Theory]
    [InlineData(22.60, PriceZone.Neutral)]
    [InlineData(22.67, PriceZone.Sell)]
    [InlineData(22.59, PriceZone.Buy)]
    public void ZoneFor_ClassifiesThePrice(decimal price, PriceZone expectedZone)
    {
        var classifier = new PriceZoneClassifier(sellPrice: 22.67m, buyPrice: 22.59m);

        Assert.Equal(expectedZone, classifier.ZoneFor(price));
    }
}
