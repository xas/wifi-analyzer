using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WiFiAnalyzer.Core.Models;
using WiFiAnalyzer.Core.Services.Networks;

namespace WiFiAnalyzer.Core.UnitTests.Fakes;

public class FakeNetworksService : INetworksService
{
    public List<WiFiNetwork> Networks { get; set; } = [];

    public Exception ExceptionToThrow { get; set; }

    public Task UpdateWiFiNetworksAsync()
        => ExceptionToThrow is null ? Task.CompletedTask : Task.FromException(ExceptionToThrow);

    public Task<IEnumerable<WiFiNetwork>> GetWiFiNetworksAsync()
        => Task.FromResult<IEnumerable<WiFiNetwork>>(Networks);

    public Task<IEnumerable<WiFiNetwork>> GetWiFiNetworksWithStatesAsync()
        => Task.FromResult<IEnumerable<WiFiNetwork>>(Networks);

    public Task AddWiFiNetworkAsync(WiFiNetwork network) => Task.CompletedTask;

    public Task UpdateWiFiNetworkAsync(WiFiNetwork network) => Task.CompletedTask;

    public Task DeleteWiFiNetworkByIdAsync(long networkId) => Task.CompletedTask;

    public Task DeleteWiFiNetworkByBSSIDAsync(string BSSID) => Task.CompletedTask;

    public Task<WiFiNetwork> GetWiFiNetworkByIdAsync(long id) => Task.FromResult<WiFiNetwork>(null);

    public Task<WiFiNetwork> GetWiFiNetworkByBSSIDAsync(string BSSID) => Task.FromResult<WiFiNetwork>(null);

    public NetworkStates GetNetworkStates(WiFiNetwork network) => network.NetworkStates;
}
