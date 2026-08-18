using System.Globalization;

namespace StockQuoteAlert.Notifications;

public static class AlertEmailFormatter
{
    private static readonly CultureInfo PtBr = new("pt-BR");

    public static string Subject(Alert alert)
    {
        var recommendation = alert.Advice == TradeAdvice.Sell ? "venda" : "compra";

        return $"{alert.Ticker} atingiu {Money(alert.Price)}: recomendação de {recommendation}";
    }

    public static string Body(Alert alert)
    {
        var action = alert.Advice == TradeAdvice.Sell ? "VENDER" : "COMPRAR";

        var summary = alert.Advice == TradeAdvice.Sell
            ? $"A cotação de {alert.Ticker} atingiu {Money(alert.Price)}, valor igual ou superior ao seu preço de referência de venda, de {Money(alert.SellPrice)}."
            : $"A cotação de {alert.Ticker} atingiu {Money(alert.Price)}, valor igual ou inferior ao seu preço de referência de compra, de {Money(alert.BuyPrice)}.";

        return $"""
                Recomendação: {action} {alert.Ticker}

                {summary}

                Ativo: {alert.Ticker}
                Cotação: {Money(alert.Price)}
                Referência de venda: {Money(alert.SellPrice)}
                Referência de compra: {Money(alert.BuyPrice)}
                """;
    }

    private static string Money(decimal value) => value.ToString("C", PtBr);
}
