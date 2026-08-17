using System.Net;
using System.Net.Mail;

namespace StockQuoteAlert.Notifications;

public class SmtpNotifier : INotifier
{
    private readonly SmtpOptions _options;

    public SmtpNotifier(SmtpOptions options)
    {
        _options = options;
    }

    public async Task SendAsync(Alert alert, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl
        };

        if (!string.IsNullOrWhiteSpace(_options.User))
        {
            client.Credentials = new NetworkCredential(_options.User, _options.Password);
        }

        using var message = new MailMessage(
            _options.From, _options.To, AlertEmailFormatter.Subject(alert), AlertEmailFormatter.Body(alert));

        await client.SendMailAsync(message, cancellationToken);
    }
}
