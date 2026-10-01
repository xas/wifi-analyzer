using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace WiFiAnalyzer.Desktop.Converters;

public class LastSeenConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DateTime lastSeen)
            return value;

        TimeSpan difference = DateTime.Now - lastSeen;

        return difference switch
        {
            { TotalMinutes: <= 1 } => "now",
            { TotalMinutes: <= 5 } => "recently",
            { TotalMinutes: <= 60 } => "this hour",
            { TotalHours: <= 24 } => "today",
            { TotalDays: <= 7 } => "this week",
            { TotalDays: <= 31 } => "this month",
            { TotalDays: <= 365 } => "this year",
            _ => "long ago"
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
