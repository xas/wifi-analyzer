using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using WiFiAnalyzer.Core.Models;
using Wifi = ManagedNativeWifi;

namespace WiFiAnalyzer.Core.Services.ConnectedNetwork;

public class ConnectedNetworkService : NetworkService, IConnectedNetworkService
{
    static (Wifi.CurrentConnectionInfo Connection, Wifi.BssNetworkInfo Bss) GetConnection()
    {
        foreach (Wifi.InterfaceInfo wlanInterface in Wifi.NativeWifi.EnumerateInterfaces().Where(i => i.State == Wifi.InterfaceState.Connected))
        {
            var (result, connection) = Wifi.NativeWifi.GetCurrentConnection(wlanInterface.Id);
            if (result != Wifi.ActionResult.Success || connection is null)
                continue;

            byte[] bssid = connection.Bssid.ToBytes();
            var (_, bssNetworks) = Wifi.NativeWifi.EnumerateBssNetworks(wlanInterface.Id);
            Wifi.BssNetworkInfo? bss = bssNetworks?.FirstOrDefault(b => b.Bssid.ToBytes().AsSpan().SequenceEqual(bssid));

            if (bss is not null)
                return (connection, bss);
        }

        throw new WiFiNotConnectedException();
    }

    public NetworkStates GetConnectedNetworkStates()
    {
        Wifi.BssNetworkInfo bss = GetConnection().Bss;

        return new NetworkStates
        {
            IsConnected = true,
            SignalStrengthIndBm = bss.Rssi,
            DistanceInMeters = CalculateDistance(bss.Rssi, ToHz(bss.Frequency))
        };
    }

    public WiFiNetwork GetConnectedWiFiNetwork()
    {
        (Wifi.CurrentConnectionInfo connection, Wifi.BssNetworkInfo bss) = GetConnection();

        return new WiFiNetwork
        {
            SSID = connection.Ssid.ToString(),
            Channel = bss.Channel,
            FrequencyInHz = ToHz(bss.Frequency),
            Protocol = ProtocolOf(connection.PhyType),
            MacAddress = bss.Bssid.ToBytes(),
            IsSecured = connection.IsSecurityEnabled,
            AuthenticationAlgorithm = ToModel(connection.AuthenticationAlgorithm),
            LastSeen = DateTime.Now
        };
    }

    public async Task<IPAddressInfo> GetConnectedIPAddressInfo()
    {
        IPAddressInfo iPAddressInfo = new ();

        iPAddressInfo.PrivateIPv4 = GetPrivateIPv4();
        iPAddressInfo.PublicIPv4 = await GetPublicIPv4();
        iPAddressInfo.SubnetMask = GetSubnetMask();

        return iPAddressInfo;
    }

    string GetPrivateIPv4()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        var ipAddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        return ipAddress!.ToString();
    }

    string GetSubnetMask()
    {
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        foreach (var networkInterface in networkInterfaces)
        {
            if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            {
                foreach (var unicastIPAddressInformation in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return unicastIPAddressInformation.IPv4Mask!.ToString();
                    }
                }
            }
        }
        return null!;
    }

    async Task<string> GetPublicIPv4()
    {
        using (HttpClient httpClient = new())
            return (await httpClient.GetStringAsync("http://icanhazip.com")).Trim();
    }
    public NetworkSecurityInfo GetConnectedNetworkSecurityInfo()
    {
        Wifi.CurrentConnectionInfo connection = GetConnection().Connection;

        return new NetworkSecurityInfo
        {
            Authentication = ToModel(connection.AuthenticationAlgorithm),
            Encryption = ToModel(connection.CipherAlgorithm)
        };
    }

    public NetworkInfrastructureInfo GetConnectedNetworkInfrastructureInfo()
    {
        NetworkInfrastructureInfo networkInfrastructureInfo = new();

        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        var currentInterface = networkInterfaces.FirstOrDefault(nic => nic.OperationalStatus == OperationalStatus.Up);

        if (currentInterface != null)
        {
            networkInfrastructureInfo.InterfaceType = currentInterface.NetworkInterfaceType;
            networkInfrastructureInfo.OperationalStatus = currentInterface.OperationalStatus;
            networkInfrastructureInfo.Interface = currentInterface.Description;
        }

        return networkInfrastructureInfo;
    }
}
