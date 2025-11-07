using System;
using System.Collections.Generic;
using System.Diagnostics;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;

namespace SkiaCharts.Gallery.ViewModels.PerformanceDemos;

public class MultiSeriesPerformanceViewModel : ReactiveObject
{
    private string _performanceInfo = "Loading...";

    public MultiSeriesPerformanceViewModel()
    {
        Chart = new LineChart
        {
            LineWidth = 1f,
            ShowMarkers = false
        };

        LoadData();
    }

    public LineChart Chart { get; }

    public string PerformanceInfo
    {
        get => _performanceInfo;
        set => this.RaiseAndSetIfChanged(ref _performanceInfo, value);
    }

    private void LoadData()
    {
        var stopwatch = Stopwatch.StartNew();

        // Create 50 series with 10,000 points each = 500K total points
        int seriesCount = 50;
        int pointsPerSeries = 10_000;
        var random = new Random(42);

        for (int s = 0; s < seriesCount; s++)
        {
            var points = new List<IDataPoint>(pointsPerSeries);
            double offset = s * 10;
            double phase = s * 0.1;

            for (int i = 0; i < pointsPerSeries; i++)
            {
                double x = i;
                double y = offset + Math.Sin((i + phase * 100) * 0.01) * 5;
                points.Add(new DataPoint(x, y));
            }

            var series = new DataSeries<IDataPoint>(points, $"Series {s + 1}");
            Chart.Series.Add(series);
        }

        stopwatch.Stop();
        PerformanceInfo = $"Loaded {seriesCount} series × {pointsPerSeries:N0} points = {seriesCount * pointsPerSeries:N0} total in {stopwatch.ElapsedMilliseconds}ms";
    }
}
