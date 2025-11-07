using System;
using System.Collections.Generic;
using System.Diagnostics;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.PerformanceDemos;

public class ScatterPerformanceViewModel : ReactiveObject
{
    private string _performanceInfo = "Loading...";

    public ScatterPerformanceViewModel()
    {
        Chart = new ScatterChart
        {
            Configuration = new ScatterChartConfiguration
            {
                ShowConnectingLines = false
            },
            DefaultStyle = new ScatterSeriesStyle
            {
                MarkerShape = MarkerShape.Circle,
                MarkerSize = 3f,
                FillColor = new SKColor(33, 150, 243),
                BorderWidth = 0f
            }
        };

        LoadData();
    }

    public ScatterChart Chart { get; }

    public string PerformanceInfo
    {
        get => _performanceInfo;
        set => this.RaiseAndSetIfChanged(ref _performanceInfo, value);
    }

    private void LoadData()
    {
        var stopwatch = Stopwatch.StartNew();

        // Create 100,000 scatter points
        var points = new List<IDataPoint>(100_000);
        var random = new Random(42);

        for (int i = 0; i < 100_000; i++)
        {
            double x = random.NextDouble() * 1000;
            double y = random.NextDouble() * 1000;
            points.Add(new DataPoint(x, y));
        }

        var series = new DataSeries<IDataPoint>(points, "100K Scatter Points");
        Chart.Series.Add(series);

        stopwatch.Stop();
        PerformanceInfo = $"Loaded 100,000 scatter points in {stopwatch.ElapsedMilliseconds}ms";
    }
}
