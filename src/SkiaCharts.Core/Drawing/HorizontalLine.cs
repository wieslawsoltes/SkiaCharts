using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Drawing;

/// <summary>
/// Horizontal line drawing tool.
/// Draws a horizontal line at a specified price level.
/// </summary>
public class HorizontalLine : DrawingToolBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HorizontalLine"/> class.
    /// </summary>
    public HorizontalLine()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HorizontalLine"/> class.
    /// </summary>
    /// <param name="y">Y coordinate (price level in data space).</param>
    public HorizontalLine(double y)
    {
        Y = y;
    }

    /// <summary>
    /// Gets or sets the Y coordinate (price level in data space).
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Gets or sets the label text.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets whether to show the label.
    /// </summary>
    public bool ShowLabel { get; set; } = true;

    /// <summary>
    /// Gets or sets the label font size.
    /// </summary>
    public float LabelFontSize { get; set; } = 12f;

    /// <summary>
    /// Sets the viewport for coordinate transformation.
    /// </summary>
    public ViewportManager? Viewport { get; set; }

    /// <inheritdoc/>
    public override void Render(IRenderContext context)
    {
        if (!IsVisible || Viewport == null)
            return;

        var screenY = Viewport.DataToScreen(0, Y).Y;

        using var paint = new SKPaint
        {
            Color = IsSelected ? SKColors.Yellow : Color,
            StrokeWidth = LineWidth,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true
        };

        if (DashPattern != null && DashPattern.Length > 0)
        {
            paint.PathEffect = SKPathEffect.CreateDash(DashPattern, 0);
        }

        context.DrawLine(Viewport.ScreenRect.Left, screenY, Viewport.ScreenRect.Right, screenY, paint);

        // Draw label
        if (ShowLabel && !string.IsNullOrEmpty(Label))
        {
            using var textPaint = new SKPaint
            {
                Color = Color,
                TextSize = LabelFontSize,
                IsAntialias = true
            };

            context.DrawText(Label, Viewport.ScreenRect.Left + 5, screenY - 5, textPaint);
        }
    }

    /// <inheritdoc/>
    public override bool HitTest(float x, float y, float tolerance = 5f)
    {
        if (Viewport == null)
            return false;

        var screenY = Viewport.DataToScreen(0, Y).Y;
        return Math.Abs(y - screenY) <= tolerance;
    }

    /// <inheritdoc/>
    public override Dictionary<string, object> Serialize()
    {
        var data = base.Serialize();
        data["Y"] = Y;
        data["Label"] = Label ?? string.Empty;
        data["ShowLabel"] = ShowLabel;
        data["LabelFontSize"] = LabelFontSize;
        return data;
    }

    /// <inheritdoc/>
    public override void Deserialize(Dictionary<string, object> data)
    {
        base.Deserialize(data);

        if (data.TryGetValue("Y", out var y))
            Y = Convert.ToDouble(y);

        if (data.TryGetValue("Label", out var label))
            Label = label.ToString();

        if (data.TryGetValue("ShowLabel", out var showLabel))
            ShowLabel = Convert.ToBoolean(showLabel);

        if (data.TryGetValue("LabelFontSize", out var labelFontSize))
            LabelFontSize = Convert.ToSingle(labelFontSize);
    }
}

/// <summary>
/// Vertical line drawing tool.
/// Draws a vertical line at a specified time/X position.
/// </summary>
public class VerticalLine : DrawingToolBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VerticalLine"/> class.
    /// </summary>
    public VerticalLine()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VerticalLine"/> class.
    /// </summary>
    /// <param name="x">X coordinate (time in data space).</param>
    public VerticalLine(double x)
    {
        X = x;
    }

    /// <summary>
    /// Gets or sets the X coordinate (time in data space).
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Gets or sets the label text.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets whether to show the label.
    /// </summary>
    public bool ShowLabel { get; set; } = true;

    /// <summary>
    /// Gets or sets the label font size.
    /// </summary>
    public float LabelFontSize { get; set; } = 12f;

    /// <summary>
    /// Sets the viewport for coordinate transformation.
    /// </summary>
    public ViewportManager? Viewport { get; set; }

    /// <inheritdoc/>
    public override void Render(IRenderContext context)
    {
        if (!IsVisible || Viewport == null)
            return;

        var screenX = Viewport.DataToScreen(X, 0).X;

        using var paint = new SKPaint
        {
            Color = IsSelected ? SKColors.Yellow : Color,
            StrokeWidth = LineWidth,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true
        };

        if (DashPattern != null && DashPattern.Length > 0)
        {
            paint.PathEffect = SKPathEffect.CreateDash(DashPattern, 0);
        }

        context.DrawLine(screenX, Viewport.ScreenRect.Top, screenX, Viewport.ScreenRect.Bottom, paint);

        // Draw label
        if (ShowLabel && !string.IsNullOrEmpty(Label))
        {
            using var textPaint = new SKPaint
            {
                Color = Color,
                TextSize = LabelFontSize,
                IsAntialias = true
            };

            context.DrawText(Label, screenX + 5, Viewport.ScreenRect.Top + 15, textPaint);
        }
    }

    /// <inheritdoc/>
    public override bool HitTest(float x, float y, float tolerance = 5f)
    {
        if (Viewport == null)
            return false;

        var screenX = Viewport.DataToScreen(X, 0).X;
        return Math.Abs(x - screenX) <= tolerance;
    }

    /// <inheritdoc/>
    public override Dictionary<string, object> Serialize()
    {
        var data = base.Serialize();
        data["X"] = X;
        data["Label"] = Label ?? string.Empty;
        data["ShowLabel"] = ShowLabel;
        data["LabelFontSize"] = LabelFontSize;
        return data;
    }

    /// <inheritdoc/>
    public override void Deserialize(Dictionary<string, object> data)
    {
        base.Deserialize(data);

        if (data.TryGetValue("X", out var x))
            X = Convert.ToDouble(x);

        if (data.TryGetValue("Label", out var label))
            Label = label.ToString();

        if (data.TryGetValue("ShowLabel", out var showLabel))
            ShowLabel = Convert.ToBoolean(showLabel);

        if (data.TryGetValue("LabelFontSize", out var labelFontSize))
            LabelFontSize = Convert.ToSingle(labelFontSize);
    }
}
