using System;
using System.Collections.Generic;
using System.Linq;
using WiFiAnalyzer.Core.Models;
using Wifi = ManagedNativeWifi;

namespace WiFiAnalyzer.Core.Services;

public abstract class NetworkService
{
    static NetworkService()
        => Wifi.NativeWifi.ThrowsOnAnyFailure = true;

    protected static long ToHz(int frequencyInKHz)
        => frequencyInKHz * 1_000L;

    protected static string ProtocolOf(Wifi.PhyType phyType)
        => phyType switch
        {
            Wifi.PhyType.Dsss or Wifi.PhyType.HrDsss => "802.11b",
            Wifi.PhyType.Ofdm => "802.11a",
            Wifi.PhyType.Erp => "802.11g",
            Wifi.PhyType.Ht => "802.11n",
            Wifi.PhyType.Vht => "802.11ac",
            Wifi.PhyType.Dmg => "802.11ad",
            Wifi.PhyType.He => "802.11ax",
            Wifi.PhyType.Eht => "802.11be",
            _ => "Unknown"
        };

    public static AuthenticationAlgorithm ToModel(Wifi.AuthenticationAlgorithm algorithm)
        => Enum.TryParse(algorithm.ToString(), out AuthenticationAlgorithm result) ? result : AuthenticationAlgorithm.Unknown;

    public static CipherAlgorithm ToModel(Wifi.CipherAlgorithm algorithm)
        => Enum.TryParse(algorithm.ToString(), out CipherAlgorithm result) ? result : CipherAlgorithm.None;

    protected static Dictionary<string, Wifi.AvailableNetworkPack> AvailableNetworksBySsid()
        => Wifi.NativeWifi.EnumerateAvailableNetworks()
            .GroupBy(n => n.Ssid.ToString())
            .ToDictionary(g => g.Key, g => g.First());

    protected static double CalculateDistance(int signalStrength, long frequencyInHz)
    {
        int frequencyInMHz = (int)(frequencyInHz / 1_000_000);
        double exp = (27.55 - (20 * Math.Log10(frequencyInMHz)) + Math.Abs(signalStrength)) / 20.0;
        return Math.Pow(10.0, exp);
    }
}
