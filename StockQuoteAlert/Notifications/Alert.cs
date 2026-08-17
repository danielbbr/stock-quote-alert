namespace StockQuoteAlert.Notifications;

public enum TradeAdvice
{
    Sell,
    Buy
}

public record Alert(TradeAdvice Advice, string Ticker, decimal Price, decimal SellPrice, decimal BuyPrice);
