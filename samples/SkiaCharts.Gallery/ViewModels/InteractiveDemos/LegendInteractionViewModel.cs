using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.InteractiveDemos;

public class LegendInteractionViewModel : ReactiveObject
{
    private readonly List<IDataSeries<IDataPoint>> _allSeries;
    private bool _series1Visible = true;
    private bool _series2Visible = true;
    private bool _series3Visible = true;

    public LegendInteractionViewModel()
    {
        Chart = new LineChart
        {
            LineWidth = 2.5f,
            ShowMarkers = true,
            MarkerSize = 6f
        };

        // Create three series
        _allSeries = new List<IDataSeries<IDataPoint>>
        {
            CreateSeries("Revenue", new SKColor(33, 150, 243), 0),
            CreateSeries("Expenses", new SKColor(244, 67, 54), 0.5),
            CreateSeries("Profit", new SKColor(76, 175, 80), 1.0)
        };

        UpdateVisibleSeries();
    }

    public LineChart Chart { get; }

    public bool Series1Visible
    {
        get => _series1Visible;
        set
        {
            this.RaiseAndSetIfChanged(ref _series1Visible, value);
            UpdateVisibleSeries();
        }
    }

    public bool Series2Visible
    {
        get => _series2Visible;
        set
        {
            this.RaiseAndSetIfChanged(ref _series2Visible, value);
            UpdateVisibleSeries();
        }
    }

    public bool Series3Visible
    {
        get => _series3Visible;
        set
        {
            this.RaiseAndSetIfChanged(ref _series3Visible, value);
            UpdateVisibleSeries();
        }
    }

    private IDataSeries<IDataPoint> CreateSeries(string name, SKColor color, double phase)
    {
        var points = new List<IDataPoint>();
        var random = new Random(name.GetHashCode());

        for (int i = 0; i <= 30; i++)
        {
            double x = i;
            double y = 50 + Math.Sin((i + phase * 10) * 0.3) * 25 + random.Next(-3, 4);
            points.Add(new DataPoint(x, y));
        }

        var series = new DataSeries<IDataPoint>(points, name);
        return series;
    }

    private void UpdateVisibleSeries()
    {
        Chart.Series.Clear();

        if (Series1Visible)
            Chart.Series.Add(_allSeries[0]);

        if (Series2Visible)
            Chart.Series.Add(_allSeries[1]);

        if (Series3Visible)
            Chart.Series.Add(_allSeries[2]);

        this.RaisePropertyChanged(nameof(Chart));
    }
}
