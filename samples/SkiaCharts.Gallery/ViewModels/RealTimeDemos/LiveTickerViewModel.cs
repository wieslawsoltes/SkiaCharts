using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.RealTimeDemos;

public class LiveTickerViewModel : ReactiveObject, IDisposable
{
    private readonly IDisposable _updateSubscription;
    private readonly Random _random = new Random();
    private readonly List<IDataPoint> _prices = new List<IDataPoint>();
    private int _tick = 0;
    private double _currentPrice = 100.0;
    private string _status = "Initializing...";

    public LiveTickerViewModel()
    {
        Chart = new LineChart
        {
            LineWidth = 2.5f,
            ShowMarkers = true,
            MarkerSize = 5f,
            LineColor = new SKColor(33, 150, 243) // Blue
        };

        // Initialize with recent price history
        for (int i = 0; i < 30; i++)
        {
            _prices.Add(new DataPoint(_tick++, _currentPrice));
            _currentPrice += (_random.NextDouble() - 0.5) * 2;
        }

        UpdateChart();

        // Update every 500ms for a ticker-like feel
        _updateSubscription = Observable
            .Interval(TimeSpan.FromMilliseconds(500))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => UpdatePrice());
    }

    public LineChart Chart { get; }

    public string Status
    {
        get => _status;
        private set => this.RaiseAndSetIfChanged(ref _status, value);
    }

    private void UpdatePrice()
    {
        // Simulate price movement
        double change = (_random.NextDouble() - 0.5) * 3;
        _currentPrice += change;
        _currentPrice = Math.Max(90, Math.Min(110, _currentPrice)); // Keep in range

        _prices.Add(new DataPoint(_tick++, _currentPrice));

        // Keep last 50 prices
        if (_prices.Count > 50)
        {
            _prices.RemoveAt(0);
        }

        UpdateChart();

        // Update status
        string direction = change > 0 ? "▲" : change < 0 ? "▼" : "━";
        Status = $"${_currentPrice:F2} {direction} {(change >= 0 ? "+" : "")}{change:F2}";
    }

    private void UpdateChart()
    {
        Chart.Series.Clear();
        var series = new DataSeries<IDataPoint>(_prices, "Live Price");
        Chart.Series.Add(series);
        this.RaisePropertyChanged(nameof(Chart));
    }

    public void Dispose()
    {
        _updateSubscription?.Dispose();
    }
}
