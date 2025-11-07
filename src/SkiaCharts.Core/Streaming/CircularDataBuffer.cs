using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Streaming;

/// <summary>
/// A circular buffer for storing streaming data points with fixed capacity.
/// When the buffer is full, the oldest data is overwritten.
/// </summary>
/// <typeparam name="T">The type of data points.</typeparam>
public class CircularDataBuffer<T> where T : IDataPoint
{
    private readonly T[] _buffer;
    private int _head;
    private int _tail;
    private int _count;
    private readonly object _lock = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CircularDataBuffer{T}"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of data points to store.</param>
    public CircularDataBuffer(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero.", nameof(capacity));

        _buffer = new T[capacity];
        Capacity = capacity;
    }

    /// <summary>
    /// Gets the capacity of the buffer.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Gets the number of items currently in the buffer.
    /// </summary>
    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _count;
            }
        }
    }

    /// <summary>
    /// Gets whether the buffer is full.
    /// </summary>
    public bool IsFull => Count == Capacity;

    /// <summary>
    /// Gets whether the buffer is empty.
    /// </summary>
    public bool IsEmpty => Count == 0;

    /// <summary>
    /// Adds a data point to the buffer.
    /// If the buffer is full, the oldest point is overwritten.
    /// </summary>
    /// <param name="item">The data point to add.</param>
    public void Add(T item)
    {
        lock (_lock)
        {
            _buffer[_tail] = item;
            _tail = (_tail + 1) % Capacity;

            if (_count < Capacity)
            {
                _count++;
            }
            else
            {
                // Buffer is full, move head forward (overwrite oldest)
                _head = (_head + 1) % Capacity;
            }
        }
    }

    /// <summary>
    /// Adds multiple data points to the buffer.
    /// </summary>
    /// <param name="items">The data points to add.</param>
    public void AddRange(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }

    /// <summary>
    /// Gets all data points in the buffer in chronological order.
    /// </summary>
    /// <returns>A list of data points.</returns>
    public List<T> GetAll()
    {
        lock (_lock)
        {
            var result = new List<T>(_count);

            if (_count == 0)
                return result;

            int index = _head;
            for (int i = 0; i < _count; i++)
            {
                result.Add(_buffer[index]);
                index = (index + 1) % Capacity;
            }

            return result;
        }
    }

    /// <summary>
    /// Gets the last N data points from the buffer.
    /// </summary>
    /// <param name="count">The number of points to retrieve.</param>
    /// <returns>A list of the most recent data points.</returns>
    public List<T> GetLast(int count)
    {
        lock (_lock)
        {
            var actualCount = Math.Min(count, _count);
            var result = new List<T>(actualCount);

            if (actualCount == 0)
                return result;

            // Start from the position that is 'actualCount' before tail
            int startIndex = (_tail - actualCount + Capacity) % Capacity;
            int index = startIndex;

            for (int i = 0; i < actualCount; i++)
            {
                result.Add(_buffer[index]);
                index = (index + 1) % Capacity;
            }

            return result;
        }
    }

    /// <summary>
    /// Gets data points within a specific time window.
    /// </summary>
    /// <param name="startX">The start X value (inclusive).</param>
    /// <param name="endX">The end X value (inclusive).</param>
    /// <returns>A list of data points within the window.</returns>
    public List<T> GetWindow(double startX, double endX)
    {
        lock (_lock)
        {
            var result = new List<T>();

            if (_count == 0)
                return result;

            int index = _head;
            for (int i = 0; i < _count; i++)
            {
                var point = _buffer[index];
                if (point.X >= startX && point.X <= endX)
                {
                    result.Add(point);
                }
                index = (index + 1) % Capacity;
            }

            return result;
        }
    }

    /// <summary>
    /// Clears all data from the buffer.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _head = 0;
            _tail = 0;
            _count = 0;
            Array.Clear(_buffer, 0, _buffer.Length);
        }
    }

    /// <summary>
    /// Gets the newest data point in the buffer.
    /// </summary>
    /// <returns>The newest data point, or default if buffer is empty.</returns>
    public T? GetNewest()
    {
        lock (_lock)
        {
            if (_count == 0)
                return default;

            int lastIndex = (_tail - 1 + Capacity) % Capacity;
            return _buffer[lastIndex];
        }
    }

    /// <summary>
    /// Gets the oldest data point in the buffer.
    /// </summary>
    /// <returns>The oldest data point, or default if buffer is empty.</returns>
    public T? GetOldest()
    {
        lock (_lock)
        {
            if (_count == 0)
                return default;

            return _buffer[_head];
        }
    }
}
