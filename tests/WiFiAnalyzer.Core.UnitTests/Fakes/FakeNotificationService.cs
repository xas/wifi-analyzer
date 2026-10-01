using System.Collections.Generic;
using WiFiAnalyzer.Core.Services.Notifications;

namespace WiFiAnalyzer.Core.UnitTests.Fakes;

public class FakeNotificationService : INotificationService
{
    public List<string> Errors { get; } = [];

    public List<string> Warnings { get; } = [];

    public void ShowError(string title, string message)
        => Errors.Add(message);

    public void ShowWarning(string title, string message)
        => Warnings.Add(message);
}
