using StockQuoteAlert.Notifications;

namespace StockQuoteAlert.Tests.Fakes;


internal sealed class FakeNotifier : INotifier
{
    public List<Alert> Sent { get; } = [];

    public Task SendAsync(Alert alert, CancellationToken cancellationToken)
    {
        Sent.Add(alert);
        return Task.CompletedTask;
    }
}
