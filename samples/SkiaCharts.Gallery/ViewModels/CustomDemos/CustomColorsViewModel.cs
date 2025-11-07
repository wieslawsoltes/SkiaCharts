using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.CustomDemos;

public class CustomColorsViewModel : ReactiveObject
{
    public CustomColorsViewModel()
    {
        Chart = new LineChart
        {
            LineWidth = 2.5f,
            ShowMarkers = true,
            MarkerSize = 7f
        };

        // Create three series with custom color palette
        var customColors = new[]
        {
            new SKColor(0, 150, 136),    // Teal
            new SKColor(121, 85, 72),    // Brown
            new SKColor(158, 158, 158)   // Gray
        };

        var seriesNames = new[] { "Product A", "Product B", "Product C" };
        var random = new Random(42);

        for (int s = 0; s < 3; s++)
        {
            var points = new List<IDataPoint>();
            double baseValue = 40 + s * 15;

            for (int i = 0; i <= 20; i++)
            {
                double x = i;
                double y = baseValue + Math.Sin(i * 0.4 + s) * 10 + random.Next(-3, 4);
                points.Add(new DataPoint(x, y));
            }

            var series = new DataSeries<IDataPoint>(points, seriesNames[s]);
            Chart.Series.Add(series);
        }
    }

    public LineChart Chart { get; }
}
