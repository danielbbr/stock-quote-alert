using System.ComponentModel.DataAnnotations;

namespace StockQuoteAlert.Notifications;

public class SmtpOptions
{
    public const string SectionName = "Smtp";

    [Required(ErrorMessage = "Smtp:To é obrigatório: o e-mail que vai receber os alertas.")]
    [EmailAddress(ErrorMessage = "Smtp:To não é um e-mail válido.")]
    public string To { get; set; } = "";

    [Required(ErrorMessage = "Smtp:Host é obrigatório: o servidor SMTP que envia os alertas.")]
    public string Host { get; set; } = "";

    [Range(1, 65535, ErrorMessage = "Smtp:Port deve estar entre 1 e 65535.")]
    public int Port { get; set; } = 587;

    public bool EnableSsl { get; set; } = true;

    [Required(ErrorMessage = "Smtp:From é obrigatório: o e-mail remetente dos alertas.")]
    [EmailAddress(ErrorMessage = "Smtp:From não é um e-mail válido.")]
    public string From { get; set; } = "";

    public string User { get; set; } = "";

    public string Password { get; set; } = "";
}
