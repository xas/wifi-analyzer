using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WiFiAnalyzer.Core.Models;
using WiFiAnalyzer.Core.Services.ConnectedNetwork;
using WiFiAnalyzer.Core.Services.Notifications;
using WiFiAnalyzer.Core.Services.Networks;
using WiFiAnalyzer.Core.Services.SpeedTest;

namespace WiFiAnalyzer.Core.ViewModels.Network;

public partial class MainPageViewModel : NetworkViewModel
{
    readonly ISpeedTestService _speedTestService;

    public MainPageViewModel(IConnectedNetworkService connectedNetworkService, ISpeedTestService speedTestService, INetworksService networksService, INotificationService notificationService)
        : base(connectedNetworkService, networksService, notificationService)
        => _speedTestService = speedTestService;

    [ObservableProperty]
    public partial DownloadSpeed? DownloadSpeed { get; set; }

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    [RelayCommand]
    public async Task GetDownloadSpeedAsync()
    {
        try
        {
            DownloadSpeed = null;
            IsBusy = true;
            DownloadSpeed = await _speedTestService.GetDownloadSpeedAsync();
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
