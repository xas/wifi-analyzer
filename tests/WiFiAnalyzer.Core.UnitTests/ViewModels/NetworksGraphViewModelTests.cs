using System.Linq;
using System.Threading.Tasks;
using WiFiAnalyzer.Core.Models;
using WiFiAnalyzer.Core.UnitTests.Fakes;
using WiFiAnalyzer.Core.ViewModels.Networks;
using Xunit;

namespace WiFiAnalyzer.Core.UnitTests.ViewModels;

public class NetworksGraphViewModelTests
{
    readonly FakeNetworksService _networksService = new()
    {
        Networks =
        [
            new() { SSID = "b", Channel = 36, FrequencyInHz = 5_180_000_000, NetworkStates = new() { SignalStrengthIndBm = -85, DistanceInMeters = 2.504 } },
            new() { SSID = "a", Channel = 1, FrequencyInHz = 2_412_000_000, NetworkStates = new() { SignalStrengthIndBm = -40, DistanceInMeters = 1 } },
            new() { SSID = "gone", Channel = 6, FrequencyInHz = 2_437_000_000 },
        ]
    };

    NetworksGraphViewModel CreateViewModel() => new(_networksService, new FakeNotificationService());

    [Fact]
    public async Task SignalPoints_SortedByChannel_SkipNetworksWithoutStates()
    {
        NetworksGraphViewModel viewModel = CreateViewModel();

        await viewModel.LoadDataAsync();

        Assert.Equal(["a (Ch.1)", "b (Ch.36)"], viewModel.SignalPoints.Select(p => p.Label));
        Assert.Equal(new NetworkChartPoint("b (Ch.36)", -85, "-85 dBm", "#FF0000"), viewModel.SignalPoints[1]);
    }

    [Fact]
    public async Task DistancePoints_UseRoundedMeters()
    {
        NetworksGraphViewModel viewModel = CreateViewModel();

        await viewModel.LoadDataAsync();

        Assert.Equal([$"{2.5} m", "1 m"], viewModel.DistancePoints.Select(p => p.ValueLabel));
    }

    [Fact]
    public async Task Filter_UpdatesPoints()
    {
        NetworksGraphViewModel viewModel = CreateViewModel();
        await viewModel.LoadDataAsync();

        viewModel.FilterByGHzCommand.Execute("5 GHz");

        Assert.Equal(["b (Ch.36)"], viewModel.SignalPoints.Select(p => p.Label));
    }
}
