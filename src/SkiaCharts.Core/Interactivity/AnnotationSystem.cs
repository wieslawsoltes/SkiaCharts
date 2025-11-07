using SkiaSharp;

namespace SkiaCharts.Core.Interactivity;

/// <summary>
/// Manages chart annotations.
/// </summary>
public class AnnotationManager
{
    private readonly List<IAnnotation> _annotations;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnnotationManager"/> class.
    /// </summary>
    public AnnotationManager()
    {
        _annotations = new List<IAnnotation>();
        IsEnabled = true;
    }

    /// <summary>
    /// Gets or sets whether annotations are enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets the annotations.
    /// </summary>
    public IReadOnlyList<IAnnotation> Annotations => _annotations;

    /// <summary>
    /// Event raised when an annotation is added.
    /// </summary>
    public event EventHandler<AnnotationEventArgs>? AnnotationAdded;

    /// <summary>
    /// Event raised when an annotation is removed.
    /// </summary>
    public event EventHandler<AnnotationEventArgs>? AnnotationRemoved;

    /// <summary>
    /// Event raised when an annotation is clicked.
    /// </summary>
    public event EventHandler<AnnotationEventArgs>? AnnotationClicked;

    /// <summary>
    /// Adds an annotation.
    /// </summary>
    /// <param name="annotation">The annotation to add.</param>
    public void AddAnnotation(IAnnotation annotation)
    {
        if (annotation != null)
        {
            _annotations.Add(annotation);
            OnAnnotationAdded(annotation);
        }
    }

