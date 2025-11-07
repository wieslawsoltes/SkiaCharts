using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.BasicCharts;

public class ScatterChartViewModel : ReactiveObject
{
    public ScatterChartViewModel()
    {
        // Create scatter chart
        Chart = new ScatterChart
        {
            Configuration = new ScatterChartConfiguration
            {
                ShowConnectingLines = false
            },
            DefaultStyle = new ScatterSeriesStyle
            {
                MarkerShape = MarkerShape.Circle,
                MarkerSize = 8f,
                FillColor = SKColors.Tomato,
                BorderColor = SKColors.DarkRed,
                BorderWidth = 1f
            }
        };

        // Create sample data - random scatter points
        var points = new List<IDataPoint>();
        var random = new Random(42);

        for (int i = 0; i < 50; i++)
        {
            double x = random.Next(0, 100);
            double y = random.Next(0, 100);
            points.Add(new DataPoint(x, y));
        }

        var series = new DataSeries<IDataPoint>(points, "Random Data");
        Chart.Series.Add(series);
    }

    public ScatterChart Chart { get; }
}
