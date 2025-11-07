using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Streaming;

/// <summary>
/// A simulated streaming data source for testing and demo purposes.
/// Generates random data points at a specified interval.
/// </summary>
public class SimulatedStreamingDataSource : StreamingDataSourceBase
{
    private readonly Random _random = new(42);
    private double _currentValue = 100.0;
    private int _currentX = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimulatedStreamingDataSource"/> class.
    /// </summary>
    public SimulatedStreamingDataSource()
    {
        UpdateInterval = TimeSpan.FromMilliseconds(100); // 10 updates per second
        Volatility = 2.0; // Amount of random change per update
    }

    /// <summary>
    /// Gets or sets the interval between data updates.
    /// </summary>
    public TimeSpan UpdateInterval { get; set; }

    /// <summary>
    /// Gets or sets the volatility (amount of random change) in the generated data.
    /// </summary>
    public double Volatility { get; set; }

    /// <summary>
    /// Gets or sets whether to generate OHLC data (4 values per point).
    /// </summary>
    public bool GenerateOHLC { get; set; }

    /// <inheritdoc/>
    protected override Task ConnectAsync(CancellationToken cancellationToken)
    {
        // Simulated connection - immediate
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override Task DisconnectAsync()
    {
        // Simulated disconnection - immediate
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override async Task StreamDataAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Wait for the update interval
                await Task.Delay(UpdateInterval, cancellationToken);

                if (State == StreamingState.Paused)
                    continue;

                // Generate new data point
                var points = GenerateOHLC ? GenerateOHLCPoint() : GenerateSinglePoint();

                // Send data with rate limiting handled by base class
                OnDataReceived(points);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                OnError(ex);
                break;
            }
        }
    }

    private IEnumerable<IDataPoint> GenerateSinglePoint()
    {
        // Random walk
        var change = (_random.NextDouble() - 0.5) * Volatility;
        _currentValue += change;

        yield return new DataPoint(_currentX++, _currentValue);
    }

    private IEnumerable<IDataPoint> GenerateOHLCPoint()
    {
        // Generate OHLC candlestick data
        var open = _currentValue;
        var change = (_random.NextDouble() - 0.5) * Volatility * 4;
        var close = open + change;

        var high = Math.Max(open, close) + _random.NextDouble() * Volatility;
        var low = Math.Min(open, close) - _random.NextDouble() * Volatility;

        _currentValue = close;

        // Create OHLC point (Y = close for compatibility, but we store OHLC in additional values)
        var point = new DataPoint(_currentX++, close)
        {
            // Note: DataPoint would need to be extended to support OHLC properly
            // For now, we just use close as Y
        };

        yield return point;
    }

    /// <summary>
    /// Resets the simulation to initial values.
    /// </summary>
    public void Reset()
    {
        _currentX = 0;
        _currentValue = 100.0;
    }
}
