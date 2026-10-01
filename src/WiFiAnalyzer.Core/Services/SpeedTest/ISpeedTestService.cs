using System.Threading.Tasks;
using WiFiAnalyzer.Core.Models;

namespace WiFiAnalyzer.Core.Services.SpeedTest;

public interface ISpeedTestService
{
    Task<DownloadSpeed> GetDownloadSpeedAsync();
}
