using System.Collections;

namespace SkiaCharts.Core.Data;

/// <summary>
/// Represents a fixed-size circular buffer that overwrites old data when full.
/// Optimized for streaming data scenarios with a maximum capacity.
/// </summary>
/// <typeparam name="T">The type of elements in the buffer.</typeparam>
public class CircularBuffer<T> : IReadOnlyList<T>
{
    private readonly T[] _buffer;
    private int _start;
    private int _count;

    /// <summary>
    /// Initializes a new instance of the <see cref="CircularBuffer{T}"/> class.
    /// </summary>
    /// <param name="capacity">The maximum capacity of the buffer.</param>
    public CircularBuffer(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentException("Capacity must be greater than zero.", nameof(capacity));
        }

        _buffer = new T[capacity];
        _start = 0;
        _count = 0;
    }

    /// <summary>
    /// Gets the maximum capacity of the buffer.
    /// </summary>
    public int Capacity => _buffer.Length;

    /// <summary>
    /// Gets the current number of elements in the buffer.
    /// </summary>
    public int Count => _count;

    /// <summary>
    /// Gets a value indicating whether the buffer is full.
    /// </summary>
    public bool IsFull => _count == Capacity;

    /// <summary>
    /// Gets a value indicating whether the buffer is empty.
    /// </summary>
    public bool IsEmpty => _count == 0;

    /// <summary>
    /// Gets the element at the specified logical index.
    /// </summary>
    /// <param name="index">The zero-based logical index.</param>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var physicalIndex = (_start + index) % Capacity;
            return _buffer[physicalIndex];
        }
    }

    /// <summary>
    /// Adds an item to the end of the buffer.
    /// If the buffer is full, the oldest item is overwritten.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void Add(T item)
    {
        var endIndex = (_start + _count) % Capacity;
        _buffer[endIndex] = item;

        if (_count == Capacity)
        {
            // Buffer is full, move start forward (overwrite oldest)
            _start = (_start + 1) % Capacity;
        }
        else
        {
            _count++;
        }
    }

    /// <summary>
    /// Removes and returns the oldest item from the buffer.
    /// </summary>
    /// <returns>The oldest item.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the buffer is empty.</exception>
    public T RemoveFirst()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException("Buffer is empty.");
        }

        var item = _buffer[_start];
        _buffer[_start] = default!;
        _start = (_start + 1) % Capacity;
        _count--;

        return item;
    }

    /// <summary>
    /// Clears all items from the buffer.
    /// </summary>
    public void Clear()
    {
        Array.Clear(_buffer, 0, _buffer.Length);
        _start = 0;
        _count = 0;
    }

    /// <summary>
    /// Gets the newest (most recently added) item without removing it.
    /// </summary>
    /// <returns>The newest item.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the buffer is empty.</exception>
    public T PeekLast()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException("Buffer is empty.");
        }

        var lastIndex = (_start + _count - 1) % Capacity;
        return _buffer[lastIndex];
    }

    /// <summary>
    /// Gets the oldest item without removing it.
    /// </summary>
    /// <returns>The oldest item.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the buffer is empty.</exception>
    public T PeekFirst()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException("Buffer is empty.");
        }

        return _buffer[_start];
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _count; i++)
        {
            yield return this[i];
        }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Converts the buffer to an array in logical order.
    /// </summary>
    /// <returns>An array containing all elements in the buffer.</returns>
    public T[] ToArray()
    {
        var result = new T[_count];
        for (int i = 0; i < _count; i++)
        {
            result[i] = this[i];
        }
        return result;
    }
}
