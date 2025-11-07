using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.BasicCharts;

public class BarChartViewModel : ReactiveObject
{
    public BarChartViewModel()
    {
        // Create bar chart (horizontal bars)
        Chart = new BarChart
        {
            Configuration = new BarChartConfiguration
            {
                Orientation = BarOrientation.Horizontal,
                StackMode = BarStackMode.None
            },
            DefaultStyle = new BarSeriesStyle
            {
                FillColor = SKColors.MediumSeaGreen,
                CornerRadius = 4f,
                BarWidthRatio = 0.6
            }
        };

        // Create sample data - product ratings
        var points = new List<IDataPoint>
        {
            new DataPoint(1, 8.5),
            new DataPoint(2, 7.8),
            new DataPoint(3, 9.2),
            new DataPoint(4, 6.9),
            new DataPoint(5, 8.8),
            new DataPoint(6, 7.5)
        };

        var series = new DataSeries<IDataPoint>(points, "Product Ratings");
        Chart.Series.Add(series);
    }

    public BarChart Chart { get; }
}
