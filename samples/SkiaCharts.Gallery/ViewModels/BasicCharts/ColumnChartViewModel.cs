using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.BasicCharts;

public class ColumnChartViewModel : ReactiveObject
{
    public ColumnChartViewModel()
    {
        // Create column chart (vertical bars)
        Chart = new BarChart
        {
            Configuration = new BarChartConfiguration
            {
                Orientation = BarOrientation.Vertical,
                StackMode = BarStackMode.None
            },
            DefaultStyle = new BarSeriesStyle
            {
                FillColor = SKColors.DodgerBlue,
                CornerRadius = 4f,
                BarWidthRatio = 0.7
            }
        };

        // Create sample data - monthly sales
        var points = new List<IDataPoint>
        {
            new DataPoint(1, 45),
            new DataPoint(2, 62),
            new DataPoint(3, 58),
            new DataPoint(4, 71),
            new DataPoint(5, 85),
            new DataPoint(6, 92),
            new DataPoint(7, 78),
            new DataPoint(8, 88),
            new DataPoint(9, 95),
            new DataPoint(10, 102),
            new DataPoint(11, 89),
            new DataPoint(12, 110)
        };

        var series = new DataSeries<IDataPoint>(points, "Monthly Sales");
        Chart.Series.Add(series);
    }

    public BarChart Chart { get; }
}
