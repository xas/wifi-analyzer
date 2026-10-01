using WiFiAnalyzer.Core.Extensions;
using Xunit;

namespace WiFiAnalyzer.Core.UnitTests;

public class StringExtensionsTests
{
    [Fact]
    public void MacAddressToString_FormatsAsHexPairs()
        => Assert.Equal("00:1A:FF", new byte[] { 0x00, 0x1A, 0xFF }.MacAddressToString());
}
