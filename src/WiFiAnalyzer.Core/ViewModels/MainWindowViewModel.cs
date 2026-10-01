using WiFiAnalyzer.Core.ViewModels.Network;
using WiFiAnalyzer.Core.ViewModels.Networks;

namespace WiFiAnalyzer.Core.ViewModels;

public class MainWindowViewModel
{
    public MainWindowViewModel(MainPageViewModel home, ConnectedNetworkViewModel connected, NetworksPageViewModel networks)
        => (Home, Connected, Networks) = (home, connected, networks);

    public MainPageViewModel Home { get; }

    public ConnectedNetworkViewModel Connected { get; }

    public NetworksPageViewModel Networks { get; }
}
