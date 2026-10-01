using System;
using System.IO;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WiFiAnalyzer.Core.Database;
using WiFiAnalyzer.Core.Services.ConnectedNetwork;
using WiFiAnalyzer.Core.Services.Networks;
using WiFiAnalyzer.Core.Services.SpeedTest;
using WiFiAnalyzer.Core.ViewModels;
using WiFiAnalyzer.Core.ViewModels.Network;
using WiFiAnalyzer.Core.ViewModels.Networks;

namespace WiFiAnalyzer.Core.Providers;

public static class ServiceProviders
{
    public static string DefaultDatabasePath
        => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WiFiAnalyzer", "WiFiAnalyzer.db");

    public static void AddSqlite(this IServiceCollection services, string? databasePath = null)
    {
        databasePath ??= DefaultDatabasePath;
        Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);

        services.AddDbContext<WiFiAnalyzerContext>(opts => opts.UseSqlite($"Data Source={databasePath}"));
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IConnectedNetworkService, ConnectedNetworkService>();
        services.AddSingleton<ISpeedTestService>(_ => new CloudflareSpeedTestService(new HttpClient()));
        services.AddSingleton<INetworksService, NetworksService>();
    }

    public static void AddViewModels(this IServiceCollection services)
    {
        services.AddSingleton<ConnectedNetworkViewModel>();
        services.AddSingleton<MainPageViewModel>();
        services.AddSingleton<NetworksTableViewModel>();
        services.AddSingleton<NetworksGraphViewModel>();
        services.AddSingleton<NetworksPageViewModel>();
        services.AddSingleton<MainWindowViewModel>();
    }
}
