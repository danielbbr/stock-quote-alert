using StockQuoteAlert.Notifications;

namespace StockQuoteAlert.Tests.Fakes;


internal sealed class FakeNotifier : INotifier
{
    public List<Alert> Sent { get; } = [];

    public bool FailNextSend { get; set; }

    public Task SendAsync(Alert alert, CancellationToken cancellationToken)
    {
        if (FailNextSend)
        {
            FailNextSend = false;
            throw new InvalidOperationException("falha simulada de SMTP");
        }

        Sent.Add(alert);
        return Task.CompletedTask;
    }
}
