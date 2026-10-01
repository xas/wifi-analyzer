using System.Collections.ObjectModel;
using WiFiAnalyzer.Core.Models;
using WiFiAnalyzer.Core.Services.Notifications;
using WiFiAnalyzer.Core.Services.Networks;

namespace WiFiAnalyzer.Core.ViewModels.Networks;

public partial class NetworksTableViewModel : NetworksViewModel
{
    public NetworksTableViewModel(INetworksService networksService, INotificationService notificationService)
        : base(networksService, notificationService)
    {
    }

    public ObservableCollection<WiFiNetwork> Rows { get; } = [];

    protected override void ApplyFilter()
    {
        base.ApplyFilter();

        Rows.Clear();
        foreach (WiFiNetwork network in FilteredWiFiNetworks)
            Rows.Add(network);
    }
}
