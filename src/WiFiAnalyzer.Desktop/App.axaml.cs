using System.Net.NetworkInformation;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using WiFiAnalyzer.Core.Providers;
using WiFiAnalyzer.Core.Services.Notifications;
using WiFiAnalyzer.Core.ViewModels;
using WiFiAnalyzer.Desktop.Services;
using WiFiAnalyzer.Desktop.Views;

namespace WiFiAnalyzer.Desktop;

public partial class App : Application
{
    public override void Initialize()
        => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            NotificationService notificationService = new();
            ServiceProvider services = ConfigureServices(notificationService);

            MainWindow mainWindow = new()
            {
                DataContext = services.GetRequiredService<MainWindowViewModel>()
            };
            notificationService.Attach(mainWindow);

            if (!NetworkInterface.GetIsNetworkAvailable())
                notificationService.ShowWarning("Network", "No network connection detected.");

            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    static ServiceProvider ConfigureServices(INotificationService notificationService)
    {
        ServiceCollection services = new();

        services.AddSingleton(notificationService);
        services.AddSqlite();
        services.AddServices();
        services.AddViewModels();

        return services.BuildServiceProvider();
    }
}
