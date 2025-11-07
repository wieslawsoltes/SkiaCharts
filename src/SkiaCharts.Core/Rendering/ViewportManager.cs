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

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewportManager"/> class.
    /// </summary>
    public ViewportManager()
    {
        _xDataRange = new DataRange(0, 1);
        _yDataRange = new DataRange(0, 1);
        _screenRect = new SKRect(0, 0, 100, 100);
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
    /// Transforms a data point to screen coordinates.
    /// </summary>
    /// <param name="dataX">The data X coordinate.</param>
    /// <param name="dataY">The data Y coordinate.</param>
    /// <returns>The screen coordinates.</returns>
    public SKPoint DataToScreen(double dataX, double dataY)
    {
        var screenX = _screenRect.Left + (float)((dataX - _xDataRange.Min) * ScaleX);
        var screenY = _screenRect.Bottom - (float)((dataY - _yDataRange.Min) * ScaleY);
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
        var dataX = _xDataRange.Min + (screenX - _screenRect.Left) / ScaleX;
        var dataY = _yDataRange.Min + (_screenRect.Bottom - screenY) / ScaleY;
        return (dataX, dataY);
    }

    /// <summary>
    /// Transforms a data X coordinate to screen space.
    /// </summary>
    /// <param name="dataX">The data X coordinate.</param>
    /// <returns>The screen X coordinate.</returns>
    public float DataToScreenX(double dataX)
    {
        return _screenRect.Left + (float)((dataX - _xDataRange.Min) * ScaleX);
    }

    /// <summary>
    /// Transforms a data Y coordinate to screen space.
    /// </summary>
    /// <param name="dataY">The data Y coordinate.</param>
    /// <returns>The screen Y coordinate.</returns>
    public float DataToScreenY(double dataY)
    {
        return _screenRect.Bottom - (float)((dataY - _yDataRange.Min) * ScaleY);
    }

    /// <summary>
    /// Transforms a screen X coordinate to data space.
    /// </summary>
    /// <param name="screenX">The screen X coordinate.</param>
    /// <returns>The data X coordinate.</returns>
    public double ScreenToDataX(float screenX)
    {
        return _xDataRange.Min + (screenX - _screenRect.Left) / ScaleX;
    }

    /// <summary>
    /// Transforms a screen Y coordinate to data space.
    /// </summary>
    /// <param name="screenY">The screen Y coordinate.</param>
    /// <returns>The data Y coordinate.</returns>
    public double ScreenToDataY(float screenY)
    {
        return _yDataRange.Min + (_screenRect.Bottom - screenY) / ScaleY;
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
        if (_xDataRange.Span > 0)
        {
            ScaleX = (float)(_screenRect.Width / _xDataRange.Span);
        }
        else
        {
            ScaleX = 1;
        }

        if (_yDataRange.Span > 0)
        {
            ScaleY = (float)(_screenRect.Height / _yDataRange.Span);
        }
        else
        {
            ScaleY = 1;
        }

        OffsetX = _screenRect.Left;
        OffsetY = _screenRect.Top;
    }
}
