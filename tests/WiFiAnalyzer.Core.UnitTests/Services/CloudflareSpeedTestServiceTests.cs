using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WiFiAnalyzer.Core.Models;
using WiFiAnalyzer.Core.Services.SpeedTest;
using WiFiAnalyzer.Core.UnitTests.Fakes;
using Xunit;

namespace WiFiAnalyzer.Core.UnitTests.Services;

public class CloudflareSpeedTestServiceTests
{
    readonly FakeHttpMessageHandler _handler = new();

    CloudflareSpeedTestService CreateService()
        => new(new HttpClient(_handler), TimeSpan.FromMilliseconds(300), TimeSpan.Zero);

    [Fact]
    public async Task GetDownloadSpeed_RepeatsParallelCloudflareDownloadsUntilTimeout()
    {
        _handler.BytesPerResponse = 1_000_000;

        DownloadSpeed speed = await CreateService().GetDownloadSpeedAsync();

        Assert.True(speed.MegabitsPerSecond > 0);
        Assert.True(_handler.RequestedUris.Count >= 4);
        Assert.All(_handler.RequestedUris, uri => Assert.StartsWith(CloudflareSpeedTestService.DownloadUrl, uri.ToString()));
    }

    [Fact]
    public async Task GetDownloadSpeed_ThrowsWhenNoData()
        => await Assert.ThrowsAsync<InvalidOperationException>(() => CreateService().GetDownloadSpeedAsync());

    [Fact]
    public async Task GetDownloadSpeed_ThrowsOnHttpError()
    {
        _handler.StatusCode = HttpStatusCode.ServiceUnavailable;

        await Assert.ThrowsAsync<HttpRequestException>(() => CreateService().GetDownloadSpeedAsync());
    }
}
