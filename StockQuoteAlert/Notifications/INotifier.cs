namespace StockQuoteAlert.Notifications;

public interface INotifier
{
    Task SendAsync(string subject, string body, CancellationToken cancellationToken);
}
