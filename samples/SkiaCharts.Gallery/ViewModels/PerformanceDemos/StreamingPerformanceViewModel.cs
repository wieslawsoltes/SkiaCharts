using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;

namespace SkiaCharts.Gallery.ViewModels.PerformanceDemos;

public class StreamingPerformanceViewModel : ReactiveObject, IDisposable
{
    private readonly IDisposable _updateSubscription;
    private readonly List<IDataPoint> _dataPoints;
    private readonly Stopwatch _fpsStopwatch;
    private int _frameCount;
    private double _currentX;
    private double _currentValue = 50;
    private readonly Random _random = new(42);
    private const int MaxDataPoints = 1000;
    private string _performanceInfo = "Starting...";

    public StreamingPerformanceViewModel()
    {
        Chart = new LineChart
        {
            LineWidth = 1.5f,
            ShowMarkers = false
        };

        _dataPoints = new List<IDataPoint>();
        _fpsStopwatch = Stopwatch.StartNew();

        // Very fast updates - 60 FPS
        _updateSubscription = Observable
            .Interval(TimeSpan.FromMilliseconds(16))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => UpdateData());
    }

    public LineChart Chart { get; }

    public string PerformanceInfo
    {
        get => _performanceInfo;
        set => this.RaiseAndSetIfChanged(ref _performanceInfo, value);
    }

    private void UpdateData()
    {
        // Add new point
        _currentValue += (_random.NextDouble() - 0.5) * 10;
        _currentValue = Math.Clamp(_currentValue, 0, 100);
        _dataPoints.Add(new DataPoint(_currentX++, _currentValue));

        // Remove old points
        if (_dataPoints.Count > MaxDataPoints)
        {
            _dataPoints.RemoveAt(0);
        }

        // Update series
        Chart.Series.Clear();
        var series = new DataSeries<IDataPoint>(_dataPoints, "High-Speed Stream");
        Chart.Series.Add(series);

        this.RaisePropertyChanged(nameof(Chart));

        // Calculate FPS
        _frameCount++;
        if (_fpsStopwatch.ElapsedMilliseconds >= 1000)
        {
            double fps = _frameCount / (_fpsStopwatch.ElapsedMilliseconds / 1000.0);
            PerformanceInfo = $"FPS: {fps:F1} | Points: {_dataPoints.Count} | Target: 60 FPS";
            _frameCount = 0;
            _fpsStopwatch.Restart();
        }
    }

    public void Dispose()
    {
        _updateSubscription?.Dispose();
    }
}
