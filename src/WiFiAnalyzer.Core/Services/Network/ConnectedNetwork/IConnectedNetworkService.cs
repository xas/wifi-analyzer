using System.Threading.Tasks;
using WiFiAnalyzer.Core.Models;

namespace WiFiAnalyzer.Core.Services.ConnectedNetwork;

public interface IConnectedNetworkService
{
    WiFiNetwork GetConnectedWiFiNetwork();
    NetworkStates GetConnectedNetworkStates();
    Task<IPAddressInfo> GetConnectedIPAddressInfo();
    NetworkSecurityInfo GetConnectedNetworkSecurityInfo();
    NetworkInfrastructureInfo GetConnectedNetworkInfrastructureInfo();
}
