using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.BasicCharts;

public class SimpleLineViewModel : ReactiveObject
{
    public SimpleLineViewModel()
    {
        // Create simple line chart with sample data
        Chart = new LineChart
        {
            LineColor = SKColors.DodgerBlue,
            LineWidth = 2f,
            ShowMarkers = true,
            MarkerSize = 6f
        };

        // Create sample data - simple sine wave
        var points = new List<IDataPoint>();
        for (int i = 0; i <= 50; i++)
        {
            double x = i;
            double y = Math.Sin(i * 0.2) * 50 + 50;
            points.Add(new DataPoint(x, y));
        }

        var series = new DataSeries<IDataPoint>(points, "Sine Wave");
        Chart.Series.Add(series);
    }

    public LineChart Chart { get; }
}
