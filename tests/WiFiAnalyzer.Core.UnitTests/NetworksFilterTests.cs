using System.Collections.Generic;
using System.Linq;
using WiFiAnalyzer.Core.Helpers;
using WiFiAnalyzer.Core.Models;
using Xunit;

namespace WiFiAnalyzer.Core.UnitTests;

public class NetworksFilterTests
{
    static readonly List<WiFiNetwork> Networks =
    [
        new() { SSID = "a", FrequencyInHz = 2_412_000_000 },
        new() { SSID = "b", FrequencyInHz = 5_180_000_000 },
        new() { SSID = "c", FrequencyInHz = 6_115_000_000 },
    ];

    [Theory]
    [InlineData("2.4 GHz", "a")]
    [InlineData("5 GHz", "b")]
    [InlineData("6 GHz", "c")]
    [InlineData("All", "abc")]
    public void FilterByGHz_ReturnsMatchingNetworks(string parameter, string expected)
    {
        IEnumerable<WiFiNetwork> result = NetworksFilter.FilterByGHz(Networks, parameter);

        Assert.Equal(expected, string.Concat(result.Select(n => n.SSID)));
    }
}
