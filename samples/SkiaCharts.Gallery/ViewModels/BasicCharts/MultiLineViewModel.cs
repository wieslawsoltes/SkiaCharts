using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.BasicCharts;

public class MultiLineViewModel : ReactiveObject
{
    public MultiLineViewModel()
    {
        // Create multi-line chart
        Chart = new LineChart
        {
            LineWidth = 2f,
            ShowMarkers = true,
            MarkerSize = 4f
        };

        // Create multiple series
        var series1 = CreateSeries("Revenue", SKColors.DodgerBlue, 0);
        var series2 = CreateSeries("Expenses", SKColors.Tomato, 1);
        var series3 = CreateSeries("Profit", SKColors.MediumSeaGreen, 2);

        Chart.Series.Add(series1);
        Chart.Series.Add(series2);
        Chart.Series.Add(series3);
    }

    private DataSeries<IDataPoint> CreateSeries(string name, SKColor color, int offset)
    {
        var points = new List<IDataPoint>();
        var random = new Random(42 + offset);

        for (int i = 0; i <= 20; i++)
        {
            double x = i;
            double y = 50 + offset * 10 + random.Next(-15, 15) + i * 2;
            points.Add(new DataPoint(x, y));
        }

        return new DataSeries<IDataPoint>(points, name);
    }

    public LineChart Chart { get; }
}
