using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using WiFiAnalyzer.Core.Helpers;
using WiFiAnalyzer.Core.Models;
using WiFiAnalyzer.Core.Services.Notifications;
using WiFiAnalyzer.Core.Services.Networks;

namespace WiFiAnalyzer.Core.ViewModels.Networks;

public partial class NetworksGraphViewModel : NetworksViewModel
{
    public NetworksGraphViewModel(INetworksService networksService, INotificationService notificationService)
        : base(networksService, notificationService)
    {
    }

    [ObservableProperty]
    public partial IReadOnlyList<NetworkChartPoint> SignalPoints { get; private set; } = [];

    [ObservableProperty]
    public partial IReadOnlyList<NetworkChartPoint> DistancePoints { get; private set; } = [];

    protected override void ApplyFilter()
    {
        base.ApplyFilter();

        List<(WiFiNetwork Network, NetworkStates States)> measured = FilteredWiFiNetworks
            .Where(n => n.NetworkStates is not null)
            .Select(n => (n, n.NetworkStates!))
            .ToList();

        SignalPoints = measured
            .OrderBy(m => m.Network.Channel)
            .Select(m => new NetworkChartPoint(
                $"{m.Network.SSID} (Ch.{m.Network.Channel})",
                m.States.SignalStrengthIndBm,
                $"{m.States.SignalStrengthIndBm} dBm",
                SingalColorHelper.GetHexColorBySingalStrength(m.States.SignalStrengthIndBm)))
            .ToList();

        DistancePoints = measured
            .Select(m => new NetworkChartPoint(
                m.Network.SSID,
                m.States.DistanceInMeters,
                $"{Math.Round(m.States.DistanceInMeters, 2)} m",
                SingalColorHelper.GetHexColorBySingalStrength(m.States.SignalStrengthIndBm)))
            .ToList();
    }
}
