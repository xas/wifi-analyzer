using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using WiFiAnalyzer.Core.Helpers;

namespace WiFiAnalyzer.Desktop.Converters;

public class SignalStrengthBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is int signalStrength
            ? new SolidColorBrush(Color.Parse(SingalColorHelper.GetHexColorBySingalStrength(signalStrength)))
            : null;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