    /// <summary>
    /// Removes an annotation.
    /// </summary>
    /// <param name="annotation">The annotation to remove.</param>
    public bool RemoveAnnotation(IAnnotation annotation)
    {
        if (_annotations.Remove(annotation))
        {
            OnAnnotationRemoved(annotation);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Clears all annotations.
    /// </summary>
    public void Clear()
    {
        _annotations.Clear();
    }

    /// <summary>
    /// Performs hit testing to find annotation at position.
    /// </summary>
    /// <param name="position">The position to test.</param>
    /// <param name="viewport">The viewport for coordinate transformation.</param>
    /// <returns>The annotation at the position, or null if none.</returns>
    public IAnnotation? HitTest(SKPoint position, Viewport viewport)
    {
        if (!IsEnabled)
            return null;

        // Test in reverse order (top to bottom)
        for (int i = _annotations.Count - 1; i >= 0; i--)
        {
            var annotation = _annotations[i];
            if (annotation.IsVisible && annotation.HitTest(position, viewport))
                return annotation;
        }

        return null;
    }

    /// <summary>
    /// Handles a click at the specified position.
    /// </summary>
    /// <param name="position">The click position.</param>
    /// <param name="viewport">The viewport for coordinate transformation.</param>
    /// <returns>True if an annotation was clicked.</returns>
    public bool HandleClick(SKPoint position, Viewport viewport)
    {
        if (!IsEnabled)
            return false;

        var annotation = HitTest(position, viewport);
        if (annotation != null)
        {
            OnAnnotationClicked(annotation);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Renders all annotations.
    /// </summary>
    /// <param name="canvas">The canvas to render on.</param>
    /// <param name="chartBounds">The chart bounds.</param>
    /// <param name="viewport">The viewport for coordinate transformation.</param>
    public void Render(SKCanvas canvas, SKRect chartBounds, Viewport viewport)
    {
        if (!IsEnabled)
            return;

        foreach (var annotation in _annotations)
        {
            if (annotation.IsVisible)
            {
                annotation.Render(canvas, chartBounds, viewport);
            }
        }
    }

    private void OnAnnotationAdded(IAnnotation annotation)
    {
        AnnotationAdded?.Invoke(this, new AnnotationEventArgs { Annotation = annotation });
    }

    private void OnAnnotationRemoved(IAnnotation annotation)
    {
        AnnotationRemoved?.Invoke(this, new AnnotationEventArgs { Annotation = annotation });
    }

    private void OnAnnotationClicked(IAnnotation annotation)
    {
        AnnotationClicked?.Invoke(this, new AnnotationEventArgs { Annotation = annotation });
    }
}

/// <summary>
/// Base interface for all annotations.
/// </summary>
public interface IAnnotation
{
    /// <summary>
    /// Gets or sets whether the annotation is visible.
    /// </summary>
    bool IsVisible { get; set; }

    /// <summary>
    /// Gets or sets the annotation name.
    /// </summary>
    string? Name { get; set; }

    /// <summary>
    /// Gets or sets associated data.
    /// </summary>
    object? Data { get; set; }

    /// <summary>
    /// Renders the annotation.
    /// </summary>
    /// <param name="canvas">The canvas to render on.</param>
    /// <param name="chartBounds">The chart bounds.</param>
    /// <param name="viewport">The viewport for coordinate transformation.</param>
    void Render(SKCanvas canvas, SKRect chartBounds, Viewport viewport);

    /// <summary>
    /// Performs hit testing.
    /// </summary>
    /// <param name="position">The position to test.</param>
    /// <param name="viewport">The viewport for coordinate transformation.</param>
    /// <returns>True if the position hits the annotation.</returns>
    bool HitTest(SKPoint position, Viewport viewport);
}

/// <summary>
/// Base class for annotations.
/// </summary>
public abstract class AnnotationBase : IAnnotation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnnotationBase"/> class.
    /// </summary>
    protected AnnotationBase()
    {
        IsVisible = true;
    }

    /// <inheritdoc/>
    public bool IsVisible { get; set; }

    /// <inheritdoc/>
    public string? Name { get; set; }

    /// <inheritdoc/>
    public object? Data { get; set; }

    /// <inheritdoc/>
    public abstract void Render(SKCanvas canvas, SKRect chartBounds, Viewport viewport);

    /// <inheritdoc/>
    public abstract bool HitTest(SKPoint position, Viewport viewport);
}

/// <summary>
/// Point annotation - marks a specific data point.
/// </summary>
public class PointAnnotation : AnnotationBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PointAnnotation"/> class.
    /// </summary>
    public PointAnnotation()
    {
        MarkerSize = 8.0f;
        MarkerColor = SKColors.Red;
        MarkerType = PointMarkerType.Circle;
        ShowLabel = true;
        LabelText = string.Empty;
        LabelFontSize = 10.0f;
        LabelColor = SKColors.Black;
        LabelBackground = new SKColor(255, 255, 255, 200);
        LabelPadding = 4.0f;
        LabelOffset = new SKPoint(10, -10);
    }

    /// <summary>
    /// Gets or sets the X coordinate (data space).
    /// </summary>
    public required double X { get; init; }

    /// <summary>
    /// Gets or sets the Y coordinate (data space).
    /// </summary>
    public required double Y { get; init; }

    /// <summary>
    /// Gets or sets the marker size.
    /// </summary>
    public float MarkerSize { get; set; }

    /// <summary>
    /// Gets or sets the marker color.
    /// </summary>
    public SKColor MarkerColor { get; set; }

    /// <summary>
    /// Gets or sets the marker type.
    /// </summary>
    public PointMarkerType MarkerType { get; set; }

    /// <summary>
    /// Gets or sets whether to show a label.
    /// </summary>
    public bool ShowLabel { get; set; }

    /// <summary>
    /// Gets or sets the label text.
    /// </summary>
    public string LabelText { get; set; }

    /// <summary>
    /// Gets or sets the label font size.
    /// </summary>
    public float LabelFontSize { get; set; }

    /// <summary>
    /// Gets or sets the label color.
    /// </summary>
    public SKColor LabelColor { get; set; }

    /// <summary>
    /// Gets or sets the label background color.
    /// </summary>
    public SKColor LabelBackground { get; set; }

    /// <summary>
    /// Gets or sets the label padding.
    /// </summary>
    public float LabelPadding { get; set; }

    /// <summary>
    /// Gets or sets the label offset from marker.
    /// </summary>
    public SKPoint LabelOffset { get; set; }

    /// <inheritdoc/>
    public override void Render(SKCanvas canvas, SKRect chartBounds, Viewport viewport)
    {
        var screenPos = viewport.DataToScreen(new SKPoint((float)X, (float)Y));

        // Check if point is within chart bounds
        if (!chartBounds.Contains(screenPos))
            return;

        // Draw marker
        using var markerPaint = new SKPaint
        {
            Color = MarkerColor,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        switch (MarkerType)
        {
            case PointMarkerType.Circle:
                canvas.DrawCircle(screenPos.X, screenPos.Y, MarkerSize / 2, markerPaint);
                break;

            case PointMarkerType.Square:
                var squareRect = new SKRect(
                    screenPos.X - MarkerSize / 2,
                    screenPos.Y - MarkerSize / 2,
                    screenPos.X + MarkerSize / 2,
                    screenPos.Y + MarkerSize / 2);
                canvas.DrawRect(squareRect, markerPaint);
                break;

            case PointMarkerType.Triangle:
                using (var path = new SKPath())
                {
                    path.MoveTo(screenPos.X, screenPos.Y - MarkerSize / 2);
                    path.LineTo(screenPos.X + MarkerSize / 2, screenPos.Y + MarkerSize / 2);
                    path.LineTo(screenPos.X - MarkerSize / 2, screenPos.Y + MarkerSize / 2);
                    path.Close();
                    canvas.DrawPath(path, markerPaint);
                }
                break;

            case PointMarkerType.Diamond:
                using (var path = new SKPath())
                {
                    path.MoveTo(screenPos.X, screenPos.Y - MarkerSize / 2);
                    path.LineTo(screenPos.X + MarkerSize / 2, screenPos.Y);
                    path.LineTo(screenPos.X, screenPos.Y + MarkerSize / 2);
                    path.LineTo(screenPos.X - MarkerSize / 2, screenPos.Y);
                    path.Close();
                    canvas.DrawPath(path, markerPaint);
                }
                break;

            case PointMarkerType.Cross:
                markerPaint.Style = SKPaintStyle.Stroke;
                markerPaint.StrokeWidth = 2;
                canvas.DrawLine(screenPos.X - MarkerSize / 2, screenPos.Y, screenPos.X + MarkerSize / 2, screenPos.Y, markerPaint);
                canvas.DrawLine(screenPos.X, screenPos.Y - MarkerSize / 2, screenPos.X, screenPos.Y + MarkerSize / 2, markerPaint);
                break;
        }

        // Draw label
        if (ShowLabel && !string.IsNullOrEmpty(LabelText))
        {
            using var textPaint = new SKPaint
            {
                Color = LabelColor,
                TextSize = LabelFontSize,
                IsAntialias = true
            };

            var textBounds = new SKRect();
            textPaint.MeasureText(LabelText, ref textBounds);

            var labelX = screenPos.X + LabelOffset.X;
            var labelY = screenPos.Y + LabelOffset.Y;

            var bgRect = new SKRect(
                labelX - LabelPadding,
                labelY - textBounds.Height - LabelPadding,
                labelX + textBounds.Width + LabelPadding,
                labelY + LabelPadding);

            using var bgPaint = new SKPaint
            {
                Color = LabelBackground,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            canvas.DrawRoundRect(bgRect, 3, 3, bgPaint);
            canvas.DrawText(LabelText, labelX, labelY, textPaint);
        }
    }

    /// <inheritdoc/>
    public override bool HitTest(SKPoint position, Viewport viewport)
    {
        var screenPos = viewport.DataToScreen(new SKPoint((float)X, (float)Y));
        var distance = (float)Math.Sqrt(
            Math.Pow(position.X - screenPos.X, 2) +
            Math.Pow(position.Y - screenPos.Y, 2));
        return distance <= MarkerSize / 2 + 5; // 5px tolerance
    }
}

/// <summary>
/// Point marker type enumeration.
/// </summary>
public enum PointMarkerType
{
    /// <summary>Circle marker.</summary>
    Circle,
    /// <summary>Square marker.</summary>
    Square,
    /// <summary>Triangle marker.</summary>
    Triangle,
    /// <summary>Diamond marker.</summary>
    Diamond,
    /// <summary>Cross marker.</summary>
    Cross
}

/// <summary>
/// Range annotation - highlights a vertical band (time range).
/// </summary>
public class RangeAnnotation : AnnotationBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RangeAnnotation"/> class.
    /// </summary>
    public RangeAnnotation()
    {
        FillColor = new SKColor(0, 128, 255, 40);
        BorderColor = new SKColor(0, 128, 255, 150);
        BorderWidth = 1.0f;
        ShowLabel = true;
        LabelText = string.Empty;
        LabelFontSize = 11.0f;
        LabelColor = SKColors.Black;
        LabelPosition = RangeLabelPosition.Top;
    }

    /// <summary>
    /// Gets or sets the start X coordinate (data space).
    /// </summary>
    public required double StartX { get; init; }

    /// <summary>
    /// Gets or sets the end X coordinate (data space).
    /// </summary>
    public required double EndX { get; init; }

    /// <summary>
    /// Gets or sets the fill color.
    /// </summary>
    public SKColor FillColor { get; set; }

    /// <summary>
    /// Gets or sets the border color.
    /// </summary>
    public SKColor BorderColor { get; set; }

    /// <summary>
    /// Gets or sets the border width.
    /// </summary>
    public float BorderWidth { get; set; }

    /// <summary>
    /// Gets or sets whether to show a label.
    /// </summary>
    public bool ShowLabel { get; set; }

    /// <summary>
    /// Gets or sets the label text.
    /// </summary>
    public string LabelText { get; set; }

    /// <summary>
    /// Gets or sets the label font size.
    /// </summary>
    public float LabelFontSize { get; set; }

    /// <summary>
    /// Gets or sets the label color.
    /// </summary>
    public SKColor LabelColor { get; set; }

    /// <summary>
    /// Gets or sets the label position.
    /// </summary>
    public RangeLabelPosition LabelPosition { get; set; }

    /// <inheritdoc/>
    public override void Render(SKCanvas canvas, SKRect chartBounds, Viewport viewport)
    {
        var startScreen = viewport.DataToScreen(new SKPoint((float)StartX, 0));
        var endScreen = viewport.DataToScreen(new SKPoint((float)EndX, 0));

        var rangeRect = new SKRect(
            Math.Min(startScreen.X, endScreen.X),
            chartBounds.Top,
            Math.Max(startScreen.X, endScreen.X),
            chartBounds.Bottom);

        // Clip to chart bounds
        rangeRect.Intersect(chartBounds);

        if (rangeRect.Width <= 0 || rangeRect.Height <= 0)
            return;

        // Draw fill
        using var fillPaint = new SKPaint
        {
            Color = FillColor,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };
        canvas.DrawRect(rangeRect, fillPaint);

        // Draw border
        if (BorderWidth > 0)
        {
            using var borderPaint = new SKPaint
            {
                Color = BorderColor,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = BorderWidth,
                IsAntialias = true
            };
            canvas.DrawRect(rangeRect, borderPaint);
        }

        // Draw label
        if (ShowLabel && !string.IsNullOrEmpty(LabelText))
        {
            using var textPaint = new SKPaint
            {
                Color = LabelColor,
                TextSize = LabelFontSize,
                IsAntialias = true
            };

            var textBounds = new SKRect();
            textPaint.MeasureText(LabelText, ref textBounds);

            float labelX = rangeRect.MidX - textBounds.Width / 2;
            float labelY = LabelPosition switch
            {
                RangeLabelPosition.Top => rangeRect.Top + LabelFontSize + 4,
                RangeLabelPosition.Bottom => rangeRect.Bottom - 4,
                _ => rangeRect.MidY + textBounds.Height / 2
            };

            canvas.DrawText(LabelText, labelX, labelY, textPaint);
        }
    }

    /// <inheritdoc/>
    public override bool HitTest(SKPoint position, Viewport viewport)
    {
        var startScreen = viewport.DataToScreen(new SKPoint((float)StartX, 0));
        var endScreen = viewport.DataToScreen(new SKPoint((float)EndX, 0));

        return position.X >= Math.Min(startScreen.X, endScreen.X) &&
               position.X <= Math.Max(startScreen.X, endScreen.X);
    }
}

/// <summary>
/// Range label position enumeration.
/// </summary>
public enum RangeLabelPosition
{
    /// <summary>Top of range.</summary>
    Top,
    /// <summary>Middle of range.</summary>
    Middle,
    /// <summary>Bottom of range.</summary>
    Bottom
}

/// <summary>
/// Horizontal threshold line annotation.
/// </summary>
public class ThresholdAnnotation : AnnotationBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ThresholdAnnotation"/> class.
    /// </summary>
    public ThresholdAnnotation()
    {
        LineColor = SKColors.Red;
        LineWidth = 2.0f;
        LineStyle = ThresholdLineStyle.Dashed;
        ShowLabel = true;
        LabelText = string.Empty;
        LabelFontSize = 10.0f;
        LabelColor = SKColors.Black;
        LabelBackground = new SKColor(255, 255, 255, 200);
        LabelPadding = 4.0f;
        LabelPosition = ThresholdLabelPosition.Right;
    }

    /// <summary>
    /// Gets or sets the Y coordinate (data space).
    /// </summary>
    public required double Y { get; init; }

    /// <summary>
    /// Gets or sets the line color.
    /// </summary>
    public SKColor LineColor { get; set; }

    /// <summary>
    /// Gets or sets the line width.
    /// </summary>
    public float LineWidth { get; set; }

    /// <summary>
    /// Gets or sets the line style.
    /// </summary>
    public ThresholdLineStyle LineStyle { get; set; }

    /// <summary>
    /// Gets or sets whether to show a label.
    /// </summary>
    public bool ShowLabel { get; set; }

    /// <summary>
    /// Gets or sets the label text.
    /// </summary>
    public string LabelText { get; set; }

    /// <summary>
    /// Gets or sets the label font size.
    /// </summary>
    public float LabelFontSize { get; set; }

    /// <summary>
    /// Gets or sets the label color.
    /// </summary>
    public SKColor LabelColor { get; set; }

    /// <summary>
    /// Gets or sets the label background color.
    /// </summary>
    public SKColor LabelBackground { get; set; }

    /// <summary>
    /// Gets or sets the label padding.
    /// </summary>
    public float LabelPadding { get; set; }

    /// <summary>
    /// Gets or sets the label position.
    /// </summary>
    public ThresholdLabelPosition LabelPosition { get; set; }

    /// <inheritdoc/>
    public override void Render(SKCanvas canvas, SKRect chartBounds, Viewport viewport)
    {
        var screenPos = viewport.DataToScreen(new SKPoint(0, (float)Y));

        // Check if line is within chart bounds
        if (screenPos.Y < chartBounds.Top || screenPos.Y > chartBounds.Bottom)
            return;

        using var linePaint = new SKPaint
        {
            Color = LineColor,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = LineWidth,
            IsAntialias = true
        };

        // Set line style
        if (LineStyle == ThresholdLineStyle.Dashed)
        {
            linePaint.PathEffect = SKPathEffect.CreateDash(new[] { 10f, 5f }, 0);
        }
        else if (LineStyle == ThresholdLineStyle.Dotted)
        {
            linePaint.PathEffect = SKPathEffect.CreateDash(new[] { 2f, 4f }, 0);
        }

        // Draw line
        canvas.DrawLine(chartBounds.Left, screenPos.Y, chartBounds.Right, screenPos.Y, linePaint);

        // Draw label
        if (ShowLabel && !string.IsNullOrEmpty(LabelText))
        {
            using var textPaint = new SKPaint
            {
                Color = LabelColor,
                TextSize = LabelFontSize,
                IsAntialias = true
            };

            var textBounds = new SKRect();
            textPaint.MeasureText(LabelText, ref textBounds);

            float labelX = LabelPosition switch
            {
                ThresholdLabelPosition.Left => chartBounds.Left + 4,
                ThresholdLabelPosition.Right => chartBounds.Right - textBounds.Width - LabelPadding * 2 - 4,
                _ => chartBounds.MidX - textBounds.Width / 2 - LabelPadding
            };

            float labelY = screenPos.Y - textBounds.Height / 2;

            var bgRect = new SKRect(
                labelX - LabelPadding,
                labelY - textBounds.Height - LabelPadding,
                labelX + textBounds.Width + LabelPadding,
                labelY + LabelPadding);

            using var bgPaint = new SKPaint
            {
                Color = LabelBackground,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            canvas.DrawRoundRect(bgRect, 3, 3, bgPaint);
            canvas.DrawText(LabelText, labelX, labelY, textPaint);
        }
    }

    /// <inheritdoc/>
    public override bool HitTest(SKPoint position, Viewport viewport)
    {
        var screenPos = viewport.DataToScreen(new SKPoint(0, (float)Y));
        return Math.Abs(position.Y - screenPos.Y) <= LineWidth / 2 + 3; // 3px tolerance
    }
}

/// <summary>
/// Threshold line style enumeration.
/// </summary>
public enum ThresholdLineStyle
{
    /// <summary>Solid line.</summary>
    Solid,
    /// <summary>Dashed line.</summary>
    Dashed,
    /// <summary>Dotted line.</summary>
    Dotted
}

/// <summary>
/// Threshold label position enumeration.
/// </summary>
public enum ThresholdLabelPosition
{
    /// <summary>Left side.</summary>
    Left,
    /// <summary>Center.</summary>
    Center,
    /// <summary>Right side.</summary>
    Right
}

/// <summary>
/// Custom annotation with user-defined rendering.
/// </summary>
public class CustomAnnotation : AnnotationBase
{
    /// <summary>
    /// Gets or sets the custom render function.
    /// </summary>
    public Action<SKCanvas, SKRect, Viewport>? RenderFunction { get; set; }

    /// <summary>
    /// Gets or sets the custom hit test function.
    /// </summary>
    public Func<SKPoint, Viewport, bool>? HitTestFunction { get; set; }

    /// <inheritdoc/>
    public override void Render(SKCanvas canvas, SKRect chartBounds, Viewport viewport)
    {
        RenderFunction?.Invoke(canvas, chartBounds, viewport);
    }

    /// <inheritdoc/>
    public override bool HitTest(SKPoint position, Viewport viewport)
    {
        return HitTestFunction?.Invoke(position, viewport) ?? false;
    }
}

/// <summary>
/// Event arguments for annotation events.
/// </summary>
public class AnnotationEventArgs : EventArgs
{
    /// <summary>
    /// Gets the annotation.
    /// </summary>
    public required IAnnotation Annotation { get; init; }
}
