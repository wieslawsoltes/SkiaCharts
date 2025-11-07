using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaCharts.Core.Theming;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.MultiSeriesDemos;

public class ManySeriesViewModel : ReactiveObject
{
    public ManySeriesViewModel()
    {
        Chart = new LineChart
        {
            LineWidth = 1.5f,
            ShowMarkers = false,
            MarkerSize = 3f
        };

        // Create 100 series with different trends
        var palette = ColorPalettes.Vibrant;
        var random = new Random(42);

        for (int seriesIndex = 0; seriesIndex < 100; seriesIndex++)
        {
            var points = new List<DataPoint>();
            double startValue = 50 + random.Next(-20, 20);
            double trend = (random.NextDouble() - 0.5) * 2; // -1 to +1

            for (int i = 0; i <= 50; i++)
            {
                double x = i;
                double noise = (random.NextDouble() - 0.5) * 5;
                double y = startValue + (i * trend) + noise;
                points.Add(new DataPoint(x, y));
            }

            var series = new DataSeries<DataPoint>(points, $"Series {seriesIndex + 1}");
            Chart.Series.Add(series);
        }
    }

    public LineChart Chart { get; }
}
