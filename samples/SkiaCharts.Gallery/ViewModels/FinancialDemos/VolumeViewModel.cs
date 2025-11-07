using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.FinancialDemos;

public class VolumeViewModel : ReactiveObject
{
    public VolumeViewModel()
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
                FillColor = new SKColor(96, 125, 139), // Blue Grey
                CornerRadius = 0f,
                BarWidthRatio = 0.9
            }
        };

        // Generate sample trading volume data
        var points = new List<IDataPoint>();
        var random = new Random(42);

        for (int i = 0; i < 30; i++)
        {
            double x = i;
            // Volume varies between 500K and 2M shares
            double volume = 500000 + random.Next(0, 1500000);
            points.Add(new DataPoint(x, volume));
        }

        var series = new DataSeries<IDataPoint>(points, "Trading Volume");
        Chart.Series.Add(series);
    }

    public BarChart Chart { get; }
}
