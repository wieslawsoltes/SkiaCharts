using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.BasicCharts;

public class StackedColumnViewModel : ReactiveObject
{
    public StackedColumnViewModel()
    {
        // Create stacked column chart
        Chart = new BarChart
        {
            Configuration = new BarChartConfiguration
            {
                Orientation = BarOrientation.Vertical,
                StackMode = BarStackMode.Absolute
            }
        };

        // Create multiple series for stacking
        var series1 = CreateSeries("Q1", SKColors.DodgerBlue, 1);
        var series2 = CreateSeries("Q2", SKColors.MediumSeaGreen, 2);
        var series3 = CreateSeries("Q3", SKColors.Orange, 3);

        Chart.SetSeriesStyle(series1, new BarSeriesStyle
        {
            FillColor = SKColors.DodgerBlue,
            CornerRadius = 0f,
            BarWidthRatio = 0.7
        });
        Chart.SetSeriesStyle(series2, new BarSeriesStyle
        {
            FillColor = SKColors.MediumSeaGreen,
            CornerRadius = 0f,
            BarWidthRatio = 0.7
        });
        Chart.SetSeriesStyle(series3, new BarSeriesStyle
        {
            FillColor = SKColors.Orange,
            CornerRadius = 0f,
            BarWidthRatio = 0.7
        });

        Chart.Series.Add(series1);
        Chart.Series.Add(series2);
        Chart.Series.Add(series3);
    }

    private IDataSeries<IDataPoint> CreateSeries(string name, SKColor color, int quarter)
    {
        var random = new Random(42 + quarter);
        var points = new List<IDataPoint>();

        for (int i = 1; i <= 6; i++)
        {
            double x = i;
            double y = 20 + random.Next(10, 30);
            points.Add(new DataPoint(x, y));
        }

        return new DataSeries<IDataPoint>(points, name);
    }

    public BarChart Chart { get; }
}
