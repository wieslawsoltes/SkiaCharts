namespace SkiaCharts.Core.Data;

/// <summary>
/// Represents a transformation that can be applied to data points.
/// </summary>
public interface IDataTransform
{
    /// <summary>
    /// Transforms a data point.
    /// </summary>
    /// <param name="point">The input point.</param>
    /// <returns>The transformed point.</returns>
    IDataPoint Transform(IDataPoint point);
}

/// <summary>
/// Pipeline for applying multiple transformations to data series.
/// </summary>
public class DataTransformPipeline
{
    private readonly List<IDataTransform> _transforms = new();

    /// <summary>
    /// Adds a transform to the pipeline.
    /// </summary>
    /// <param name="transform">The transform to add.</param>
    /// <returns>This pipeline for chaining.</returns>
    public DataTransformPipeline Add(IDataTransform transform)
    {
        _transforms.Add(transform);
        return this;
    }

    /// <summary>
    /// Applies all transforms in the pipeline to a data point.
    /// </summary>
    /// <param name="point">The input point.</param>
    /// <returns>The transformed point.</returns>
    public IDataPoint Apply(IDataPoint point)
    {
        var result = point;
        foreach (var transform in _transforms)
        {
            result = transform.Transform(result);
        }
        return result;
    }

    /// <summary>
    /// Applies all transforms to an entire series.
    /// </summary>
    /// <typeparam name="T">The data point type.</typeparam>
    /// <param name="series">The input series.</param>
    /// <returns>A new series with transformed points.</returns>
    public IDataSeries<T> Apply<T>(IDataSeries<T> series) where T : IDataPoint
    {
        var points = new List<T>();
        for (int i = 0; i < series.Count; i++)
        {
            var transformed = Apply(series[i]);
            if (transformed is T typedPoint)
            {
                points.Add(typedPoint);
            }
        }
        return new DataSeries<T>(points);
    }

    /// <summary>
    /// Clears all transforms from the pipeline.
    /// </summary>
    public void Clear()
    {
        _transforms.Clear();
    }

    /// <summary>
    /// Gets the number of transforms in the pipeline.
    /// </summary>
    public int Count => _transforms.Count;
}

/// <summary>
/// Scales data points by a factor.
/// </summary>
public class ScaleTransform : IDataTransform
{
    /// <summary>
    /// Gets or sets the X scale factor.
    /// </summary>
    public double ScaleX { get; set; } = 1.0;

    /// <summary>
    /// Gets or sets the Y scale factor.
    /// </summary>
    public double ScaleY { get; set; } = 1.0;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScaleTransform"/> class.
    /// </summary>
    /// <param name="scaleX">The X scale factor.</param>
    /// <param name="scaleY">The Y scale factor.</param>
    public ScaleTransform(double scaleX = 1.0, double scaleY = 1.0)
    {
        ScaleX = scaleX;
        ScaleY = scaleY;
    }

    /// <inheritdoc/>
    public IDataPoint Transform(IDataPoint point)
    {
        return new DataPoint(point.X * ScaleX, point.Y * ScaleY);
    }
}

/// <summary>
/// Offsets data points by a constant.
/// </summary>
public class OffsetTransform : IDataTransform
{
    /// <summary>
    /// Gets or sets the X offset.
    /// </summary>
    public double OffsetX { get; set; }

    /// <summary>
    /// Gets or sets the Y offset.
    /// </summary>
    public double OffsetY { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OffsetTransform"/> class.
    /// </summary>
    /// <param name="offsetX">The X offset.</param>
    /// <param name="offsetY">The Y offset.</param>
    public OffsetTransform(double offsetX = 0, double offsetY = 0)
    {
        OffsetX = offsetX;
        OffsetY = offsetY;
    }

    /// <inheritdoc/>
    public IDataPoint Transform(IDataPoint point)
    {
        return new DataPoint(point.X + OffsetX, point.Y + OffsetY);
    }
}

/// <summary>
/// Normalizes data to a 0-1 range.
/// </summary>
public class NormalizeTransform : IDataTransform
{
    private readonly double _minX, _maxX, _minY, _maxY;

    /// <summary>
    /// Initializes a new instance of the <see cref="NormalizeTransform"/> class.
    /// </summary>
    /// <param name="minX">The minimum X value.</param>
    /// <param name="maxX">The maximum X value.</param>
    /// <param name="minY">The minimum Y value.</param>
    /// <param name="maxY">The maximum Y value.</param>
    public NormalizeTransform(double minX, double maxX, double minY, double maxY)
    {
        _minX = minX;
        _maxX = maxX;
        _minY = minY;
        _maxY = maxY;
    }

