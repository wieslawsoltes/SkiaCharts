using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.BasicCharts;

public class GroupedBarViewModel : ReactiveObject
{
    public GroupedBarViewModel()
    {
        // Create grouped bar chart (horizontal)
        Chart = new BarChart
        {
            Configuration = new BarChartConfiguration
            {
                Orientation = BarOrientation.Horizontal,
                StackMode = BarStackMode.None,
                GroupSpacing = 0.3
            }
        };

        // Create multiple series for grouping
        var series1 = CreateSeries("2023", SKColors.DodgerBlue);
        var series2 = CreateSeries("2024", SKColors.MediumSeaGreen);

        Chart.SetSeriesStyle(series1, new BarSeriesStyle
        {
            FillColor = SKColors.DodgerBlue,
            CornerRadius = 4f,
            BarWidthRatio = 0.8
        });
        Chart.SetSeriesStyle(series2, new BarSeriesStyle
        {
            FillColor = SKColors.MediumSeaGreen,
            CornerRadius = 4f,
            BarWidthRatio = 0.8
        });

        Chart.Series.Add(series1);
        Chart.Series.Add(series2);
    }

    private IDataSeries<IDataPoint> CreateSeries(string name, SKColor color)
    {
        var random = new Random(name == "2023" ? 42 : 84);
        var points = new List<IDataPoint>();

        for (int i = 1; i <= 5; i++)
        {
            double x = i;
            double y = 30 + random.Next(10, 40);
            points.Add(new DataPoint(x, y));
        }

        return new DataSeries<IDataPoint>(points, name);
    }

    public BarChart Chart { get; }
}
