using System;
using System.Threading.Tasks;
using WiFiAnalyzer.Core.Services.ConnectedNetwork;
using WiFiAnalyzer.Core.UnitTests.Fakes;
using WiFiAnalyzer.Core.ViewModels.Network;
using Xunit;

namespace WiFiAnalyzer.Core.UnitTests.ViewModels;

public class MainPageViewModelTests
{
    readonly FakeSpeedTestService _speedTestService = new();
    readonly FakeNotificationService _notificationService = new();

    MainPageViewModel CreateViewModel()
        => new(new ConnectedNetworkService(), _speedTestService, new FakeNetworksService(), _notificationService);

    [Fact]
    public async Task GetDownloadSpeed_SetsResult()
    {
        MainPageViewModel viewModel = CreateViewModel();

        await viewModel.GetDownloadSpeedAsync();

        Assert.Equal(42, viewModel.DownloadSpeed.MegabitsPerSecond);
        Assert.False(viewModel.IsBusy);
    }

    [Fact]
    public async Task GetDownloadSpeed_OnError_ResetsBusyAndShowsError()
    {
        _speedTestService.ExceptionToThrow = new InvalidOperationException("offline");
        MainPageViewModel viewModel = CreateViewModel();

        await viewModel.GetDownloadSpeedAsync();

        Assert.False(viewModel.IsBusy);
        Assert.Null(viewModel.DownloadSpeed);
        Assert.Equal(["offline"], _notificationService.Errors);
    }
}
