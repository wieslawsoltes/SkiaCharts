using SkiaCharts.Core.Data;
using System.Diagnostics;

namespace SkiaCharts.Core.Streaming;

/// <summary>
/// Base class for streaming data sources with built-in rate limiting and throttling.
/// </summary>
public abstract class StreamingDataSourceBase : IStreamingDataSource
{
    private StreamingState _state = StreamingState.Disconnected;
    private readonly object _stateLock = new();
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _streamingTask;
    private readonly Stopwatch _throttleStopwatch = new();
    private double _minUpdateInterval; // milliseconds
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamingDataSourceBase"/> class.
    /// </summary>
    protected StreamingDataSourceBase()
    {
        MaxUpdateFrequency = 60; // 60 FPS default
        UpdateMinInterval();
    }

    /// <inheritdoc/>
    public event EventHandler<DataPointsEventArgs>? DataReceived;

    /// <inheritdoc/>
    public event EventHandler<StreamingErrorEventArgs>? Error;

    /// <inheritdoc/>
    public event EventHandler<ConnectionStateEventArgs>? StateChanged;

    /// <inheritdoc/>
    public StreamingState State
    {
        get
        {
            lock (_stateLock)
            {
                return _state;
            }
        }
        private set
        {
            StreamingState oldState;
            lock (_stateLock)
            {
                if (_state == value)
                    return;

                oldState = _state;
                _state = value;
            }

            StateChanged?.Invoke(this, new ConnectionStateEventArgs(oldState, value));
        }
    }

    private double _maxUpdateFrequency;
    /// <inheritdoc/>
    public double MaxUpdateFrequency
    {
        get => _maxUpdateFrequency;
        set
        {
            _maxUpdateFrequency = value;
            UpdateMinInterval();
        }
    }

    /// <inheritdoc/>
    public virtual async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (State == StreamingState.Connected || State == StreamingState.Connecting)
            return;

        State = StreamingState.Connecting;
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        try
        {
            await ConnectAsync(_cancellationTokenSource.Token);
            State = StreamingState.Connected;

            // Start streaming loop
            _throttleStopwatch.Start();
            _streamingTask = StreamDataAsync(_cancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            State = StreamingState.Error;
            OnError(ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task StopAsync()
    {
        if (State == StreamingState.Disconnected)
            return;

        _cancellationTokenSource?.Cancel();

        if (_streamingTask != null)
        {
            try
            {
                await _streamingTask;
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
        }

        await DisconnectAsync();
        State = StreamingState.Disconnected;
        _throttleStopwatch.Stop();
    }

    /// <inheritdoc/>
    public virtual void Pause()
    {
        if (State == StreamingState.Connected)
        {
            State = StreamingState.Paused;
        }
    }

    /// <inheritdoc/>
    public virtual void Resume()
    {
        if (State == StreamingState.Paused)
        {
            State = StreamingState.Connected;
        }
    }

    /// <summary>
    /// Raises the DataReceived event with rate limiting.
    /// </summary>
    /// <param name="points">The new data points.</param>
    protected void OnDataReceived(IEnumerable<IDataPoint> points)
    {
        if (State != StreamingState.Connected)
            return;

        // Rate limiting - check if enough time has passed
        if (MaxUpdateFrequency > 0)
        {
            var elapsed = _throttleStopwatch.Elapsed.TotalMilliseconds;
            if (elapsed < _minUpdateInterval)
            {
                // Too soon, skip this update
                return;
            }

            _throttleStopwatch.Restart();
        }

        DataReceived?.Invoke(this, new DataPointsEventArgs(points));
    }

    /// <summary>
    /// Raises the Error event.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    protected void OnError(Exception exception)
    {
        State = StreamingState.Error;
        Error?.Invoke(this, new StreamingErrorEventArgs(exception));
    }

    /// <summary>
    /// Abstract method to establish connection to the data source.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    protected abstract Task ConnectAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Abstract method to disconnect from the data source.
    /// </summary>
    protected abstract Task DisconnectAsync();

    /// <summary>
    /// Abstract method to continuously stream data from the source.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    protected abstract Task StreamDataAsync(CancellationToken cancellationToken);

    private void UpdateMinInterval()
    {
        _minUpdateInterval = MaxUpdateFrequency > 0
            ? 1000.0 / MaxUpdateFrequency
            : 0;
    }

    /// <summary>
    /// Disposes the streaming data source.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the streaming data source.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            StopAsync().Wait();
            _cancellationTokenSource?.Dispose();
        }

        _disposed = true;
    }
}
