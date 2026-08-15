namespace StockQuoteAlert.Quotes;

public interface IQuoteProvider
{
    Task<decimal> GetPriceAsync(string ticker, CancellationToken cancellationToken);
}
