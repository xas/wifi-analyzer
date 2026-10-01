using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WiFiAnalyzer.Core.Services;
using WiFiAnalyzer.Core.Services.Notifications;

namespace WiFiAnalyzer.Core.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    public static readonly TimeSpan RefreshInterval = TimeSpan.FromSeconds(30);

    readonly INotificationService _notificationService;
    readonly SemaphoreSlim _gate = new(1, 1);
    CancellationTokenSource? _refreshCts;
    string? _lastProblem;

    protected ViewModelBase(INotificationService notificationService)
        => _notificationService = notificationService;

    public bool IsActive => _refreshCts is not null;

    public void Activate()
    {
        if (IsActive)
            return;

        _refreshCts = new CancellationTokenSource();
        _ = RunAsync(_refreshCts.Token);
    }

    public void Deactivate()
    {
        if (!IsActive)
            return;

        _refreshCts!.Cancel();
        _refreshCts.Dispose();
        _refreshCts = null;
    }

    [RelayCommand]
    public Task LoadDataAsync()
        => RunSafeAsync(GetDataAsync);

    protected abstract Task GetDataAsync();

    protected abstract Task UpdateStatesAsync();

    protected void ShowError(string message)
        => _notificationService.ShowError("Error", message);

    async Task RunAsync(CancellationToken cancellationToken)
    {
        await LoadDataAsync();

        using PeriodicTimer timer = new(RefreshInterval);
        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
                await RunSafeAsync(UpdateStatesAsync);
        }
        catch (OperationCanceledException)
        {
        }
    }

    async Task RunSafeAsync(Func<Task> action)
    {
        await _gate.WaitAsync();
        try
        {
            await action();
            _lastProblem = null;
        }
        catch (Exception ex)
        {
            ReportOnce(ex);
        }
        finally
        {
            _gate.Release();
        }
    }

    void ReportOnce(Exception exception)
    {
        if (exception.Message == _lastProblem)
            return;

        _lastProblem = exception.Message;

        if (exception is WiFiNotConnectedException)
            _notificationService.ShowWarning("WiFi", exception.Message);
        else
            ShowError(exception.Message);
    }
}
