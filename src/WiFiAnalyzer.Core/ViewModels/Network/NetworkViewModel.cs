using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using WiFiAnalyzer.Core.Models;
using WiFiAnalyzer.Core.Services.ConnectedNetwork;
using WiFiAnalyzer.Core.Services.Notifications;
using WiFiAnalyzer.Core.Services.Networks;

namespace WiFiAnalyzer.Core.ViewModels.Network;

public abstract partial class NetworkViewModel : ViewModelBase
{
    protected readonly IConnectedNetworkService _connectedNetworkService;
    protected readonly INetworksService _networksService;

    protected NetworkViewModel(IConnectedNetworkService connectedNetworkService, INetworksService networksService, INotificationService notificationService)
        : base(notificationService)
        => (_connectedNetworkService, _networksService) = (connectedNetworkService, networksService);

    [ObservableProperty]
    public partial WiFiNetwork? ConnectedNetwork { get; set; }

    [ObservableProperty]
    public partial NetworkStates? NetworkStates { get; set; }

    protected override async Task UpdateStatesAsync()
        => NetworkStates = await Task.Run(_connectedNetworkService.GetConnectedNetworkStates);

    protected override async Task GetDataAsync()
    {
        ConnectedNetwork = await Task.Run(_connectedNetworkService.GetConnectedWiFiNetwork);

        await _networksService.UpdateWiFiNetworkAsync(ConnectedNetwork);

        await UpdateStatesAsync();
    }
}
