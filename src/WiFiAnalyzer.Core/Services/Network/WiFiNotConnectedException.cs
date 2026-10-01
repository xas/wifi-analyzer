using System;

namespace WiFiAnalyzer.Core.Services;

public class WiFiNotConnectedException : InvalidOperationException
{
    public WiFiNotConnectedException()
        : base("Not connected to a WiFi network.")
    {
    }
}
