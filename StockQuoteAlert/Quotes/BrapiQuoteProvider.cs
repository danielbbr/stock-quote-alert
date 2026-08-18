using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace StockQuoteAlert.Quotes;

public class BrapiQuoteProvider : IQuoteProvider
{
    private readonly HttpClient _httpClient;
    private readonly BrapiOptions _options;

    public BrapiQuoteProvider(HttpClient httpClient, BrapiOptions options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<decimal> GetPriceAsync(string ticker, CancellationToken cancellationToken)
    {
        var url = $"{_options.BaseUrl.TrimEnd('/')}/v2/stocks/quote?symbols={Uri.EscapeDataString(ticker)}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.Token);

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            throw new HttpRequestException("Token da Brapi inválido ou sem permissão para esta consulta.");
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new HttpRequestException($"Ativo \"{ticker}\" não encontrado na Brapi.");
        }

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<QuoteResponse>(cancellationToken);

        return payload?.Results?.FirstOrDefault()?.Data?.RegularMarketPrice
               ?? throw new InvalidOperationException($"A Brapi não retornou cotação para o ativo \"{ticker}\".");
    }

    private sealed record QuoteResponse(
        [property: JsonPropertyName("results")] IReadOnlyList<QuoteResult>? Results);

    private sealed record QuoteResult(
        [property: JsonPropertyName("data")] QuoteData? Data);

    private sealed record QuoteData(
        [property: JsonPropertyName("regularMarketPrice")] decimal? RegularMarketPrice);
}
