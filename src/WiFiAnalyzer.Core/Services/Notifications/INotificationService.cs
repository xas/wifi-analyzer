namespace WiFiAnalyzer.Core.Services.Notifications;

public interface INotificationService
{
    void ShowError(string title, string message);

    void ShowWarning(string title, string message);
}
