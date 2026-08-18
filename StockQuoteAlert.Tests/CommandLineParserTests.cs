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
        Assert.NotNull(result.Asset);
        Assert.Equal(ticker, result.Asset.Ticker);
        Assert.Equal(22.67m, result.Asset.SellPrice);
        Assert.Equal(22.59m, result.Asset.BuyPrice);
    }

    [Fact]
    public void Parse_NormalizesTheTicker()
    {
        var result = CommandLineParser.Parse([" petr4 ", "22.67", "22.59"]);

        Assert.True(result.IsSuccess);
        Assert.Equal("PETR4", result.Asset!.Ticker);
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
        Assert.Null(result.Asset);
        Assert.NotNull(result.Error);
    }
}
