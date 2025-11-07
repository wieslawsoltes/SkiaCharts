using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace SkiaCharts.Core.Data;

/// <summary>
/// Represents an observable data series that notifies when its contents change.
/// Suitable for real-time data updates.
/// </summary>
/// <typeparam name="T">The type of data points in the series.</typeparam>
public class ObservableDataSeries<T> : IDataSeries<T>, INotifyCollectionChanged, INotifyPropertyChanged
    where T : IDataPoint
{
    private readonly List<T> _points;
    private double _minX;
    private double _maxX;
    private double _minY;
    private double _maxY;
    private bool _areBoundsValid;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableDataSeries{T}"/> class.
    /// </summary>
    /// <param name="name">The name of the series.</param>
    public ObservableDataSeries(string? name = null)
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

    /// <summary>
    /// Occurs when the collection changes.
    /// </summary>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Adds a data point to the series.
    /// </summary>
    /// <param name="point">The point to add.</param>
    public void Add(T point)
    {
        _points.Add(point);
        InvalidateBounds();
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(
            NotifyCollectionChangedAction.Add, point, _points.Count - 1));
        OnPropertyChanged(nameof(Count));
    }

    /// <summary>
    /// Adds multiple data points to the series.
    /// </summary>
    /// <param name="points">The points to add.</param>
    public void AddRange(IEnumerable<T> points)
    {
        var pointsList = points.ToList();
        _points.AddRange(pointsList);
        InvalidateBounds();
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(
            NotifyCollectionChangedAction.Add, pointsList));
        OnPropertyChanged(nameof(Count));
    }

    /// <summary>
    /// Removes all data points from the series.
    /// </summary>
    public void Clear()
    {
        _points.Clear();
        InvalidateBounds();
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        OnPropertyChanged(nameof(Count));
    }

    /// <summary>
    /// Removes data points at the beginning of the series up to the specified count.
    /// Useful for maintaining a rolling window of data.
    /// </summary>
    /// <param name="count">The number of points to remove from the beginning.</param>
    public void RemoveRange(int count)
    {
        if (count > 0 && count <= _points.Count)
        {
            _points.RemoveRange(0, count);
            InvalidateBounds();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(nameof(Count));
        }
    }

    /// <inheritdoc/>
    public void InvalidateBounds()
    {
        _areBoundsValid = false;
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => _points.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Raises the CollectionChanged event.
    /// </summary>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        CollectionChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

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
