using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.MultiSeriesDemos;

public class MixedTypesViewModel : ReactiveObject
{
    public MixedTypesViewModel()
    {
        // Create line chart
        LineChart = new LineChart
        {
            LineWidth = 2.5f,
            ShowMarkers = true,
            MarkerSize = 6f
        };

        var linePoints = new List<DataPoint>();
        for (int i = 0; i <= 12; i++)
        {
            double x = i;
            double y = 30 + Math.Sin(i * 0.5) * 15;
            linePoints.Add(new DataPoint(x, y));
        }
        var lineSeries = new DataSeries<DataPoint>(linePoints, "Trend");
        LineChart.Series.Add(lineSeries);

        // Create bar chart
        BarChart = new BarChart
        {
            Configuration = new BarChartConfiguration
            {
                Orientation = BarOrientation.Vertical,
                StackMode = BarStackMode.None
            },
            DefaultStyle = new BarSeriesStyle
            {
                BarWidthRatio = 0.7,
                CornerRadius = 4f
            }
        };

        var barPoints = new List<DataPoint>();
        var random = new Random(42);
        for (int i = 1; i <= 12; i++)
        {
            double x = i;
            double y = 20 + random.Next(0, 30);
            barPoints.Add(new DataPoint(x, y));
        }
        var barSeries = new DataSeries<DataPoint>(barPoints, "Values");
        BarChart.Series.Add(barSeries);

        // Create area chart
        AreaChart = new AreaChart
        {
            Configuration = new AreaChartConfiguration
            {
                StackMode = AreaStackMode.None
            },
            DefaultStyle = new AreaSeriesStyle
            {
                LineWidth = 2f,
                ShowLine = true,
                FillAlpha = 100
            }
        };

        var areaPoints = new List<DataPoint>();
        for (int i = 0; i <= 12; i++)
        {
            double x = i;
            double y = 40 + Math.Cos(i * 0.5) * 10;
            areaPoints.Add(new DataPoint(x, y));
        }
        var areaSeries = new DataSeries<DataPoint>(areaPoints, "Range");
        AreaChart.Series.Add(areaSeries);
    }

    public LineChart LineChart { get; }
    public BarChart BarChart { get; }
    public AreaChart AreaChart { get; }
}
