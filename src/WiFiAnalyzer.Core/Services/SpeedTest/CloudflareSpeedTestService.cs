using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WiFiAnalyzer.Core.Models;

namespace WiFiAnalyzer.Core.Services.SpeedTest;

public sealed class CloudflareSpeedTestService : ISpeedTestService
{
    public const string DownloadUrl = "https://speed.cloudflare.com/__down?bytes=";

    const long BytesPerRequest = 5_000_000;
    const int ParallelRequests = 4;
    const int BufferSize = 81_920;

    readonly HttpClient _httpClient;
    readonly TimeSpan _duration;
    readonly TimeSpan _warmUp;

    public CloudflareSpeedTestService(HttpClient httpClient)
        : this(httpClient, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(2))
    {
    }

    public CloudflareSpeedTestService(HttpClient httpClient, TimeSpan duration, TimeSpan warmUp)
        => (_httpClient, _duration, _warmUp) = (httpClient, duration, warmUp);

    public async Task<DownloadSpeed> GetDownloadSpeedAsync()
    {
        using CancellationTokenSource timeout = new(_duration);
        Measurement measurement = new(_warmUp);

        try
        {
            await Task.WhenAll(Enumerable.Range(0, ParallelRequests).Select(_ => DownloadAsync(measurement, timeout.Token)));
        }
        catch (OperationCanceledException) when (timeout.IsCancellationRequested)
        {
        }

        return new DownloadSpeed(measurement.MegabitsPerSecond());
    }

    async Task DownloadAsync(Measurement measurement, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
            await DownloadOnceAsync(measurement, cancellationToken);
    }

    async Task DownloadOnceAsync(Measurement measurement, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync(
            DownloadUrl + BytesPerRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        byte[] buffer = new byte[BufferSize];
        int read;
        while ((read = await stream.ReadAsync(buffer, cancellationToken)) > 0)
            measurement.Add(read);
    }

    sealed class Measurement
    {
        readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        readonly TimeSpan _warmUp;
        readonly Lock _lock = new();
        long _totalBytes;
        long _bytesAtWarmUp = -1;
        TimeSpan _elapsedAtWarmUp;

        public Measurement(TimeSpan warmUp)
            => _warmUp = warmUp;

        public void Add(int bytes)
        {
            lock (_lock)
            {
                if (_bytesAtWarmUp < 0 && _stopwatch.Elapsed >= _warmUp)
                    (_bytesAtWarmUp, _elapsedAtWarmUp) = (_totalBytes, _stopwatch.Elapsed);

                _totalBytes += bytes;
            }
        }

        public double MegabitsPerSecond()
        {
            lock (_lock)
            {
                bool warmedUp = _bytesAtWarmUp >= 0;
                long bytes = warmedUp ? _totalBytes - _bytesAtWarmUp : _totalBytes;
                double seconds = (warmedUp ? _stopwatch.Elapsed - _elapsedAtWarmUp : _stopwatch.Elapsed).TotalSeconds;

                if (bytes == 0 || seconds <= 0)
                    throw new InvalidOperationException("Speed test received no data before timeout.");

                return bytes * 8 / seconds / 1_000_000;
            }
        }
    }
}
