using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.AxesDemos;

public class DateTimeAxisViewModel : ReactiveObject
{
    public DateTimeAxisViewModel()
    {
        Chart = new LineChart
        {
            LineWidth = 2f,
            ShowMarkers = true,
            MarkerSize = 5f
        };

        // Create time series data - temperature readings over 24 hours
        var points = new List<IDataPoint>();
        var baseTime = new DateTime(2025, 1, 1, 0, 0, 0);
        var random = new Random(42);

        for (int hour = 0; hour < 24; hour++)
        {
            var time = baseTime.AddHours(hour);
            // Simulate temperature variation throughout the day
            double temp = 15 + 8 * Math.Sin((hour - 6) * Math.PI / 12) + random.Next(-2, 3);
            points.Add(new DataPoint(time.ToOADate(), temp));
        }

        var series = new DataSeries<IDataPoint>(points, "Temperature (°C)");
        Chart.Series.Add(series);
    }

    public LineChart Chart { get; }
}
