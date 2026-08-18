using System.Net;
using System.Text;

namespace StockQuoteAlert.Tests.Fakes;

internal sealed class FakeHttpMessageHandler(HttpStatusCode statusCode, string content = "") : HttpMessageHandler
{
    public string? RequestUrl { get; private set; }

    public string? Authorization { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        RequestUrl = request.RequestUri?.ToString();
        Authorization = request.Headers.Authorization?.ToString();

        return Task.FromResult(new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        });
    }
}
