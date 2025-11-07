using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.RealTimeDemos;

public class StreamingLineViewModel : ReactiveObject, IDisposable
{
    private readonly IDisposable _updateSubscription;
    private readonly Random _random = new Random();
    private readonly List<IDataPoint> _dataPoints = new List<IDataPoint>();
    private int _currentX = 0;
    private double _currentValue = 50.0;
    private const int MaxDataPoints = 100;

    public StreamingLineViewModel()
    {
        Chart = new LineChart
        {
            LineWidth = 2f,
            ShowMarkers = false,
            MarkerSize = 4f
        };

        // Initialize with some data
        for (int i = 0; i < 20; i++)
        {
            _dataPoints.Add(new DataPoint(_currentX++, _currentValue));
            _currentValue += (_random.NextDouble() - 0.5) * 5;
        }

        var series = new DataSeries<IDataPoint>(_dataPoints, "Real-Time Data");
        Chart.Series.Add(series);

        // Update data every 100ms
        _updateSubscription = Observable
            .Interval(TimeSpan.FromMilliseconds(100))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => UpdateData());
    }

    public LineChart Chart { get; }

    private void UpdateData()
    {
        // Add new point
        _currentValue += (_random.NextDouble() - 0.5) * 10;
        _currentValue = Math.Clamp(_currentValue, 0, 100);
        _dataPoints.Add(new DataPoint(_currentX++, _currentValue));

        // Remove old points if too many
        if (_dataPoints.Count > MaxDataPoints)
        {
            _dataPoints.RemoveAt(0);
        }

        // Update the series
        Chart.Series.Clear();
        var series = new DataSeries<IDataPoint>(_dataPoints, "Real-Time Data");
        Chart.Series.Add(series);

        // Notify UI to refresh
        this.RaisePropertyChanged(nameof(Chart));
    }

    public void Dispose()
    {
        _updateSubscription?.Dispose();
    }
}
