using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.AxesDemos;

public class LinearAxisViewModel : ReactiveObject
{
    public LinearAxisViewModel()
    {
        Chart = new LineChart
        {
            LineWidth = 2f,
            ShowMarkers = true,
            MarkerSize = 6f
        };

        // Create linear data (y = 2x + 10)
        var points = new List<IDataPoint>();
        for (int i = 0; i <= 20; i++)
        {
            double x = i;
            double y = 2 * x + 10;
            points.Add(new DataPoint(x, y));
        }

        var series = new DataSeries<IDataPoint>(points, "Linear Function (y = 2x + 10)");
        Chart.Series.Add(series);
    }

    public LineChart Chart { get; }
}
