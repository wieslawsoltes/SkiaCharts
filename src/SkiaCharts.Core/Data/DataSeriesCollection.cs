using System.Collections;

namespace SkiaCharts.Core.Data;

/// <summary>
/// Represents a collection of data series with efficient access and bounds calculation.
/// </summary>
public class DataSeriesCollection : IReadOnlyList<IDataSeries<IDataPoint>>
{
    private readonly List<IDataSeries<IDataPoint>> _series;
    private DataRange? _xRange;
    private DataRange? _yRange;
    private bool _areBoundsValid;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSeriesCollection"/> class.
    /// </summary>
    public DataSeriesCollection()
    {
        _series = new List<IDataSeries<IDataPoint>>();
        _areBoundsValid = false;
    }

    /// <summary>
    /// Gets the number of series in the collection.
    /// </summary>
    public int Count => _series.Count;

    /// <summary>
    /// Gets the series at the specified index.
    /// </summary>
    public IDataSeries<IDataPoint> this[int index] => _series[index];

    /// <summary>
    /// Gets the combined X range across all series.
    /// </summary>
    public DataRange XRange
    {
        get
        {
            EnsureBoundsCalculated();
            return _xRange ?? DataRange.Empty;
        }
    }

    /// <summary>
    /// Gets the combined Y range across all series.
    /// </summary>
    public DataRange YRange
    {
        get
        {
            EnsureBoundsCalculated();
            return _yRange ?? DataRange.Empty;
        }
    }

    /// <summary>
    /// Adds a series to the collection.
    /// </summary>
    /// <typeparam name="T">The type of data points in the series.</typeparam>
    /// <param name="series">The series to add.</param>
    public void Add<T>(IDataSeries<T> series) where T : IDataPoint
    {
        _series.Add((IDataSeries<IDataPoint>)series);
        _areBoundsValid = false;
    }

    /// <summary>
    /// Removes a series from the collection.
    /// </summary>
    /// <param name="series">The series to remove.</param>
    /// <returns>True if the series was removed; otherwise, false.</returns>
    public bool Remove(IDataSeries<IDataPoint> series)
    {
        var removed = _series.Remove(series);
        if (removed)
        {
            _areBoundsValid = false;
        }
        return removed;
    }

    /// <summary>
    /// Removes all series from the collection.
    /// </summary>
    public void Clear()
    {
        _series.Clear();
        _areBoundsValid = false;
        _xRange = null;
        _yRange = null;
    }

    /// <summary>
    /// Invalidates cached bounds, forcing recalculation on next access.
    /// </summary>
    public void InvalidateBounds()
    {
        _areBoundsValid = false;
    }

    /// <inheritdoc/>
    public IEnumerator<IDataSeries<IDataPoint>> GetEnumerator() => _series.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void EnsureBoundsCalculated()
    {
        if (_areBoundsValid || _series.Count == 0)
        {
            return;
        }

        double minX = double.MaxValue;
        double maxX = double.MinValue;
        double minY = double.MaxValue;
        double maxY = double.MinValue;

        foreach (var series in _series)
        {
            if (series.Count == 0)
            {
                continue;
            }

            if (series.MinX < minX) minX = series.MinX;
            if (series.MaxX > maxX) maxX = series.MaxX;
            if (series.MinY < minY) minY = series.MinY;
            if (series.MaxY > maxY) maxY = series.MaxY;
        }

        if (minX != double.MaxValue && maxX != double.MinValue)
        {
            _xRange = new DataRange(minX, maxX);
            _yRange = new DataRange(minY, maxY);
        }
        else
        {
            _xRange = DataRange.Empty;
            _yRange = DataRange.Empty;
        }

        _areBoundsValid = true;
    }
}
