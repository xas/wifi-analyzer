using WiFiAnalyzer.Core.Models;
using Xunit;

namespace WiFiAnalyzer.Core.UnitTests;

public class NetworkStatesTests
{
    [Theory]
    [InlineData(-120, 0)]
    [InlineData(-100, 0)]
    [InlineData(-75, 50)]
    [InlineData(-50, 100)]
    [InlineData(-20, 100)]
    public void SignalStrengthInPercentage_MapsDbmToPercent(int dBm, int expected)
        => Assert.Equal(expected, new NetworkStates { SignalStrengthIndBm = dBm }.SignalStrengthInPercentage);
}
