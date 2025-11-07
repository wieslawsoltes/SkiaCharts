using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.CustomDemos;

public class CustomStylingViewModel : ReactiveObject
{
    public CustomStylingViewModel()
    {
        // Create custom styled bar chart
        Chart = new BarChart
        {
            Configuration = new BarChartConfiguration
            {
                Orientation = BarOrientation.Horizontal,
                StackMode = BarStackMode.None
            },
            DefaultStyle = new BarSeriesStyle
            {
                FillColor = new SKColor(156, 39, 176),  // Purple
                BorderColor = new SKColor(74, 20, 140),  // Dark Purple
                BorderWidth = 2f,
                CornerRadius = 8f,
                BarWidthRatio = 0.7
            }
        };

        // Create sample data
        var points = new List<IDataPoint>();
        var categories = new[] { "Q1", "Q2", "Q3", "Q4" };
        var values = new[] { 45.0, 67.0, 55.0, 82.0 };

        for (int i = 0; i < categories.Length; i++)
        {
            points.Add(new DataPoint(i, values[i]));
        }

        var series = new DataSeries<IDataPoint>(points, "Custom Styled Bars");
        Chart.Series.Add(series);
    }

    public BarChart Chart { get; }
}