    /// <summary>
    /// Creates a normalize transform from a data series.
    /// </summary>
    /// <typeparam name="T">The data point type.</typeparam>
    /// <param name="series">The series to analyze.</param>
    /// <returns>A normalize transform.</returns>
    public static NormalizeTransform FromSeries<T>(IDataSeries<T> series) where T : IDataPoint
    {
        return new NormalizeTransform(series.MinX, series.MaxX, series.MinY, series.MaxY);
    }

    /// <inheritdoc/>
    public IDataPoint Transform(IDataPoint point)
    {
        var rangeX = _maxX - _minX;
        var rangeY = _maxY - _minY;

        var normalizedX = rangeX > 0 ? (point.X - _minX) / rangeX : 0;
        var normalizedY = rangeY > 0 ? (point.Y - _minY) / rangeY : 0;

        return new DataPoint(normalizedX, normalizedY);
    }
}

/// <summary>
/// Applies a logarithmic transformation to data.
/// </summary>
public class LogTransform : IDataTransform
{
    /// <summary>
    /// Gets or sets whether to transform X values.
    /// </summary>
    public bool TransformX { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to transform Y values.
    /// </summary>
    public bool TransformY { get; set; } = true;

    /// <summary>
    /// Gets or sets the logarithm base (default is natural log).
    /// </summary>
    public double Base { get; set; } = Math.E;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogTransform"/> class.
    /// </summary>
    /// <param name="transformX">Whether to transform X values.</param>
    /// <param name="transformY">Whether to transform Y values.</param>
    /// <param name="logBase">The logarithm base.</param>
    public LogTransform(bool transformX = true, bool transformY = true, double logBase = Math.E)
    {
        TransformX = transformX;
        TransformY = transformY;
        Base = logBase;
    }

    /// <inheritdoc/>
    public IDataPoint Transform(IDataPoint point)
    {
        var x = TransformX && point.X > 0 ? Math.Log(point.X, Base) : point.X;
        var y = TransformY && point.Y > 0 ? Math.Log(point.Y, Base) : point.Y;
        return new DataPoint(x, y);
    }
}

/// <summary>
/// Smooths data using a moving average.
/// </summary>
public class MovingAverageTransform : IDataTransform
{
    private readonly Queue<double> _windowY = new();
    private readonly int _windowSize;
    private double _sumY;

    /// <summary>
    /// Gets the window size for the moving average.
    /// </summary>
    public int WindowSize => _windowSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="MovingAverageTransform"/> class.
    /// </summary>
    /// <param name="windowSize">The number of points to average.</param>
    public MovingAverageTransform(int windowSize = 5)
    {
        if (windowSize < 1)
            throw new ArgumentException("Window size must be at least 1", nameof(windowSize));

        _windowSize = windowSize;
    }

    /// <inheritdoc/>
    public IDataPoint Transform(IDataPoint point)
    {
        _windowY.Enqueue(point.Y);
        _sumY += point.Y;

        if (_windowY.Count > _windowSize)
        {
            _sumY -= _windowY.Dequeue();
        }

        var averageY = _sumY / _windowY.Count;
        return new DataPoint(point.X, averageY);
    }

    /// <summary>
    /// Resets the moving average state.
    /// </summary>
    public void Reset()
    {
        _windowY.Clear();
        _sumY = 0;
    }
}

/// <summary>
/// Clamps data values to a specified range.
/// </summary>
public class ClampTransform : IDataTransform
{
    /// <summary>
    /// Gets or sets the minimum X value.
    /// </summary>
    public double? MinX { get; set; }

    /// <summary>
    /// Gets or sets the maximum X value.
    /// </summary>
    public double? MaxX { get; set; }

    /// <summary>
    /// Gets or sets the minimum Y value.
    /// </summary>
    public double? MinY { get; set; }

    /// <summary>
    /// Gets or sets the maximum Y value.
    /// </summary>
    public double? MaxY { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClampTransform"/> class.
    /// </summary>
    /// <param name="minX">The minimum X value.</param>
    /// <param name="maxX">The maximum X value.</param>
    /// <param name="minY">The minimum Y value.</param>
    /// <param name="maxY">The maximum Y value.</param>
    public ClampTransform(double? minX = null, double? maxX = null, double? minY = null, double? maxY = null)
    {
        MinX = minX;
        MaxX = maxX;
        MinY = minY;
        MaxY = maxY;
    }

    /// <inheritdoc/>
    public IDataPoint Transform(IDataPoint point)
    {
        var x = point.X;
        var y = point.Y;

        if (MinX.HasValue && x < MinX.Value) x = MinX.Value;
        if (MaxX.HasValue && x > MaxX.Value) x = MaxX.Value;
        if (MinY.HasValue && y < MinY.Value) y = MinY.Value;
        if (MaxY.HasValue && y > MaxY.Value) y = MaxY.Value;

        return new DataPoint(x, y);
    }
}
