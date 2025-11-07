using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.BasicCharts;

public class AreaChartViewModel : ReactiveObject
{
    public AreaChartViewModel()
    {
        // Create area chart
        Chart = new AreaChart
        {
            Configuration = new AreaChartConfiguration
            {
                StackMode = AreaStackMode.None
            },
            DefaultStyle = new AreaSeriesStyle
            {
                FillColor = SKColors.DodgerBlue,
                FillAlpha = 100,
                LineColor = SKColors.DodgerBlue,
                LineWidth = 2f,
                AreaMode = AreaMode.Linear,
                ShowLine = true
            }
        };

        // Create sample data - temperature variation
        var points = new List<IDataPoint>();
        var random = new Random(42);

        for (int i = 0; i <= 30; i++)
        {
            double x = i;
            double y = 20 + Math.Sin(i * 0.3) * 10 + random.Next(-3, 3);
            points.Add(new DataPoint(x, y));
        }

        var series = new DataSeries<IDataPoint>(points, "Temperature");
        Chart.Series.Add(series);
    }

    public AreaChart Chart { get; }
}
