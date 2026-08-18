using System.Globalization;

namespace StockQuoteAlert.CLI;

public record CommandLineArgs(string Ticker, decimal SellPrice, decimal BuyPrice);

public static class CommandLineParser
{
    public static CommandLineResult Parse(string[] args)
    {
        if (args.Length != 3)
        {
            return CommandLineResult.Failure($"Esperado 3 argumentos, recebido {args.Length}.");
        }

        var ticker = args[0].Trim().ToUpperInvariant();

        if (ticker.Length == 0)
        {
            return CommandLineResult.Failure("O ativo não pode ser vazio.");
        }

        if (!TryParsePrice(args[1], "Preço de venda", out var sellPrice, out var error))
        {
            return CommandLineResult.Failure(error);
        }

        if (!TryParsePrice(args[2], "Preço de compra", out var buyPrice, out error))
        {
            return CommandLineResult.Failure(error);
        }

        if (buyPrice >= sellPrice)
        {
            return CommandLineResult.Failure(
                $"O preço de compra ({buyPrice}) deve ser menor que o preço de venda ({sellPrice}).");
        }

        return CommandLineResult.Success(new CommandLineArgs(ticker, sellPrice, buyPrice));
    }

    private static bool TryParsePrice(string value, string label, out decimal price, out string error)
    {
        price = 0;
        error = "";

        // Assumindo '.' como separador decimal
        var styles = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;
        if (!decimal.TryParse(value.Trim(), styles, CultureInfo.InvariantCulture, out price))
        {
            error = $"{label} \"{value}\" não é um número válido. Use '.' como separador decimal, ex: 10.50.";
            return false;
        }

        if (price <= 0)
        {
            error = $"{label} deve ser maior que zero.";
            return false;
        }

        return true;
    }
}

public record CommandLineResult
{
    private CommandLineResult(CommandLineArgs? args, string? error)
    {
        Args = args;
        Error = error;
    }

    public CommandLineArgs? Args { get; }

    public string? Error { get; }

    public bool IsSuccess => Error is null;

    public static CommandLineResult Success(CommandLineArgs args) => new(args, null);
    public static CommandLineResult Failure(string error) => new(null, error);
}
