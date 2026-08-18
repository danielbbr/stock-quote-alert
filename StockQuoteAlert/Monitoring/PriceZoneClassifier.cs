namespace StockQuoteAlert.Monitoring;

public enum PriceZone
{
    Neutral,
    Sell,
    Buy
}

public class PriceZoneClassifier
{
    private readonly decimal _sellPrice;
    private readonly decimal _buyPrice;

    public PriceZoneClassifier(decimal sellPrice, decimal buyPrice)
    {
        _sellPrice = sellPrice;
        _buyPrice = buyPrice;
    }

    public PriceZone ZoneFor(decimal price)
    {
        if (price >= _sellPrice)
        {
            return PriceZone.Sell;
        }

        if (price <= _buyPrice)
        {
            return PriceZone.Buy;
        }

        return PriceZone.Neutral;
    }
}
