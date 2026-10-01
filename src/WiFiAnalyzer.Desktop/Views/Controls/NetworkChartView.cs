using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using ScottPlot;
using ScottPlot.Avalonia;
using WiFiAnalyzer.Core.Models;

namespace WiFiAnalyzer.Desktop.Views.Controls;

public enum NetworkChartKind
{
    Line,
    Column
}

public class NetworkChartView : UserControl
{
    public static readonly StyledProperty<IReadOnlyList<NetworkChartPoint>?> PointsProperty =
        AvaloniaProperty.Register<NetworkChartView, IReadOnlyList<NetworkChartPoint>?>(nameof(Points));

    public static readonly StyledProperty<NetworkChartKind> KindProperty =
        AvaloniaProperty.Register<NetworkChartView, NetworkChartKind>(nameof(Kind));

    static readonly Color BackgroundColor = Color.FromHex("#000000");
    static readonly Color ForegroundColor = Color.FromHex("#FFFFFF");
    static readonly Color GridColor = Color.FromHex("#3D3D3D");

    const int MaxTickLabelLength = 18;

    readonly AvaPlot _plot = new();

    public NetworkChartView()
    {
        _plot.UserInputProcessor.Disable();
        Content = _plot;
    }

    public IReadOnlyList<NetworkChartPoint>? Points
    {
        get => GetValue(PointsProperty);
        set => SetValue(PointsProperty, value);
    }

    public NetworkChartKind Kind
    {
        get => GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == PointsProperty || change.Property == KindProperty)
            UpdateChart();
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        UpdateChart();
    }

    void UpdateChart()
    {
        NetworkChartPoint[] points = Points?.ToArray() ?? [];
        ChartLayout layout = ChartLayout.For(points, Bounds.Size);
        Plot plot = _plot.Plot;

        plot.Clear();
        ApplyDarkTheme(plot);

        if (Kind == NetworkChartKind.Column)
            AddColumns(plot, points, layout);
        else
            AddLine(plot, points, layout);

        double[] positions = Enumerable.Range(0, points.Length).Select(i => (double)i).ToArray();
        string[] labels = points.Select((p, i) => i % layout.TickStep == 0 ? Shorten(p.Label, layout.MaxLabelLength) : string.Empty).ToArray();
        plot.Axes.Bottom.SetTicks(positions, labels);
        plot.Axes.Bottom.TickLabelStyle.Rotation = 45;
        plot.Axes.Bottom.TickLabelStyle.Alignment = Alignment.MiddleLeft;
        plot.Axes.Bottom.MinimumSize = (float)layout.BottomAxisHeight;
        plot.Axes.AutoScale();

        _plot.Refresh();
    }

    static void AddColumns(Plot plot, NetworkChartPoint[] points, ChartLayout layout)
    {
        Bar[] bars = points
            .Select((point, index) => new Bar
            {
                Position = index,
                Value = point.Value,
                FillColor = Color.FromHex(point.HexColor),
                Label = layout.ShowValueLabels ? point.ValueLabel : string.Empty
            })
            .ToArray();

        var barPlot = plot.Add.Bars(bars);
        barPlot.ValueLabelStyle.ForeColor = ForegroundColor;
        plot.Axes.Margins(bottom: 0, top: layout.TopMargin);
    }

    static void AddLine(Plot plot, NetworkChartPoint[] points, ChartLayout layout)
    {
        double[] xs = Enumerable.Range(0, points.Length).Select(i => (double)i).ToArray();
        double[] ys = points.Select(p => p.Value).ToArray();

        plot.Axes.Margins(top: layout.TopMargin);

        var line = plot.Add.ScatterLine(xs, ys);
        line.Color = GridColor.Lighten(0.3);
        line.LineWidth = 2;

        for (int i = 0; i < points.Length; i++)
        {
            Color color = Color.FromHex(points[i].HexColor);
            plot.Add.Marker(xs[i], ys[i], MarkerShape.FilledCircle, layout.MarkerSize, color);

            if (!layout.ShowValueLabels)
                continue;

            var label = plot.Add.Text(points[i].ValueLabel, xs[i], ys[i]);
            label.LabelFontColor = color;
            label.LabelAlignment = Alignment.LowerCenter;
            label.OffsetY = -10;
        }
    }

    static string Shorten(string label, int maxLength)
        => label.Length <= maxLength ? label : label[..(maxLength - 1)] + "…";

    readonly record struct ChartLayout(bool ShowValueLabels, double TopMargin, double BottomAxisHeight, int MaxLabelLength, int TickStep, float MarkerSize)
    {
        const double LeftAxisWidth = 60;
        const double ValueLabelHeight = 40;
        const double PixelsPerLabelChar = 5.5;
        const double MaxBottomAxisShare = 0.35;
        const double MinTickSpacing = 14;
        const double MinValueLabelSpacing = 52;

        public static ChartLayout For(NetworkChartPoint[] points, Size size)
        {
            int maxLabelLength = (int)Math.Clamp((size.Height * MaxBottomAxisShare - 16) / PixelsPerLabelChar, 4, MaxTickLabelLength);
            int longestLabel = points.Length == 0 ? 0 : points.Max(p => Math.Min(p.Label.Length, maxLabelLength));
            double bottomAxisHeight = longestLabel * PixelsPerLabelChar + 16;
            double dataHeight = Math.Max(size.Height - bottomAxisHeight - 20, 1);
            double spacing = Math.Max(size.Width - LeftAxisWidth, 1) / Math.Max(points.Length, 1);

            bool showValueLabels = spacing >= MinValueLabelSpacing && dataHeight >= 100;
            double topMargin = showValueLabels ? Math.Clamp(ValueLabelHeight / dataHeight, 0.05, 0.45) : 0.05;
            int tickStep = (int)Math.Ceiling(MinTickSpacing / spacing);

            return new ChartLayout(showValueLabels, topMargin, bottomAxisHeight, maxLabelLength, Math.Max(tickStep, 1), spacing < 20 ? 6 : 12);
        }
    }

    static void ApplyDarkTheme(Plot plot)
    {
        plot.FigureBackground.Color = BackgroundColor;
        plot.DataBackground.Color = BackgroundColor;
        plot.Axes.Color(ForegroundColor);
        plot.Grid.MajorLineColor = GridColor;
    }
}
