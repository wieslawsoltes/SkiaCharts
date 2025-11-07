using SkiaSharp;

namespace SkiaCharts.Core.Interactivity;

/// <summary>
/// Manages the viewport transformation for chart navigation (pan and zoom).
/// </summary>
public class Viewport
{
    private SKRect _dataBounds;
    private SKRect _viewBounds;
    private float _zoom = 1.0f;
    private SKPoint _pan = SKPoint.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="Viewport"/> class.
    /// </summary>
    public Viewport()
    {
        _dataBounds = SKRect.Empty;
        _viewBounds = SKRect.Empty;
        MinZoom = 0.1f;
        MaxZoom = 100.0f;
    }

    /// <summary>
    /// Gets or sets the data bounds (the full extent of the data).
    /// </summary>
    public SKRect DataBounds
    {
        get => _dataBounds;
        set
        {
            _dataBounds = value;
            OnBoundsChanged();
        }
    }

    /// <summary>
    /// Gets or sets the view bounds (the screen/canvas area).
    /// </summary>
    public SKRect ViewBounds
    {
        get => _viewBounds;
        set
        {
            _viewBounds = value;
            OnBoundsChanged();
        }
    }

    /// <summary>
    /// Gets or sets the zoom level (1.0 = 100%).
    /// </summary>
    public float Zoom
    {
        get => _zoom;
        set
        {
            var newZoom = Math.Clamp(value, MinZoom, MaxZoom);
            if (Math.Abs(_zoom - newZoom) > 0.0001f)
            {
                _zoom = newZoom;
                OnTransformChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the pan offset in data coordinates.
    /// </summary>
    public SKPoint Pan
    {
        get => _pan;
        set
        {
            _pan = value;
            OnTransformChanged();
        }
    }

    /// <summary>
    /// Gets or sets the minimum zoom level.
    /// </summary>
    public float MinZoom { get; set; }

    /// <summary>
    /// Gets or sets the maximum zoom level.
    /// </summary>
    public float MaxZoom { get; set; }

    /// <summary>
    /// Gets the current visible data rectangle.
    /// </summary>
    public SKRect VisibleDataRect
    {
        get
        {
            if (_viewBounds.IsEmpty || _zoom == 0)
                return _dataBounds;

            var width = _viewBounds.Width / _zoom;
            var height = _viewBounds.Height / _zoom;

            return new SKRect(
                _pan.X,
                _pan.Y,
                _pan.X + width,
                _pan.Y + height
            );
        }
    }

    /// <summary>
    /// Event raised when the transformation changes.
    /// </summary>
    public event EventHandler? TransformChanged;

    /// <summary>
    /// Converts a screen point to data coordinates.
    /// </summary>
    /// <param name="screenPoint">The screen point.</param>
    /// <returns>The data point.</returns>
    public SKPoint ScreenToData(SKPoint screenPoint)
    {
        if (_viewBounds.IsEmpty || _zoom == 0)
            return screenPoint;

        var relativeX = (screenPoint.X - _viewBounds.Left) / _viewBounds.Width;
        var relativeY = (screenPoint.Y - _viewBounds.Top) / _viewBounds.Height;

        var visible = VisibleDataRect;
        return new SKPoint(
            visible.Left + relativeX * visible.Width,
            visible.Top + relativeY * visible.Height
        );
    }

    /// <summary>
    /// Converts a data point to screen coordinates.
    /// </summary>
    /// <param name="dataPoint">The data point.</param>
    /// <returns>The screen point.</returns>
    public SKPoint DataToScreen(SKPoint dataPoint)
    {
        if (_viewBounds.IsEmpty || _zoom == 0)
            return dataPoint;

        var visible = VisibleDataRect;
        if (visible.Width == 0 || visible.Height == 0)
            return dataPoint;

        var relativeX = (dataPoint.X - visible.Left) / visible.Width;
        var relativeY = (dataPoint.Y - visible.Top) / visible.Height;

        return new SKPoint(
            _viewBounds.Left + relativeX * _viewBounds.Width,
            _viewBounds.Top + relativeY * _viewBounds.Height
        );
    }

    /// <summary>
    /// Zooms in by a factor.
    /// </summary>
    /// <param name="factor">The zoom factor (e.g., 1.2 for 20% zoom in).</param>
    /// <param name="centerScreen">The screen center point for the zoom (optional).</param>
    public void ZoomIn(float factor, SKPoint? centerScreen = null)
    {
        ZoomBy(factor, centerScreen);
    }

    /// <summary>
    /// Zooms out by a factor.
    /// </summary>
    /// <param name="factor">The zoom factor (e.g., 1.2 for 20% zoom out).</param>
    /// <param name="centerScreen">The screen center point for the zoom (optional).</param>
    public void ZoomOut(float factor, SKPoint? centerScreen = null)
    {
        ZoomBy(1.0f / factor, centerScreen);
    }

    /// <summary>
    /// Zooms by a factor around a center point.
    /// </summary>
    /// <param name="factor">The zoom factor.</param>
    /// <param name="centerScreen">The screen center point (if null, uses view center).</param>
    public void ZoomBy(float factor, SKPoint? centerScreen = null)
    {
        var center = centerScreen ?? new SKPoint(_viewBounds.MidX, _viewBounds.MidY);
        var dataCenter = ScreenToData(center);

        var newZoom = _zoom * factor;
        newZoom = Math.Clamp(newZoom, MinZoom, MaxZoom);

        if (Math.Abs(newZoom - _zoom) < 0.0001f)
            return;

        _zoom = newZoom;

        // Adjust pan to keep the center point fixed
        var newDataCenter = ScreenToData(center);
        _pan.X += (dataCenter.X - newDataCenter.X);
        _pan.Y += (dataCenter.Y - newDataCenter.Y);

        ConstrainPan();
        OnTransformChanged();
    }

    /// <summary>
    /// Pans by a delta in screen coordinates.
    /// </summary>
    /// <param name="deltaScreen">The delta in screen coordinates.</param>
    public void PanBy(SKPoint deltaScreen)
    {
        if (_zoom == 0)
            return;

        var deltaData = new SKPoint(
            deltaScreen.X / _zoom,
            deltaScreen.Y / _zoom
        );

        _pan.X -= deltaData.X;
        _pan.Y -= deltaData.Y;

        ConstrainPan();
        OnTransformChanged();
    }

    /// <summary>
    /// Zooms to fit the data bounds in the view.
    /// </summary>
    /// <param name="margin">Optional margin as a fraction (e.g., 0.1 for 10% margin).</param>
    public void ZoomToFit(float margin = 0.1f)
    {
        if (_dataBounds.IsEmpty || _viewBounds.IsEmpty)
            return;

        var dataWidth = _dataBounds.Width;
        var dataHeight = _dataBounds.Height;
        var viewWidth = _viewBounds.Width;
        var viewHeight = _viewBounds.Height;

        if (dataWidth == 0 || dataHeight == 0 || viewWidth == 0 || viewHeight == 0)
            return;

        // Calculate zoom to fit both dimensions
        var zoomX = viewWidth / dataWidth;
        var zoomY = viewHeight / dataHeight;
        var zoom = Math.Min(zoomX, zoomY);

        // Apply margin
        zoom *= (1.0f - margin);
        zoom = Math.Clamp(zoom, MinZoom, MaxZoom);

        _zoom = zoom;

        // Center the data in the view
        var visible = VisibleDataRect;
        _pan.X = _dataBounds.Left - (visible.Width - dataWidth) / 2;
        _pan.Y = _dataBounds.Top - (visible.Height - dataHeight) / 2;

        ConstrainPan();
        OnTransformChanged();
    }

    /// <summary>
    /// Zooms to a specific data rectangle.
    /// </summary>
    /// <param name="dataRect">The data rectangle to zoom to.</param>
    /// <param name="margin">Optional margin as a fraction.</param>
    public void ZoomToRect(SKRect dataRect, float margin = 0.05f)
    {
        if (dataRect.IsEmpty || _viewBounds.IsEmpty)
            return;

        var dataWidth = dataRect.Width;
        var dataHeight = dataRect.Height;
        var viewWidth = _viewBounds.Width;
        var viewHeight = _viewBounds.Height;

        if (dataWidth == 0 || dataHeight == 0 || viewWidth == 0 || viewHeight == 0)
            return;

        var zoomX = viewWidth / dataWidth;
        var zoomY = viewHeight / dataHeight;
        var zoom = Math.Min(zoomX, zoomY);

        zoom *= (1.0f - margin);
        zoom = Math.Clamp(zoom, MinZoom, MaxZoom);

        _zoom = zoom;

        // Pan to the rectangle
        var visible = VisibleDataRect;
        _pan.X = dataRect.Left - (visible.Width - dataWidth) / 2;
        _pan.Y = dataRect.Top - (visible.Height - dataHeight) / 2;

        ConstrainPan();
        OnTransformChanged();
    }

    /// <summary>
    /// Resets the zoom and pan to defaults (shows all data).
    /// </summary>
    public void Reset()
    {
        ZoomToFit(0.05f);
    }

    /// <summary>
    /// Constrains the pan to keep data in view.
    /// </summary>
    private void ConstrainPan()
    {
        if (_dataBounds.IsEmpty || _zoom == 0)
            return;

        var visible = VisibleDataRect;

        // Don't allow panning beyond data bounds
        if (visible.Width < _dataBounds.Width)
        {
            if (_pan.X < _dataBounds.Left)
                _pan.X = _dataBounds.Left;
            if (_pan.X + visible.Width > _dataBounds.Right)
                _pan.X = _dataBounds.Right - visible.Width;
        }
        else
        {
            // If zoomed out, center horizontally
            _pan.X = _dataBounds.Left - (visible.Width - _dataBounds.Width) / 2;
        }

        if (visible.Height < _dataBounds.Height)
        {
            if (_pan.Y < _dataBounds.Top)
                _pan.Y = _dataBounds.Top;
            if (_pan.Y + visible.Height > _dataBounds.Bottom)
                _pan.Y = _dataBounds.Bottom - visible.Height;
        }
        else
        {
            // If zoomed out, center vertically
            _pan.Y = _dataBounds.Top - (visible.Height - _dataBounds.Height) / 2;
        }
    }

    private void OnBoundsChanged()
    {
        ConstrainPan();
        OnTransformChanged();
    }

    private void OnTransformChanged()
    {
        TransformChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Gets the transformation matrix for rendering.
    /// </summary>
    public SKMatrix GetTransformMatrix()
    {
        var matrix = SKMatrix.Identity;

        if (_viewBounds.IsEmpty || _zoom == 0)
            return matrix;

        var visible = VisibleDataRect;
        if (visible.Width == 0 || visible.Height == 0)
            return matrix;

        // Translate to view bounds origin
        matrix = matrix.PreConcat(SKMatrix.CreateTranslation(_viewBounds.Left, _viewBounds.Top));

        // Scale to fit view
        var scaleX = _viewBounds.Width / visible.Width;
        var scaleY = _viewBounds.Height / visible.Height;
        matrix = matrix.PreConcat(SKMatrix.CreateScale(scaleX, scaleY));

        // Translate by pan offset
        matrix = matrix.PreConcat(SKMatrix.CreateTranslation(-visible.Left, -visible.Top));

        return matrix;
    }
}
