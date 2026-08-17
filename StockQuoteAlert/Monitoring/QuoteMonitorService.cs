using System.Globalization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StockQuoteAlert.CLI;
using StockQuoteAlert.Notifications;
using StockQuoteAlert.Quotes;

namespace StockQuoteAlert.Monitoring;

public class QuoteMonitorService(
    CommandLineArgs args,
    IQuoteProvider quoteProvider,
    AlertDecider alertDecider,
    INotifier notifier,
    IOptions<MonitoringOptions> monitoringOptions,
    ILogger<QuoteMonitorService> logger) : BackgroundService
{
    private static readonly CultureInfo PtBr = new("pt-BR");

    private readonly TimeSpan _interval = TimeSpan.FromSeconds(monitoringOptions.Value.IntervalSeconds);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "Monitorando {Ticker} a cada {Interval}s. Venda a partir de {SellPrice}, compra a partir de {BuyPrice}.",
            args.Ticker, _interval.TotalSeconds, Money(args.SellPrice), Money(args.BuyPrice));

        using var timer = new PeriodicTimer(_interval);

        try
        {
            do
            {
                await CheckQuoteAsync(stoppingToken);
            } while (await timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException)
        {
            // Encerramento normal
        }
        
        logger.LogInformation("Monitoramento encerrado.");
    }

    public async Task CheckQuoteAsync(CancellationToken cancellationToken)
    {
        try
        {
            var price = await quoteProvider.GetPriceAsync(args.Ticker, cancellationToken);
            logger.LogInformation("{Ticker} cotado a {Price}.", args.Ticker, Money(price));

            if (!alertDecider.ShouldAlert(price))
            {
                return;
            }

            var zone = alertDecider.CurrentZone;
            await notifier.SendAsync(Subject(zone), Body(zone, price), cancellationToken);

            logger.LogInformation("Alerta de {Zone} enviado.", zone);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha no ciclo de monitoramento de {Ticker}.", args.Ticker);
        }
    }

    private string Subject(PriceZone zone) => zone == PriceZone.Sell
        ? $"Hora de vender {args.Ticker}"
        : $"Hora de comprar {args.Ticker}";

    private string Body(PriceZone zone, decimal price) => zone == PriceZone.Sell
        ? $"{args.Ticker} está cotado a {Money(price)}, no seu preço de venda ({Money(args.SellPrice)}) ou acima dele."
        : $"{args.Ticker} está cotado a {Money(price)}, no seu preço de compra ({Money(args.BuyPrice)}) ou abaixo dele.";

    private static string Money(decimal value) => value.ToString("C", PtBr);
}
