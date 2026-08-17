using System.Globalization;

namespace StockQuoteAlert.Notifications;

public static class AlertEmailFormatter
{
    private static readonly CultureInfo PtBr = new("pt-BR");

    public static string Subject(Alert alert) => alert.Advice == TradeAdvice.Sell
        ? $"Hora de vender {alert.Ticker}"
        : $"Hora de comprar {alert.Ticker}";

    public static string Body(Alert alert) => alert.Advice == TradeAdvice.Sell
        ? $"{alert.Ticker} está cotado a {Money(alert.Price)}, no seu preço de venda ({Money(alert.SellPrice)}) ou acima dele."
        : $"{alert.Ticker} está cotado a {Money(alert.Price)}, no seu preço de compra ({Money(alert.BuyPrice)}) ou abaixo dele.";

    private static string Money(decimal value) => value.ToString("C", PtBr);
}
