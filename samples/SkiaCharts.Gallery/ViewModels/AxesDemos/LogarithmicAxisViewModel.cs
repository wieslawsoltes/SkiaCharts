using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.AxesDemos;

public class LogarithmicAxisViewModel : ReactiveObject
{
    public LogarithmicAxisViewModel()
    {
        Chart = new LineChart
        {
            LineWidth = 2.5f,
            ShowMarkers = true,
            MarkerSize = 6f
        };

        // Create exponential data that benefits from log scale (y = 10^x)
        var points = new List<IDataPoint>();
        for (int i = 0; i <= 10; i++)
        {
            double x = i;
            double y = Math.Pow(10, i * 0.5); // 1, 3.16, 10, 31.6, 100, 316, 1000...
            points.Add(new DataPoint(x, y));
        }

        var series = new DataSeries<IDataPoint>(points, "Exponential Growth (10^(x/2))");
        Chart.Series.Add(series);
    }

    public LineChart Chart { get; }
}
