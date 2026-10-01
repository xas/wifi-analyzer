using System.Linq;

namespace WiFiAnalyzer.Core.Extensions;

public static class StringExtensions
{
    public static string MacAddressToString(this byte[] macAddress)
        => string.Join(":", macAddress.Select(b => b.ToString("X2")));
}
