using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.AxesDemos;

public class CategoryAxisViewModel : ReactiveObject
{
    public CategoryAxisViewModel()
    {
        Chart = new BarChart
        {
            Configuration = new BarChartConfiguration
            {
                Orientation = BarOrientation.Vertical,
                StackMode = BarStackMode.None
            },
            DefaultStyle = new BarSeriesStyle
            {
                FillColor = SKColors.MediumSeaGreen,
                CornerRadius = 4f,
                BarWidthRatio = 0.7
            }
        };

        // Create categorical data - sales by product category
        // Category 1=Electronics, 2=Clothing, 3=Home&Garden, 4=Sports, 5=Books, 6=Toys
        var points = new List<IDataPoint>
        {
            new DataPoint(1, 145),
            new DataPoint(2, 98),
            new DataPoint(3, 167),
            new DataPoint(4, 82),
            new DataPoint(5, 121),
            new DataPoint(6, 134)
        };

        var series = new DataSeries<IDataPoint>(points, "Sales by Category");
        Chart.Series.Add(series);
    }

    public BarChart Chart { get; }
}
