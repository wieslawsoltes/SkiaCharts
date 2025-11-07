using SkiaCharts.Core.Data;
using System.Collections;

namespace SkiaCharts.Core.Streaming;

/// <summary>
/// A data series optimized for streaming data with time-based windowing and efficient buffering.
/// </summary>
/// <typeparam name="T">The type of data points.</typeparam>
public class StreamingDataSeries<T> : IDataSeries<T> where T : IDataPoint
{
    private readonly CircularDataBuffer<T> _buffer;
    private IStreamingDataSource? _dataSource;
    private TimeSpan? _timeWindow;
    private bool _boundsValid;
    private double _minX, _maxX, _minY, _maxY;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamingDataSeries{T}"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of data points to store.</param>
    /// <param name="name">The name of the series.</param>
    public StreamingDataSeries(int capacity, string? name = null)
    {
        _buffer = new CircularDataBuffer<T>(capacity);
        Name = name;
    }

    /// <inheritdoc/>
    public string? Name { get; set; }

    /// <inheritdoc/>
    public int Count => GetVisiblePoints().Count();

    /// <inheritdoc/>
    public T this[int index] => GetVisiblePoints().ElementAt(index);

    /// <inheritdoc/>
    public double MinX
    {
        get
        {
            EnsureBounds();
            return _minX;
        }
    }

    /// <inheritdoc/>
    public double MaxX
    {
        get
        {
            EnsureBounds();
            return _maxX;
        }
    }

    /// <inheritdoc/>
    public double MinY
    {
        get
        {
            EnsureBounds();
            return _minY;
        }
    }

    /// <inheritdoc/>
    public double MaxY
    {
        get
        {
            EnsureBounds();
            return _maxY;
        }
    }

    /// <inheritdoc/>
    public bool AreBoundsValid => _boundsValid;

    /// <inheritdoc/>
    public void InvalidateBounds()
    {
        _boundsValid = false;
    }

    /// <summary>
    /// Gets or sets the time window for displaying data.
    /// If set, only data within the window relative to the newest point is shown.
    /// </summary>
    public TimeSpan? TimeWindow
    {
        get => _timeWindow;
        set
        {
            _timeWindow = value;
            OnPointsChanged();
        }
    }

    /// <summary>
    /// Gets the capacity of the underlying buffer.
    /// </summary>
    public int Capacity => _buffer.Capacity;

    /// <summary>
    /// Event raised when points in the series change.
    /// </summary>
    public event EventHandler? PointsChanged;

    /// <summary>
    /// Attaches a streaming data source to this series.
    /// </summary>
    /// <param name="dataSource">The data source to attach.</param>
    public void AttachDataSource(IStreamingDataSource dataSource)
    {
        DetachDataSource();

        _dataSource = dataSource;
        _dataSource.DataReceived += OnDataReceived;
    }

    /// <summary>
    /// Detaches the current streaming data source.
    /// </summary>
    public void DetachDataSource()
    {
        if (_dataSource != null)
        {
            _dataSource.DataReceived -= OnDataReceived;
            _dataSource = null;
        }
    }

    /// <summary>
    /// Adds a single data point to the series.
    /// </summary>
    /// <param name="point">The data point to add.</param>
    public void AddPoint(T point)
    {
        _buffer.Add(point);
        OnPointsChanged();
    }

    /// <summary>
    /// Adds multiple data points to the series.
    /// </summary>
    /// <param name="points">The data points to add.</param>
    public void AddPoints(IEnumerable<T> points)
    {
        _buffer.AddRange(points);
        OnPointsChanged();
    }

    /// <summary>
    /// Clears all data from the series.
    /// </summary>
    public void Clear()
    {
        _buffer.Clear();
        OnPointsChanged();
    }

    /// <summary>
    /// Gets all data points in the buffer.
    /// </summary>
    /// <returns>A list of all data points.</returns>
    public List<T> GetAllPoints()
    {
        return _buffer.GetAll();
    }

    /// <summary>
    /// Gets the last N data points.
    /// </summary>
    /// <param name="count">The number of points to retrieve.</param>
    /// <returns>A list of the most recent data points.</returns>
    public List<T> GetLastPoints(int count)
    {
        return _buffer.GetLast(count);
    }

    private IEnumerable<T> GetVisiblePoints()
    {
        if (_timeWindow == null)
        {
            return _buffer.GetAll();
        }

        // Time-based windowing
        var newest = _buffer.GetNewest();
        if (newest == null)
            return Array.Empty<T>();

        double windowStart = newest.X - _timeWindow.Value.TotalSeconds;
        return _buffer.GetWindow(windowStart, newest.X);
    }

    private void OnDataReceived(object? sender, DataPointsEventArgs e)
    {
        // Convert IDataPoint to T
        var points = e.Points.OfType<T>();
        _buffer.AddRange(points);
        OnPointsChanged();
    }

    private void OnPointsChanged()
    {
        InvalidateBounds();
        PointsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void EnsureBounds()
    {
        if (_boundsValid)
            return;

        var points = GetVisiblePoints().ToList();
        if (points.Count == 0)
        {
            _minX = _maxX = _minY = _maxY = 0;
            _boundsValid = true;
            return;
        }

        _minX = points.Min(p => p.X);
        _maxX = points.Max(p => p.X);
        _minY = points.Min(p => p.Y);
        _maxY = points.Max(p => p.Y);
        _boundsValid = true;
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return GetVisiblePoints().GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
