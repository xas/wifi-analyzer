namespace WiFiAnalyzer.Core.Models;

public enum CipherAlgorithm
{
    None,
    WEP,
    WEP_40,
    WEP_104,
    TKIP,
    CCMP,
    BIP,
    GCMP,
    GCMP_256,
    CCMP_256,
    BIP_GMAC_128,
    BIP_GMAC_256,
    BIP_CMAC_256,
    WPA_USE_GROUP,
    RSN_USE_GROUP,
    IHV_START,
    IHV_END
}
