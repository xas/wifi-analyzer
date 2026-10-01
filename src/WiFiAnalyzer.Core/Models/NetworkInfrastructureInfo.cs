using System.Net.NetworkInformation;

namespace WiFiAnalyzer.Core.Models;

public class NetworkInfrastructureInfo
{
    public string Interface { get; set; } = null!;
    public NetworkInterfaceType InterfaceType { get; set; }
    public OperationalStatus OperationalStatus { get; set; }
}
