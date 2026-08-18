using System.ComponentModel.DataAnnotations;

namespace StockQuoteAlert.Quotes;

public class BrapiOptions
{
    public const string SectionName = "Brapi";

    [Required(ErrorMessage = "Brapi:BaseUrl é obrigatório.")]
    [Url(ErrorMessage = "Brapi:BaseUrl não é uma URL válida.")]
    public string BaseUrl { get; set; } = "https://brapi.dev/api";

    [Required(ErrorMessage = "Brapi:Token é obrigatório: gere um token gratuito em https://brapi.dev.")]
    public string Token { get; set; } = "";
}
