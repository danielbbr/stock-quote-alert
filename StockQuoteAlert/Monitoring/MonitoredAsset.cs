namespace StockQuoteAlert.Monitoring;

public record MonitoredAsset(string Ticker, decimal SellPrice, decimal BuyPrice);
