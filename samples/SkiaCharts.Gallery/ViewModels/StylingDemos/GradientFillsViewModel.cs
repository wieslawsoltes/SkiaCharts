using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.StylingDemos;

public class GradientFillsViewModel : ReactiveObject
{
    public GradientFillsViewModel()
    {
        // Create bar chart with gradient fills
        Chart = new BarChart
        {
            Configuration = new BarChartConfiguration
            {
                Orientation = BarOrientation.Vertical,
                StackMode = BarStackMode.None
            }
        };

        // Create 5 series with different gradient fills
        for (int i = 0; i < 5; i++)
        {
            var points = new List<IDataPoint>
            {
                new DataPoint(i + 1, 60 + (i * 10))
            };
            var series = new DataSeries<IDataPoint>(points, $"Series {i + 1}");
            Chart.Series.Add(series);

            // Set gradient style for each series
            Chart.SetSeriesStyle(series, new BarSeriesStyle
            {
                GradientColors = GetGradientColors(i),
                GradientAngle = 90f,
                CornerRadius = 6f,
                BarWidthRatio = 0.75
            });
        }
    }

    public BarChart Chart { get; }

    private SKColor[] GetGradientColors(int index)
    {
        return index switch
        {
            0 => new[] { new SKColor(135, 206, 250), new SKColor(0, 0, 139) }, // Light to Dark Blue
            1 => new[] { new SKColor(144, 238, 144), new SKColor(0, 100, 0) }, // Light to Dark Green
            2 => new[] { new SKColor(255, 182, 193), new SKColor(139, 0, 0) }, // Light to Dark Red
            3 => new[] { new SKColor(255, 215, 0), new SKColor(184, 134, 11) }, // Gold gradient
            4 => new[] { new SKColor(221, 160, 221), new SKColor(75, 0, 130) }, // Purple gradient
            _ => new[] { SKColors.LightGray, SKColors.DarkGray }
        };
    }
}
