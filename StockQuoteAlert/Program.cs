using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StockQuoteAlert.CLI;
using StockQuoteAlert.Monitoring;
using StockQuoteAlert.Notifications;
using StockQuoteAlert.Quotes;

var parseResult = CommandLineParser.Parse(args);
if (!parseResult.IsSuccess)
{
    Console.Error.WriteLine(parseResult.Error);
    return 1;
}

var monitoredAsset = parseResult.Asset!;

var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    ContentRootPath = AppContext.BaseDirectory
});

builder.Configuration.AddUserSecrets<Program>(optional: true);

AddValidatedOptions<BrapiOptions>(BrapiOptions.SectionName);
AddValidatedOptions<SmtpOptions>(SmtpOptions.SectionName);
AddValidatedOptions<MonitoringOptions>(MonitoringOptions.SectionName);

void AddValidatedOptions<TOptions>(string sectionName) where TOptions : class =>
    builder.Services.AddOptions<TOptions>()
        .Bind(builder.Configuration.GetSection(sectionName))
        .ValidateDataAnnotations()
        .ValidateOnStart();

builder.Services.AddSingleton(monitoredAsset);

builder.Services.AddSingleton(new PriceZoneClassifier(monitoredAsset.SellPrice, monitoredAsset.BuyPrice));

builder.Services.AddSingleton<INotifier>(sp =>
    new SmtpNotifier(sp.GetRequiredService<IOptions<SmtpOptions>>().Value));

builder.Services.AddHttpClient<IQuoteProvider, BrapiQuoteProvider>((httpClient, sp) =>
        new BrapiQuoteProvider(httpClient, sp.GetRequiredService<IOptions<BrapiOptions>>().Value))
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(5)
    })
    .SetHandlerLifetime(Timeout.InfiniteTimeSpan);

builder.Services.AddHostedService<QuoteMonitorService>();

var host = builder.Build();

try
{
    host.Services.GetRequiredService<IStartupValidator>().Validate();
}
catch (OptionsValidationException ex)
{
    Console.Error.WriteLine("Configuração inválida:");

    foreach (var failure in ex.Failures)
    {
        Console.Error.WriteLine($"  - {failure}");
    }

    return 1;
}

await host.RunAsync();

return 0;
