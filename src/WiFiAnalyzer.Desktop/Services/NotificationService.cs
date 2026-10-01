using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using WiFiAnalyzer.Core.Services.Notifications;

namespace WiFiAnalyzer.Desktop.Services;

public sealed class NotificationService : INotificationService
{
    readonly Queue<Notification> _pending = new();
    WindowNotificationManager? _manager;

    public void Attach(Window window)
    {
        _manager = new WindowNotificationManager(window)
        {
            Position = NotificationPosition.TopRight,
            MaxItems = 3
        };

        while (_pending.TryDequeue(out Notification? notification))
            _manager.Show(notification);
    }

    public void ShowError(string title, string message)
        => Show(new Notification(title, message, NotificationType.Error, TimeSpan.FromSeconds(10)));

    public void ShowWarning(string title, string message)
        => Show(new Notification(title, message, NotificationType.Warning, TimeSpan.FromSeconds(10)));

    void Show(Notification notification)
        => Dispatcher.UIThread.Post(() =>
        {
            if (_manager is null)
                _pending.Enqueue(notification);
            else
                _manager.Show(notification);
        });
}
