using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.MultiSeriesDemos;

public class StackedAreaViewModel : ReactiveObject
{
    public StackedAreaViewModel()
    {
        Chart = new AreaChart
        {
            Configuration = new AreaChartConfiguration
            {
                StackMode = AreaStackMode.Stacked
            }
        };

        // Create three stacked area series
        var series1 = CreateSeries("Product A", SKColors.DodgerBlue, 0);
        var series2 = CreateSeries("Product B", SKColors.MediumSeaGreen, 1);
        var series3 = CreateSeries("Product C", SKColors.Coral, 2);

        Chart.Series.Add(series1);
        Chart.Series.Add(series2);
        Chart.Series.Add(series3);

        // Apply styles
        Chart.SetSeriesStyle(series1, new AreaSeriesStyle
        {
            FillColor = SKColors.DodgerBlue,
            FillAlpha = 180,
            LineWidth = 2f,
            ShowLine = true
        });

        Chart.SetSeriesStyle(series2, new AreaSeriesStyle
        {
            FillColor = SKColors.MediumSeaGreen,
            FillAlpha = 180,
            LineWidth = 2f,
            ShowLine = true
        });

        Chart.SetSeriesStyle(series3, new AreaSeriesStyle
        {
            FillColor = SKColors.Coral,
            FillAlpha = 180,
            LineWidth = 2f,
            ShowLine = true
        });
    }

    public AreaChart Chart { get; }

    private IDataSeries<IDataPoint> CreateSeries(string name, SKColor color, int offset)
    {
        var points = new List<IDataPoint>();
        var random = new Random(42 + offset);

        for (int i = 0; i <= 24; i++)
        {
            double x = i;
            double baseValue = 20 + offset * 5;
            double trend = i * 0.5;
            double noise = (random.NextDouble() - 0.5) * 5;
            double y = baseValue + trend + noise;
            points.Add(new DataPoint(x, y));
        }

        return new DataSeries<IDataPoint>(points, name);
    }
}
