using System.ComponentModel.DataAnnotations;

namespace StockQuoteAlert.Monitoring;

public class MonitoringOptions
{
    public const string SectionName = "Monitoring";

    [Range(1, int.MaxValue, ErrorMessage = "Monitoring:IntervalSeconds deve ser maior que zero.")]
    public int IntervalSeconds { get; set; } = 60;
}
