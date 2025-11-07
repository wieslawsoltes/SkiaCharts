using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Streaming;

/// <summary>
/// Represents a data source that provides streaming, real-time data updates.
/// </summary>
public interface IStreamingDataSource : IDisposable
{
    /// <summary>
    /// Event raised when new data points are available.
    /// </summary>
    event EventHandler<DataPointsEventArgs>? DataReceived;

    /// <summary>
    /// Event raised when the data source encounters an error.
    /// </summary>
    event EventHandler<StreamingErrorEventArgs>? Error;

    /// <summary>
    /// Event raised when the connection state changes.
    /// </summary>
    event EventHandler<ConnectionStateEventArgs>? StateChanged;

    /// <summary>
    /// Gets the current connection state.
    /// </summary>
    StreamingState State { get; }

    /// <summary>
    /// Gets or sets the maximum update frequency in Hz (updates per second).
    /// Default is 60 (60 FPS). Set to 0 for unlimited.
    /// </summary>
    double MaxUpdateFrequency { get; set; }

    /// <summary>
    /// Starts streaming data from the source.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to stop streaming.</param>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops streaming data from the source.
    /// </summary>
    Task StopAsync();

    /// <summary>
    /// Pauses streaming without disconnecting.
    /// </summary>
    void Pause();

    /// <summary>
    /// Resumes streaming after a pause.
    /// </summary>
    void Resume();
}

/// <summary>
/// Represents the state of a streaming data source.
/// </summary>
public enum StreamingState
{
    /// <summary>
    /// The source is disconnected and not streaming.
    /// </summary>
    Disconnected,

    /// <summary>
    /// The source is connecting.
    /// </summary>
    Connecting,

    /// <summary>
    /// The source is connected and streaming data.
    /// </summary>
    Connected,

    /// <summary>
    /// The source is paused.
    /// </summary>
    Paused,

    /// <summary>
    /// The source encountered an error.
    /// </summary>
    Error
}

/// <summary>
/// Event args for data received events.
/// </summary>
public class DataPointsEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataPointsEventArgs"/> class.
    /// </summary>
    /// <param name="points">The new data points.</param>
    public DataPointsEventArgs(IEnumerable<IDataPoint> points)
    {
        Points = points.ToList();
        Timestamp = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the new data points.
    /// </summary>
    public IReadOnlyList<IDataPoint> Points { get; }

    /// <summary>
    /// Gets the timestamp when the data was received.
    /// </summary>
    public DateTime Timestamp { get; }
}

/// <summary>
/// Event args for streaming errors.
/// </summary>
public class StreamingErrorEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StreamingErrorEventArgs"/> class.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    public StreamingErrorEventArgs(Exception exception)
    {
        Exception = exception;
        Timestamp = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the exception that occurred.
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    /// Gets the timestamp when the error occurred.
    /// </summary>
    public DateTime Timestamp { get; }
}

/// <summary>
/// Event args for connection state changes.
/// </summary>
public class ConnectionStateEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionStateEventArgs"/> class.
    /// </summary>
    /// <param name="oldState">The old state.</param>
    /// <param name="newState">The new state.</param>
    public ConnectionStateEventArgs(StreamingState oldState, StreamingState newState)
    {
        OldState = oldState;
        NewState = newState;
        Timestamp = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the previous state.
    /// </summary>
    public StreamingState OldState { get; }

    /// <summary>
    /// Gets the new state.
    /// </summary>
    public StreamingState NewState { get; }

    /// <summary>
    /// Gets the timestamp of the state change.
    /// </summary>
    public DateTime Timestamp { get; }
}
