using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WiFiAnalyzer.Core.ViewModels.Networks;

public partial class NetworksPageViewModel : ObservableObject
{
    public NetworksPageViewModel(NetworksTableViewModel table, NetworksGraphViewModel graph)
        => (Table, Graph) = (table, graph);

    public NetworksTableViewModel Table { get; }

    public NetworksGraphViewModel Graph { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Current))]
    [NotifyPropertyChangedFor(nameof(ToggleLabel))]
    public partial bool ShowGraph { get; private set; }

    public NetworksViewModel Current => ShowGraph ? Graph : Table;

    public string ToggleLabel => ShowGraph ? "View Table" : "View Graph";

    [RelayCommand]
    void Toggle()
        => ShowGraph = !ShowGraph;
}
