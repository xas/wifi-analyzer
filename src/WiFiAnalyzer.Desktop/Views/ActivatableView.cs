using Avalonia;
using Avalonia.Controls;
using WiFiAnalyzer.Core.ViewModels;

namespace WiFiAnalyzer.Desktop.Views;

public class ActivatableView : UserControl
{
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        (DataContext as ViewModelBase)?.Activate();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        (DataContext as ViewModelBase)?.Deactivate();
        base.OnDetachedFromVisualTree(e);
    }
}
