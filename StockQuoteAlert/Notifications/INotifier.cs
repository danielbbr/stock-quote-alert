namespace StockQuoteAlert.Notifications;

public interface INotifier
{
    Task SendAsync(Alert alert, CancellationToken cancellationToken);
}
