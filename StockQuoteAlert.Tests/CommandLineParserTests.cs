using StockQuoteAlert.CLI;

namespace StockQuoteAlert.Tests;

public class CommandLineParserTests
{
    [Fact]
    public void Parse_WithValidArguments_ReturnsSuccess()
    {
        const string ticker = "PETR4";

        var result = CommandLineParser.Parse([ticker, "22.67", "22.59"]);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Error);
        Assert.NotNull(result.Args);
        Assert.Equal(ticker, result.Args.Ticker);
        Assert.Equal(22.67m, result.Args.SellPrice);
        Assert.Equal(22.59m, result.Args.BuyPrice);
    }

    [Fact]
    public void Parse_WithWrongArgumentCount_ReturnsError()
    {
        AssertFailure(CommandLineParser.Parse(["PETR4", "22.67"]));
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("0")]
    public void Parse_WithInvalidPrice_ReturnsError(string sellPrice)
    {
        AssertFailure(CommandLineParser.Parse(["PETR4", sellPrice, "1"]));
    }

    [Fact]
    public void Parse_WhenBuyPriceIsNotLowerThanSellPrice_ReturnsError()
    {
        AssertFailure(CommandLineParser.Parse(["PETR4", "22.59", "22.67"]));
    }

    private static void AssertFailure(CommandLineResult result)
    {
        Assert.False(result.IsSuccess);
        Assert.Null(result.Args);
        Assert.NotNull(result.Error);
    }
}
