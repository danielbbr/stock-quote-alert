namespace StockQuoteAlert.Monitoring;

public class MonitoringOptions
{
    public const string SectionName = "Monitoring";

    public int IntervalSeconds { get; set; } = 60;
}
