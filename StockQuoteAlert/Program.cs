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

var commandLineArgs = parseResult.Args!;

var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    ContentRootPath = AppContext.BaseDirectory
});

builder.Configuration.AddUserSecrets<Program>(optional: true);

builder.Services.Configure<BrapiOptions>(builder.Configuration.GetSection(BrapiOptions.SectionName));
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.SectionName));
builder.Services.Configure<MonitoringOptions>(builder.Configuration.GetSection(MonitoringOptions.SectionName));

builder.Services.AddSingleton(commandLineArgs);

builder.Services.AddSingleton(new PriceZoneClassifier(commandLineArgs.SellPrice, commandLineArgs.BuyPrice));

builder.Services.AddSingleton<INotifier>(sp =>
    new SmtpNotifier(sp.GetRequiredService<IOptions<SmtpOptions>>().Value));

builder.Services.AddHttpClient<IQuoteProvider, BrapiQuoteProvider>((httpClient, sp) =>
    new BrapiQuoteProvider(httpClient, sp.GetRequiredService<IOptions<BrapiOptions>>().Value));

builder.Services.AddHostedService<QuoteMonitorService>();

var host = builder.Build();
await host.RunAsync();

return 0;
