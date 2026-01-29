using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Core.Rendering;

/// <summary>
/// Manages coordinate transformations between data space and screen space.
/// Handles viewport, zoom, and pan operations.
/// </summary>
public class ViewportManager
{
    private DataRange _xDataRange;
    private DataRange _yDataRange;
    private SKRect _screenRect;
    private Func<double, double>? _xTransform;
    private Func<double, double>? _xInverseTransform;
    private Func<double, double>? _yTransform;
    private Func<double, double>? _yInverseTransform;
    private double _xTransformMin;
    private double _yTransformMin;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewportManager"/> class.
    /// </summary>
    public ViewportManager()
    {
        _xDataRange = new DataRange(0, 1);
        _yDataRange = new DataRange(0, 1);
        _screenRect = new SKRect(0, 0, 100, 100);
        _xTransformMin = 0;
        _yTransformMin = 0;
    }

    /// <summary>
    /// Gets or sets the visible data range on the X axis.
    /// </summary>
    public DataRange XDataRange
    {
        get => _xDataRange;
        set
        {
            _xDataRange = value;
            UpdateTransform();
        }
    }

    /// <summary>
    /// Gets or sets the visible data range on the Y axis.
    /// </summary>
    public DataRange YDataRange
    {
        get => _yDataRange;
        set
        {
            _yDataRange = value;
            UpdateTransform();
        }
    }

    /// <summary>
    /// Gets or sets the screen rectangle for rendering.
    /// </summary>
    public SKRect ScreenRect
    {
        get => _screenRect;
        set
        {
            _screenRect = value;
            UpdateTransform();
        }
    }

    /// <summary>
    /// Gets the scale factor for the X axis (screen pixels per data unit).
    /// </summary>
    public float ScaleX { get; private set; }

    /// <summary>
    /// Gets the scale factor for the Y axis (screen pixels per data unit).
    /// </summary>
    public float ScaleY { get; private set; }

    /// <summary>
    /// Gets the translation offset for the X axis.
    /// </summary>
    public float OffsetX { get; private set; }

    /// <summary>
    /// Gets the translation offset for the Y axis.
    /// </summary>
    public float OffsetY { get; private set; }

    /// <summary>
    /// Sets a custom transformation for the X axis (e.g., log scale).
    /// </summary>
    /// <param name="transform">Transform applied to data values.</param>
    /// <param name="inverseTransform">Inverse transform applied to screen-to-data conversions.</param>
    public void SetXTransform(Func<double, double>? transform, Func<double, double>? inverseTransform)
    {
        _xTransform = transform;
        _xInverseTransform = inverseTransform;
        UpdateTransform();
    }

    /// <summary>
    /// Sets a custom transformation for the Y axis (e.g., log scale).
    /// </summary>
    /// <param name="transform">Transform applied to data values.</param>
    /// <param name="inverseTransform">Inverse transform applied to screen-to-data conversions.</param>
    public void SetYTransform(Func<double, double>? transform, Func<double, double>? inverseTransform)
    {
        _yTransform = transform;
        _yInverseTransform = inverseTransform;
        UpdateTransform();
    }

    /// <summary>
    /// Transforms a data point to screen coordinates.
    /// </summary>
    /// <param name="dataX">The data X coordinate.</param>
    /// <param name="dataY">The data Y coordinate.</param>
    /// <returns>The screen coordinates.</returns>
    public SKPoint DataToScreen(double dataX, double dataY)
    {
        var transformedX = ApplyTransformX(dataX);
        var transformedY = ApplyTransformY(dataY);

        if (!double.IsFinite(transformedX) || !double.IsFinite(transformedY))
        {
            return new SKPoint(float.NaN, float.NaN);
        }

        var screenX = _screenRect.Left + (float)((transformedX - _xTransformMin) * ScaleX);
        var screenY = _screenRect.Bottom - (float)((transformedY - _yTransformMin) * ScaleY);
        return new SKPoint(screenX, screenY);
    }

    /// <summary>
    /// Transforms screen coordinates to data space.
    /// </summary>
    /// <param name="screenX">The screen X coordinate.</param>
    /// <param name="screenY">The screen Y coordinate.</param>
    /// <returns>The data coordinates.</returns>
    public (double dataX, double dataY) ScreenToData(float screenX, float screenY)
    {
        var normalizedX = ScaleX == 0 ? 0 : (screenX - _screenRect.Left) / ScaleX;
        var normalizedY = ScaleY == 0 ? 0 : (_screenRect.Bottom - screenY) / ScaleY;
        var transformedX = _xTransformMin + normalizedX;
        var transformedY = _yTransformMin + normalizedY;
        var dataX = ApplyInverseTransformX(transformedX);
        var dataY = ApplyInverseTransformY(transformedY);
        return (dataX, dataY);
    }

    /// <summary>
    /// Transforms a data X coordinate to screen space.
    /// </summary>
    /// <param name="dataX">The data X coordinate.</param>
    /// <returns>The screen X coordinate.</returns>
    public float DataToScreenX(double dataX)
    {
        var transformedX = ApplyTransformX(dataX);
        if (!double.IsFinite(transformedX))
        {
            return float.NaN;
        }

        return _screenRect.Left + (float)((transformedX - _xTransformMin) * ScaleX);
    }

