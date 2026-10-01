using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using WiFiAnalyzer.Core.Models;
using WiFiAnalyzer.Core.Services.ConnectedNetwork;
using WiFiAnalyzer.Core.Services.Notifications;
using WiFiAnalyzer.Core.Services.Networks;

namespace WiFiAnalyzer.Core.ViewModels.Network;

public partial class ConnectedNetworkViewModel : NetworkViewModel
{
    public ConnectedNetworkViewModel(IConnectedNetworkService connectedNetworkService, INetworksService networksService, INotificationService notificationService)
        : base(connectedNetworkService, networksService, notificationService)
    {
    }

    [ObservableProperty]
    public partial IPAddressInfo? IPAddressInfo { get; set; }

    [ObservableProperty]
    public partial NetworkSecurityInfo? NetworkSecurityInfo { get; set; }

    [ObservableProperty]
    public partial NetworkInfrastructureInfo? NetworkInfrastructureInfo { get; set; }

    protected override async Task GetDataAsync()
    {
        await base.GetDataAsync();

        IPAddressInfo = await _connectedNetworkService.GetConnectedIPAddressInfo();
        NetworkSecurityInfo = await Task.Run(_connectedNetworkService.GetConnectedNetworkSecurityInfo);
        NetworkInfrastructureInfo = await Task.Run(_connectedNetworkService.GetConnectedNetworkInfrastructureInfo);
    }
}
