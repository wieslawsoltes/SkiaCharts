using System.Collections;

namespace SkiaCharts.Core.Data;

/// <summary>
/// Represents an immutable data series with efficient access and automatic bounds calculation.
/// </summary>
/// <typeparam name="T">The type of data points in the series.</typeparam>
public class DataSeries<T> : IDataSeries<T> where T : IDataPoint
{
    private readonly List<T> _points;
    private double _minX;
    private double _maxX;
    private double _minY;
    private double _maxY;
    private bool _areBoundsValid;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSeries{T}"/> class.
    /// </summary>
    /// <param name="points">The collection of data points.</param>
    /// <param name="name">The name of the series.</param>
    public DataSeries(IEnumerable<T> points, string? name = null)
    {
        _points = new List<T>(points);
        Name = name;
        _areBoundsValid = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSeries{T}"/> class.
    /// </summary>
    /// <param name="name">The name of the series.</param>
    public DataSeries(string? name = null)
    {
        _points = new List<T>();
        Name = name;
        _areBoundsValid = false;
    }

    /// <inheritdoc/>
    public string? Name { get; }

    /// <inheritdoc/>
    public int Count => _points.Count;

    /// <inheritdoc/>
    public T this[int index] => _points[index];

    /// <inheritdoc/>
    public double MinX
    {
        get
        {
            EnsureBoundsCalculated();
            return _minX;
        }
    }

    /// <inheritdoc/>
    public double MaxX
    {
        get
        {
            EnsureBoundsCalculated();
            return _maxX;
        }
    }

    /// <inheritdoc/>
    public double MinY
    {
        get
        {
            EnsureBoundsCalculated();
            return _minY;
        }
    }

    /// <inheritdoc/>
    public double MaxY
    {
        get
        {
            EnsureBoundsCalculated();
            return _maxY;
        }
    }

    /// <inheritdoc/>
    public bool AreBoundsValid => _areBoundsValid;

    /// <inheritdoc/>
    public void InvalidateBounds()
    {
        _areBoundsValid = false;
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => _points.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void EnsureBoundsCalculated()
    {
        if (_areBoundsValid || _points.Count == 0)
        {
            return;
        }

        _minX = double.MaxValue;
        _maxX = double.MinValue;
        _minY = double.MaxValue;
        _maxY = double.MinValue;

        foreach (var point in _points)
        {
            if (point.X < _minX) _minX = point.X;
            if (point.X > _maxX) _maxX = point.X;
            if (point.Y < _minY) _minY = point.Y;
            if (point.Y > _maxY) _maxY = point.Y;
        }

        _areBoundsValid = true;
    }
}
