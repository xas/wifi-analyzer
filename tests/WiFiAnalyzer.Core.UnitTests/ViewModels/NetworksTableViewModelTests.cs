using System;
using System.Linq;
using System.Threading.Tasks;
using WiFiAnalyzer.Core.Services;
using WiFiAnalyzer.Core.UnitTests.Fakes;
using WiFiAnalyzer.Core.ViewModels.Networks;
using Xunit;

namespace WiFiAnalyzer.Core.UnitTests.ViewModels;

public class NetworksTableViewModelTests
{
    readonly FakeNetworksService _networksService = new()
    {
        Networks =
        [
            new() { SSID = "b", Channel = 6, FrequencyInHz = 2_437_000_000 },
            new() { SSID = "a", Channel = 36, FrequencyInHz = 5_180_000_000 },
            new() { SSID = "c", Channel = 1, FrequencyInHz = 2_412_000_000 },
        ]
    };

    readonly FakeNotificationService _notificationService = new();

    NetworksTableViewModel CreateViewModel() => new(_networksService, _notificationService);

    static string Ssids(NetworksViewModel viewModel)
        => string.Concat(viewModel.FilteredWiFiNetworks.Select(n => n.SSID));

    [Fact]
    public async Task LoadData_ExposesAllNetworks()
    {
        NetworksTableViewModel viewModel = CreateViewModel();

        await viewModel.LoadDataAsync();

        Assert.Equal("bac", Ssids(viewModel));
        Assert.True(viewModel.HasFilteredNetworks);
    }

    [Fact]
    public async Task LoadData_FillsRows()
    {
        NetworksTableViewModel viewModel = CreateViewModel();

        await viewModel.LoadDataAsync();

        Assert.Equal("bac", string.Concat(viewModel.Rows.Select(n => n.SSID)));
    }

    [Fact]
    public async Task Filter_SurvivesRefreshAndKeepsSameRowsInstance()
    {
        NetworksTableViewModel viewModel = CreateViewModel();
        await viewModel.LoadDataAsync();
        var rows = viewModel.Rows;

        viewModel.FilterByGHzCommand.Execute("2.4 GHz");
        await viewModel.LoadDataAsync();

        Assert.Equal("bc", Ssids(viewModel));
        Assert.Equal("bc", string.Concat(viewModel.Rows.Select(n => n.SSID)));
        Assert.Equal("2.4 GHz", viewModel.Frequency);
        Assert.Same(rows, viewModel.Rows);
    }

    [Fact]
    public async Task LoadData_ShowsErrorInsteadOfThrowing()
    {
        _networksService.ExceptionToThrow = new InvalidOperationException("boom");
        NetworksTableViewModel viewModel = CreateViewModel();

        await viewModel.LoadDataAsync();

        Assert.Equal(["boom"], _notificationService.Errors);
    }

    [Fact]
    public async Task SameError_IsNotifiedOnceUntilRecovery()
    {
        _networksService.ExceptionToThrow = new InvalidOperationException("boom");
        NetworksTableViewModel viewModel = CreateViewModel();

        await viewModel.LoadDataAsync();
        await viewModel.LoadDataAsync();
        Assert.Equal(["boom"], _notificationService.Errors);

        _networksService.ExceptionToThrow = null;
        await viewModel.LoadDataAsync();
        _networksService.ExceptionToThrow = new InvalidOperationException("boom");
        await viewModel.LoadDataAsync();

        Assert.Equal(["boom", "boom"], _notificationService.Errors);
    }

    [Fact]
    public async Task WiFiNotConnected_IsAWarning()
    {
        _networksService.ExceptionToThrow = new WiFiNotConnectedException();
        NetworksTableViewModel viewModel = CreateViewModel();

        await viewModel.LoadDataAsync();

        Assert.Empty(_notificationService.Errors);
        Assert.Equal(["Not connected to a WiFi network."], _notificationService.Warnings);
    }

    [Fact]
    public void ActivateDeactivate_CanBeRepeated()
    {
        NetworksTableViewModel viewModel = CreateViewModel();

        viewModel.Activate();
        viewModel.Activate();
        Assert.True(viewModel.IsActive);

        viewModel.Deactivate();
        Assert.False(viewModel.IsActive);

        viewModel.Activate();
        Assert.True(viewModel.IsActive);
        viewModel.Deactivate();
    }
}
