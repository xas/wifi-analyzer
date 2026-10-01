namespace WiFiAnalyzer.Core.Models;

public class NetworkSecurityInfo
{
    public AuthenticationAlgorithm Authentication { get; set; }
    public CipherAlgorithm Encryption { get; set; }
}
