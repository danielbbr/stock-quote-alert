namespace StockQuoteAlert.Quotes;

public class BrapiOptions
{
    public const string SectionName = "Brapi";

    public string BaseUrl { get; set; } = "https://brapi.dev/api";

    public string Token { get; set; } = "";
}
