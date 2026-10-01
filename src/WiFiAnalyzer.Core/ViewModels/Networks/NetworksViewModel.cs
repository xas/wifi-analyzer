using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WiFiAnalyzer.Core.Helpers;
using WiFiAnalyzer.Core.Models;
using WiFiAnalyzer.Core.Services.Notifications;
using WiFiAnalyzer.Core.Services.Networks;

namespace WiFiAnalyzer.Core.ViewModels.Networks;

public abstract partial class NetworksViewModel : ViewModelBase
{
    public const string AllFrequencies = "All";

    protected readonly INetworksService _networksService;

    protected NetworksViewModel(INetworksService networksService, INotificationService notificationService)
        : base(notificationService)
        => _networksService = networksService;

    public IEnumerable<WiFiNetwork> Networks { get; private set; } = [];

    [ObservableProperty]
    public partial string Frequency { get; private set; } = AllFrequencies;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasFilteredNetworks))]
    [NotifyPropertyChangedFor(nameof(HasNoFilteredNetworks))]
    public partial IEnumerable<WiFiNetwork> FilteredWiFiNetworks { get; set; } = [];

    public bool HasFilteredNetworks => FilteredWiFiNetworks.Any();

    public bool HasNoFilteredNetworks => !HasFilteredNetworks;

    protected override async Task GetDataAsync()
    {
        await Task.Run(_networksService.UpdateWiFiNetworksAsync);
        await UpdateStatesAsync();
    }

    protected override async Task UpdateStatesAsync()
    {
        Networks = await Task.Run(_networksService.GetWiFiNetworksWithStatesAsync);
        ApplyFilter();
    }

    [RelayCommand]
    void FilterByGHz(string frequency)
    {
        Frequency = frequency;
        ApplyFilter();
    }

    protected virtual void ApplyFilter()
        => FilteredWiFiNetworks = NetworksFilter.FilterByGHz(Networks, Frequency);
}
