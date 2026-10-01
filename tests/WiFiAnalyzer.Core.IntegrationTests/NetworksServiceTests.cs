using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WiFiAnalyzer.Core.Database;
using WiFiAnalyzer.Core.Models;
using WiFiAnalyzer.Core.Providers;
using WiFiAnalyzer.Core.Services.Networks;
using Xunit;


namespace WiFiAnalyzer.Core.IntegrationTests;

public sealed class NetworksServiceTests : IDisposable
{
    readonly string _directory = Path.Combine(Path.GetTempPath(), "WiFiAnalyzerTests", Guid.NewGuid().ToString("N"));
    readonly ServiceProvider _services;

    public NetworksServiceTests()
    {
        ServiceCollection services = new();
        services.AddSqlite(Path.Combine(_directory, "test.db"));
        services.AddServices();
        _services = services.BuildServiceProvider();
    }

    INetworksService NetworksService => _services.GetRequiredService<INetworksService>();

    static WiFiNetwork CreateNetwork(string ssid, byte lastMacByte) => new()
    {
        SSID = ssid,
        Protocol = "802.11ax",
        MacAddress = [0xD0, 0x4D, 0xC6, 0x8B, 0x60, lastMacByte],
        FrequencyInHz = 5_180_000_000,
        Channel = 36,
        IsSecured = true,
        AuthenticationAlgorithm = AuthenticationAlgorithm.RSNA_PSK,
        LastSeen = new DateTime(2026, 10, 1, 9, 30, 0)
    };

    [Fact]
    public async Task AddWiFiNetwork_CreatesDatabaseFileAndRoundTrips()
    {
        await NetworksService.AddWiFiNetworkAsync(CreateNetwork("home", 0x96));

        WiFiNetwork network = await NetworksService.GetWiFiNetworkByBSSIDAsync("D0:4D:C6:8B:60:96");

        Assert.True(File.Exists(Path.Combine(_directory, "test.db")));
        Assert.NotNull(network);
        Assert.Equal("home", network.SSID);
        Assert.Equal(5180, network.FrequencyInMHz);
        Assert.Equal(AuthenticationAlgorithm.RSNA_PSK, network.AuthenticationAlgorithm);
        Assert.Equal(new DateTime(2026, 10, 1, 9, 30, 0), network.LastSeen);
    }

    [Fact]
    public async Task UpdateWiFiNetwork_UpdatesExistingRowByBSSID()
    {
        await NetworksService.AddWiFiNetworkAsync(CreateNetwork("old", 0x01));

        WiFiNetwork changed = CreateNetwork("new", 0x01);
        changed.Channel = 140;
        await NetworksService.UpdateWiFiNetworkAsync(changed);

        WiFiNetwork network = Assert.Single(await NetworksService.GetWiFiNetworksAsync());
        Assert.Equal("new", network.SSID);
        Assert.Equal(140, network.Channel);
    }

    [Fact]
    public async Task DeleteWiFiNetworkByBSSID_RemovesRow()
    {
        await NetworksService.AddWiFiNetworkAsync(CreateNetwork("a", 0x01));
        await NetworksService.AddWiFiNetworkAsync(CreateNetwork("b", 0x02));

        await NetworksService.DeleteWiFiNetworkByBSSIDAsync("D0:4D:C6:8B:60:01");

        Assert.Equal(["b"], (await NetworksService.GetWiFiNetworksAsync()).Select(n => n.SSID));
    }

    [Fact]
    public void Schema_HasUniqueIndexOnMacAddress()
    {
        WiFiAnalyzerContext context = _services.GetRequiredService<WiFiAnalyzerContext>();
        context.WiFiNetworks.Add(CreateNetwork("a", 0x01));
        context.WiFiNetworks.Add(CreateNetwork("b", 0x01));

        Assert.Throws<DbUpdateException>(() => context.SaveChanges());
    }

    public void Dispose()
    {
        _services.Dispose();
        SqliteConnection.ClearAllPools();
        Directory.Delete(_directory, recursive: true);
    }
}
