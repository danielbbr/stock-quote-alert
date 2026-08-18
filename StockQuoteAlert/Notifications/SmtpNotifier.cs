using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace StockQuoteAlert.Notifications;

public class SmtpNotifier : INotifier
{
    private const int ImplicitTlsPort = 465;

    private readonly SmtpOptions _options;

    public SmtpNotifier(SmtpOptions options)
    {
        _options = options;
    }

    public async Task SendAsync(Alert alert, CancellationToken cancellationToken)
    {
        var message = new MimeMessage
        {
            Subject = AlertEmailFormatter.Subject(alert),
            Body = new TextPart("plain") { Text = AlertEmailFormatter.Body(alert) }
        };

        message.From.Add(MailboxAddress.Parse(_options.From));
        message.To.Add(MailboxAddress.Parse(_options.To));

        var security = _options.EnableSsl
            ? _options.Port == ImplicitTlsPort ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls
            : SecureSocketOptions.None;

        using var client = new SmtpClient();

        await client.ConnectAsync(_options.Host, _options.Port, security, cancellationToken);

        if (!string.IsNullOrWhiteSpace(_options.User))
        {
            await client.AuthenticateAsync(_options.User, _options.Password, cancellationToken);
        }

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(quit: true, cancellationToken);
    }
}
