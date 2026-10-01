using System;
using WiFiAnalyzer.Core.Models;
using WiFiAnalyzer.Core.Services;
using Xunit;
using Wifi = ManagedNativeWifi;

namespace WiFiAnalyzer.Core.UnitTests.Services;

public class NetworkServiceTests
{
    [Fact]
    public void ToModel_MapsEveryAuthenticationAlgorithmByName()
    {
        foreach (Wifi.AuthenticationAlgorithm algorithm in Enum.GetValues<Wifi.AuthenticationAlgorithm>())
            Assert.Equal(algorithm.ToString(), NetworkService.ToModel(algorithm).ToString());
    }

    [Fact]
    public void ToModel_MapsEveryCipherAlgorithmByName()
    {
        foreach (Wifi.CipherAlgorithm algorithm in Enum.GetValues<Wifi.CipherAlgorithm>())
            Assert.Equal(algorithm.ToString(), NetworkService.ToModel(algorithm).ToString());
    }
}
