using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.InteractiveDemos;

public class PointSelectionViewModel : ReactiveObject
{
    private string _selectedPoint = "Click on a data point to see its value";

    public PointSelectionViewModel()
    {
        Chart = new LineChart
        {
            LineWidth = 2f,
            ShowMarkers = true,
            MarkerSize = 8f
        };

        // Create sample data
        var points = new List<IDataPoint>();
        var random = new Random(42);

        for (int i = 0; i <= 20; i++)
        {
            double x = i;
            double y = 50 + Math.Sin(i * 0.5) * 20 + random.Next(-5, 5);
            points.Add(new DataPoint(x, y));
        }

        var series = new DataSeries<IDataPoint>(points, "Interactive Data");
        Chart.Series.Add(series);
    }

    public LineChart Chart { get; }

    public string SelectedPoint
    {
        get => _selectedPoint;
        set => this.RaiseAndSetIfChanged(ref _selectedPoint, value);
    }

    // This would be called by chart interaction events
    public void OnPointClicked(double x, double y)
    {
        SelectedPoint = $"Selected Point: X={x:F1}, Y={y:F2}";
    }
}
