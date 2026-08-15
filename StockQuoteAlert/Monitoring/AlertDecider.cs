namespace StockQuoteAlert.Monitoring;

public enum PriceZone
{
    Neutral,
    Sell,
    Buy
}

public class AlertDecider
{
    private readonly decimal _sellPrice;
    private readonly decimal _buyPrice;

    public AlertDecider(decimal sellPrice, decimal buyPrice)
    {
        _sellPrice = sellPrice;
        _buyPrice = buyPrice;
    }

    public PriceZone CurrentZone { get; private set; } = PriceZone.Neutral;

    public bool ShouldAlert(decimal price)
    {
        var zone = ZoneFor(price);
        if (zone == CurrentZone)
            return false;

        CurrentZone = zone;
        return zone != PriceZone.Neutral;
    }

    private PriceZone ZoneFor(decimal price)
    {
        if (price >= _sellPrice)
        {
            return PriceZone.Sell;
        }

        else if (price <= _buyPrice)
        {
            return PriceZone.Buy;
        }
        return PriceZone.Neutral;
    }
}
