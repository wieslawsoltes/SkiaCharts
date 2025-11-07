using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;

namespace SkiaCharts.Gallery.ViewModels.InteractiveDemos;

public class ZoomPanViewModel : ReactiveObject
{
    private string _viewportInfo = "Use mouse wheel to zoom, drag to pan";

    public ZoomPanViewModel()
    {
        Chart = new LineChart
        {
            LineWidth = 2f,
            ShowMarkers = true,
            MarkerSize = 5f
        };

        // Create large dataset for zoom/pan demonstration
        var points = new List<IDataPoint>();
        var random = new Random(42);

        for (int i = 0; i <= 500; i++)
        {
            double x = i;
            double y = 50 + Math.Sin(i * 0.1) * 30 + Math.Cos(i * 0.05) * 20 + random.Next(-8, 9);
            points.Add(new DataPoint(x, y));
        }

        var series = new DataSeries<IDataPoint>(points, "Large Dataset");
        Chart.Series.Add(series);
    }

    public LineChart Chart { get; }

    public string ViewportInfo
    {
        get => _viewportInfo;
        set => this.RaiseAndSetIfChanged(ref _viewportInfo, value);
    }

    // These would be called by chart zoom/pan events
    public void OnZoom(double zoomLevel)
    {
        ViewportInfo = $"Zoom level: {zoomLevel:F2}x";
    }

    public void OnPan(double xMin, double xMax, double yMin, double yMax)
    {
        ViewportInfo = $"Viewport: X=[{xMin:F1}, {xMax:F1}], Y=[{yMin:F1}, {yMax:F1}]";
    }

    public void ResetView()
    {
        ViewportInfo = "View reset to default";
        this.RaisePropertyChanged(nameof(Chart));
    }
}
