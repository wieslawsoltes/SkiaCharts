using System;
using System.Collections.Generic;
using System.Diagnostics;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;

namespace SkiaCharts.Gallery.ViewModels.PerformanceDemos;

public class MillionPointsViewModel : ReactiveObject
{
    private string _performanceInfo = "Loading...";

    public MillionPointsViewModel()
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

        // Create 1 million data points
        var points = new List<IDataPoint>(1_000_000);

        for (int i = 0; i < 1_000_000; i++)
        {
            double x = i;
            double y = 50 + Math.Sin(i * 0.001) * 30 + Math.Cos(i * 0.0005) * 20;
            points.Add(new DataPoint(x, y));
        }

        var series = new DataSeries<IDataPoint>(points, "1M Points");
        Chart.Series.Add(series);

        stopwatch.Stop();
        PerformanceInfo = $"Loaded 1,000,000 points in {stopwatch.ElapsedMilliseconds}ms";
    }
}