    /// <summary>
    /// Transforms a data Y coordinate to screen space.
    /// </summary>
    /// <param name="dataY">The data Y coordinate.</param>
    /// <returns>The screen Y coordinate.</returns>
    public float DataToScreenY(double dataY)
    {
        var transformedY = ApplyTransformY(dataY);
        if (!double.IsFinite(transformedY))
        {
            return float.NaN;
        }

        return _screenRect.Bottom - (float)((transformedY - _yTransformMin) * ScaleY);
    }

    /// <summary>
    /// Transforms a screen X coordinate to data space.
    /// </summary>
    /// <param name="screenX">The screen X coordinate.</param>
    /// <returns>The data X coordinate.</returns>
    public double ScreenToDataX(float screenX)
    {
        var normalizedX = ScaleX == 0 ? 0 : (screenX - _screenRect.Left) / ScaleX;
        var transformedX = _xTransformMin + normalizedX;
        return ApplyInverseTransformX(transformedX);
    }

    /// <summary>
    /// Transforms a screen Y coordinate to data space.
    /// </summary>
    /// <param name="screenY">The screen Y coordinate.</param>
    /// <returns>The data Y coordinate.</returns>
    public double ScreenToDataY(float screenY)
    {
        var normalizedY = ScaleY == 0 ? 0 : (_screenRect.Bottom - screenY) / ScaleY;
        var transformedY = _yTransformMin + normalizedY;
        return ApplyInverseTransformY(transformedY);
    }

    /// <summary>
    /// Zooms the viewport by the specified factor around a center point.
    /// </summary>
    /// <param name="factor">The zoom factor (> 1 zooms in, &lt; 1 zooms out).</param>
    /// <param name="centerX">The center X coordinate in data space.</param>
    /// <param name="centerY">The center Y coordinate in data space.</param>
    public void Zoom(double factor, double centerX, double centerY)
    {
        var newXSpan = _xDataRange.Span / factor;
        var newYSpan = _yDataRange.Span / factor;

        var xRatio = (centerX - _xDataRange.Min) / _xDataRange.Span;
        var yRatio = (centerY - _yDataRange.Min) / _yDataRange.Span;

        var newXMin = centerX - newXSpan * xRatio;
        var newXMax = centerX + newXSpan * (1 - xRatio);
        var newYMin = centerY - newYSpan * yRatio;
        var newYMax = centerY + newYSpan * (1 - yRatio);

        _xDataRange = new DataRange(newXMin, newXMax);
        _yDataRange = new DataRange(newYMin, newYMax);

        UpdateTransform();
    }

    /// <summary>
    /// Pans the viewport by the specified delta in data units.
    /// </summary>
    /// <param name="deltaX">The X delta in data units.</param>
    /// <param name="deltaY">The Y delta in data units.</param>
    public void Pan(double deltaX, double deltaY)
    {
        _xDataRange = new DataRange(_xDataRange.Min + deltaX, _xDataRange.Max + deltaX);
        _yDataRange = new DataRange(_yDataRange.Min + deltaY, _yDataRange.Max + deltaY);

        UpdateTransform();
    }

    /// <summary>
    /// Fits the viewport to show the specified data ranges.
    /// </summary>
    /// <param name="xRange">The X data range to fit.</param>
    /// <param name="yRange">The Y data range to fit.</param>
    /// <param name="padding">Optional padding as a fraction of the range (e.g., 0.1 for 10%).</param>
    public void FitToRange(DataRange xRange, DataRange yRange, double padding = 0.05)
    {
        _xDataRange = xRange.WithPadding(padding);
        _yDataRange = yRange.WithPadding(padding);

        UpdateTransform();
    }

    private void UpdateTransform()
    {
        var xMin = ApplyTransformX(_xDataRange.Min);
        var xMax = ApplyTransformX(_xDataRange.Max);

        if (!double.IsFinite(xMin) || !double.IsFinite(xMax))
        {
            _xTransformMin = 0;
            ScaleX = 1;
        }
        else
        {
            if (xMax < xMin)
            {
                (xMin, xMax) = (xMax, xMin);
            }

            _xTransformMin = xMin;
            var xSpan = xMax - xMin;
            ScaleX = xSpan != 0 ? (float)(_screenRect.Width / xSpan) : 1;
        }

        var yMin = ApplyTransformY(_yDataRange.Min);
        var yMax = ApplyTransformY(_yDataRange.Max);

        if (!double.IsFinite(yMin) || !double.IsFinite(yMax))
        {
            _yTransformMin = 0;
            ScaleY = 1;
        }
        else
        {
            if (yMax < yMin)
            {
                (yMin, yMax) = (yMax, yMin);
            }

            _yTransformMin = yMin;
            var ySpan = yMax - yMin;
            ScaleY = ySpan != 0 ? (float)(_screenRect.Height / ySpan) : 1;
        }

        OffsetX = _screenRect.Left;
        OffsetY = _screenRect.Top;
    }

    private double ApplyTransformX(double value)
    {
        return _xTransform == null ? value : _xTransform(value);
    }

    private double ApplyTransformY(double value)
    {
        return _yTransform == null ? value : _yTransform(value);
    }

    private double ApplyInverseTransformX(double value)
    {
        return _xInverseTransform == null ? value : _xInverseTransform(value);
    }

    private double ApplyInverseTransformY(double value)
    {
        return _yInverseTransform == null ? value : _yInverseTransform(value);
    }
}
