using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.CustomDemos;

public class CustomThemeViewModel : ReactiveObject
{
    public CustomThemeViewModel()
    {
        // Create custom themed chart
        Chart = new LineChart
        {
            LineWidth = 3f,
            ShowMarkers = true,
            MarkerSize = 8f,
            LineColor = new SKColor(255, 87, 34)  // Deep Orange
        };

        // Create sample data
        var points = new List<IDataPoint>();
        var random = new Random(42);

        for (int i = 0; i <= 30; i++)
        {
            double x = i;
            double y = 50 + Math.Sin(i * 0.3) * 25 + random.Next(-5, 6);
            points.Add(new DataPoint(x, y));
        }

        var series = new DataSeries<IDataPoint>(points, "Custom Themed Data");
        Chart.Series.Add(series);
    }

    public LineChart Chart { get; }
}
