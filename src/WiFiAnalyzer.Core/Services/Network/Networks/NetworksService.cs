using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WiFiAnalyzer.Core.Database;
using WiFiAnalyzer.Core.Extensions;
using WiFiAnalyzer.Core.Models;
using Wifi = ManagedNativeWifi;

namespace WiFiAnalyzer.Core.Services.Networks;

public class NetworksService : NetworkService, INetworksService
{
    readonly WiFiAnalyzerContext db;
    public NetworksService(WiFiAnalyzerContext db)
        => this.db = db;

    DbSet<WiFiNetwork> dbSet => db.WiFiNetworks;

    public async Task<IEnumerable<WiFiNetwork>> GetWiFiNetworksWithStatesAsync()
    {
        IEnumerable<WiFiNetwork> networks = await GetWiFiNetworksAsync();
        Dictionary<string, NetworkStates> states = GetVisibleNetworkStates();

        foreach (WiFiNetwork network in networks)
            network.NetworkStates = states.GetValueOrDefault(network.StringMacAddress);

        return networks;
    }

    public async Task<IEnumerable<WiFiNetwork>> GetWiFiNetworksAsync()
        => await dbSet.ToListAsync();

    public async Task AddWiFiNetworkAsync(WiFiNetwork network)
    {
        var existingNetwork = await GetWiFiNetworkByIdAsync(network.Id);
        if (existingNetwork is null)
        {
            await dbSet.AddAsync(network);
            await SaveChangesAsync();
        }
    }

    public async Task UpdateWiFiNetworkAsync(WiFiNetwork network)
    {
        WiFiNetwork? _network = await GetWiFiNetworkByBSSIDAsync(network.StringMacAddress)!;

        if (_network is not null)
        {
            CopyWiFiNetworkValues(network, _network);
            dbSet.Update(_network);
            await SaveChangesAsync();
        }
    }

    async Task DeleteWiFiNetworkAsync(WiFiNetwork? network)
    {
        if (network is not null)
        {
            dbSet.Remove(network);
            await SaveChangesAsync();
        }
    }

    public async Task DeleteWiFiNetworkByIdAsync(long networkId)
    {
        WiFiNetwork? network = await GetWiFiNetworkByIdAsync(networkId);
        await DeleteWiFiNetworkAsync(network);
    }

    public async Task DeleteWiFiNetworkByBSSIDAsync(string BSSID)
    {
        WiFiNetwork? network = await GetWiFiNetworkByBSSIDAsync(BSSID);
        await DeleteWiFiNetworkAsync(network);
    }

    public async Task<WiFiNetwork?> GetWiFiNetworkByIdAsync(long id)
        => (await GetWiFiNetworksAsync()).SingleOrDefault(n => n.Id == id);

    public async Task<WiFiNetwork?> GetWiFiNetworkByBSSIDAsync(string BSSID)
        => (await GetWiFiNetworksAsync()).SingleOrDefault(n => n.StringMacAddress == BSSID);

    public async Task UpdateWiFiNetworksAsync()
    {
        Dictionary<string, Wifi.AvailableNetworkPack> availableNetworks = AvailableNetworksBySsid();

        foreach (Wifi.BssNetworkPack bss in Wifi.NativeWifi.EnumerateBssNetworks())
        {
            string ssid = bss.Ssid.ToString();
            if (!availableNetworks.TryGetValue(ssid, out Wifi.AvailableNetworkPack? availableNetwork))
                continue;

            WiFiNetwork network = new()
            {
                SSID = ssid,
                FrequencyInHz = ToHz(bss.Frequency),
                Channel = bss.Channel,
                MacAddress = bss.Bssid.ToBytes(),
                IsSecured = availableNetwork.IsSecurityEnabled,
                AuthenticationAlgorithm = ToModel(availableNetwork.AuthenticationAlgorithm),
                Protocol = ProtocolOf(bss.PhyType),
                LastSeen = DateTime.Now
            };

            WiFiNetwork? existingNetwork = await GetWiFiNetworkByBSSIDAsync(network.StringMacAddress);

            if (existingNetwork is null)
            {
                await AddWiFiNetworkAsync(network);
            }
            else
            {
                CopyWiFiNetworkValues(network, existingNetwork);
                await UpdateWiFiNetworkAsync(existingNetwork);
            }
        }
    }

    public NetworkStates? GetNetworkStates(WiFiNetwork network)
        => GetVisibleNetworkStates().GetValueOrDefault(network.StringMacAddress);

    static Dictionary<string, NetworkStates> GetVisibleNetworkStates()
    {
        HashSet<string> connectedBssids = Wifi.NativeWifi.EnumerateInterfaces()
            .Where(i => i.State == Wifi.InterfaceState.Connected)
            .Select(i => Wifi.NativeWifi.GetCurrentConnection(i.Id))
            .Where(c => c.Item1 == Wifi.ActionResult.Success && c.Item2 is not null)
            .Select(c => c.Item2!.Bssid.ToBytes().MacAddressToString())
            .ToHashSet();

        Dictionary<string, NetworkStates> states = [];
        foreach (Wifi.BssNetworkPack bss in Wifi.NativeWifi.EnumerateBssNetworks())
        {
            string bssid = bss.Bssid.ToBytes().MacAddressToString();
            states[bssid] = new NetworkStates
            {
                IsConnected = connectedBssids.Contains(bssid),
                SignalStrengthIndBm = bss.Rssi,
                DistanceInMeters = CalculateDistance(bss.Rssi, ToHz(bss.Frequency))
            };
        }

        return states;
    }

    void CopyWiFiNetworkValues(WiFiNetwork fromNetwork, WiFiNetwork toNetwork)
    {
        toNetwork.SSID = fromNetwork.SSID;
        toNetwork.FrequencyInHz = fromNetwork.FrequencyInHz;
        toNetwork.Channel = fromNetwork.Channel;
        toNetwork.IsSecured = fromNetwork.IsSecured;
        toNetwork.AuthenticationAlgorithm = fromNetwork.AuthenticationAlgorithm;
        toNetwork.Protocol = fromNetwork.Protocol;
        toNetwork.LastSeen = fromNetwork.LastSeen;
    }

    async Task SaveChangesAsync()
        => await db.SaveChangesAsync();
}
