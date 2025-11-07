using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.CustomDemos;

public class CustomMarkersViewModel : ReactiveObject
{
    public CustomMarkersViewModel()
    {
        Chart = new ScatterChart
        {
            Configuration = new ScatterChartConfiguration
            {
                ShowConnectingLines = false
            },
            DefaultStyle = new ScatterSeriesStyle
            {
                MarkerShape = MarkerShape.Circle,
                MarkerSize = 10f,
                FillColor = new SKColor(233, 30, 99),      // Pink
                BorderColor = new SKColor(194, 24, 91),    // Dark Pink
                BorderWidth = 2f
            }
        };

        // Create sample data with custom marker styling
        var points = new List<IDataPoint>();
        var random = new Random(42);

        for (int i = 0; i < 30; i++)
        {
            double x = random.NextDouble() * 100;
            double y = random.NextDouble() * 100;
            points.Add(new DataPoint(x, y));
        }

        var series = new DataSeries<IDataPoint>(points, "Custom Markers");
        Chart.Series.Add(series);
    }

    public ScatterChart Chart { get; }
}
