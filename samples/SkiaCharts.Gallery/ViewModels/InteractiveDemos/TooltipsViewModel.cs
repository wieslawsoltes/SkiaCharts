using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.InteractiveDemos;

public class TooltipsViewModel : ReactiveObject
{
    private string _hoveredPoint = "Hover over data points to see tooltips";

    public TooltipsViewModel()
    {
        Chart = new AreaChart
        {
            Configuration = new AreaChartConfiguration
            {
                StackMode = AreaStackMode.None
            },
            DefaultStyle = new AreaSeriesStyle
            {
                FillColor = new SKColor(33, 150, 243),
                FillAlpha = 120,
                LineColor = new SKColor(33, 150, 243),
                LineWidth = 2.5f,
                AreaMode = AreaMode.Linear,
                ShowLine = true
            }
        };

        // Create sample data with interesting points
        var points = new List<IDataPoint>();
        var random = new Random(42);

        for (int i = 0; i <= 40; i++)
        {
            double x = i;
            double y = 50 + Math.Sin(i * 0.25) * 30 + random.Next(-5, 6);
            points.Add(new DataPoint(x, y));
        }

        var series = new DataSeries<IDataPoint>(points, "Sales Data");
        Chart.Series.Add(series);
    }

    public AreaChart Chart { get; }

    public string HoveredPoint
    {
        get => _hoveredPoint;
        set => this.RaiseAndSetIfChanged(ref _hoveredPoint, value);
    }

    // This would be called by chart hover events
    public void OnPointHovered(double x, double y)
    {
        HoveredPoint = $"Value: {y:F2} at position {x:F1}";
    }

    public void OnChartLeave()
    {
        HoveredPoint = "Hover over data points to see tooltips";
    }
}
