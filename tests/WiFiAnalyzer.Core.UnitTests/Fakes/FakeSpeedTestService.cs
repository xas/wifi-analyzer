using System;
using System.Threading.Tasks;
using WiFiAnalyzer.Core.Models;
using WiFiAnalyzer.Core.Services.SpeedTest;

namespace WiFiAnalyzer.Core.UnitTests.Fakes;

public class FakeSpeedTestService : ISpeedTestService
{
    public Exception ExceptionToThrow { get; set; }

    public Task<DownloadSpeed> GetDownloadSpeedAsync()
        => ExceptionToThrow is null
            ? Task.FromResult(new DownloadSpeed(42))
            : Task.FromException<DownloadSpeed>(ExceptionToThrow);
}
