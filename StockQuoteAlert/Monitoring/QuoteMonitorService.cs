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
    PriceZoneClassifier classifier,
    INotifier notifier,
    IOptions<MonitoringOptions> monitoringOptions,
    ILogger<QuoteMonitorService> logger) : BackgroundService
{
    private static readonly CultureInfo PtBr = new("pt-BR");

    private readonly TimeSpan _interval = TimeSpan.FromSeconds(monitoringOptions.Value.IntervalSeconds);

    // O alerta só dispara na transição de zona, então uma sequência de cotações
    // na mesma zona avisa uma única vez.
    private PriceZone _lastNotifiedZone = PriceZone.Neutral;

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

            var zone = classifier.ZoneFor(price);

            if (zone != PriceZone.Neutral && zone != _lastNotifiedZone)
            {
                var advice = zone == PriceZone.Sell ? TradeAdvice.Sell : TradeAdvice.Buy;

                await notifier.SendAsync(
                    new Alert(advice, args.Ticker, price, args.SellPrice, args.BuyPrice), cancellationToken);

                logger.LogInformation("Alerta de {Advice} enviado.", advice);
            }

            _lastNotifiedZone = zone;
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

    private static string Money(decimal value) => value.ToString("C", PtBr);
}
