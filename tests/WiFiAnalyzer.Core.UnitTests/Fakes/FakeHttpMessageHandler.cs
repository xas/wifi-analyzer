using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WiFiAnalyzer.Core.UnitTests.Fakes;

public class FakeHttpMessageHandler : HttpMessageHandler
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

    public int BytesPerResponse { get; set; }

    public ConcurrentBag<Uri> RequestedUris { get; } = [];

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        RequestedUris.Add(request.RequestUri);
        return Task.FromResult(new HttpResponseMessage(StatusCode) { Content = new ByteArrayContent(new byte[BytesPerResponse]) });
    }
}
