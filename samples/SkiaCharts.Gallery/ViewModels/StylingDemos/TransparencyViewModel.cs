using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.StylingDemos;

public class TransparencyViewModel : ReactiveObject
{
    public TransparencyViewModel()
    {
        Chart = new AreaChart
        {
            Configuration = new AreaChartConfiguration
            {
                StackMode = AreaStackMode.None
            }
        };

        // Create 3 overlapping area series with different transparency levels
        for (int i = 0; i < 3; i++)
        {
            var points = new List<IDataPoint>();
            for (int j = 0; j <= 20; j++)
            {
                double x = j;
                double y = 30 + (i * 15) + (j * 2);
                points.Add(new DataPoint(x, y));
            }
            var series = new DataSeries<IDataPoint>(points, $"Series {i + 1}");
            Chart.Series.Add(series);

            // Set transparency level
            byte alpha = (byte)(50 + (i * 70)); // 50, 120, 190
            Chart.SetSeriesStyle(series, new AreaSeriesStyle
            {
                FillColor = i == 0 ? SKColors.Blue : i == 1 ? SKColors.Green : SKColors.Red,
                FillAlpha = alpha,
                LineWidth = 2f,
                ShowLine = true
            });
        }
    }

    public AreaChart Chart { get; }
}
