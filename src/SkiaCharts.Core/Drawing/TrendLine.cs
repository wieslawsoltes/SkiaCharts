using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Drawing;

/// <summary>
/// Trend line drawing tool.
/// Draws a line between two points with optional extension.
/// </summary>
public class TrendLine : DrawingToolBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TrendLine"/> class.
    /// </summary>
    public TrendLine()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TrendLine"/> class.
    /// </summary>
    /// <param name="x1">First point X coordinate (data space).</param>
    /// <param name="y1">First point Y coordinate (data space).</param>
    /// <param name="x2">Second point X coordinate (data space).</param>
    /// <param name="y2">Second point Y coordinate (data space).</param>
    public TrendLine(double x1, double y1, double x2, double y2)
    {
        X1 = x1;
        Y1 = y1;
        X2 = x2;
        Y2 = y2;
    }

    /// <summary>
    /// Gets or sets the first point X coordinate (data space).
    /// </summary>
    public double X1 { get; set; }

    /// <summary>
    /// Gets or sets the first point Y coordinate (data space).
    /// </summary>
    public double Y1 { get; set; }

    /// <summary>
    /// Gets or sets the second point X coordinate (data space).
    /// </summary>
    public double X2 { get; set; }

    /// <summary>
    /// Gets or sets the second point Y coordinate (data space).
    /// </summary>
    public double Y2 { get; set; }

    /// <summary>
    /// Gets or sets whether to extend the line beyond the endpoints.
    /// </summary>
    public bool ExtendLine { get; set; }

    /// <summary>
    /// Gets or sets whether to extend the line to the left.
    /// </summary>
    public bool ExtendLeft { get; set; }

    /// <summary>
    /// Gets or sets whether to extend the line to the right.
    /// </summary>
    public bool ExtendRight { get; set; }

    /// <summary>
    /// Sets the viewport for coordinate transformation.
    /// </summary>
    public ViewportManager? Viewport { get; set; }

    /// <inheritdoc/>
    public override void Render(IRenderContext context)
    {
        if (!IsVisible || Viewport == null)
            return;

        var p1 = Viewport.DataToScreen(X1, Y1);
        var p2 = Viewport.DataToScreen(X2, Y2);

        float x1 = p1.X;
        float y1 = p1.Y;
        float x2 = p2.X;
        float y2 = p2.Y;

        // Extend line if requested
        if (ExtendLine || ExtendLeft || ExtendRight)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;

            if (Math.Abs(dx) > 0.001f)
            {
                float slope = dy / dx;

                if (ExtendLine || ExtendLeft)
                {
                    // Extend to left edge
                    float leftX = Viewport.ScreenRect.Left;
                    float leftY = y1 + slope * (leftX - x1);
                    x1 = leftX;
                    y1 = leftY;
                }

                if (ExtendLine || ExtendRight)
                {
                    // Extend to right edge
                    float rightX = Viewport.ScreenRect.Right;
                    float rightY = y1 + slope * (rightX - x1);
                    x2 = rightX;
                    y2 = rightY;
                }
            }
        }

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

        context.DrawLine(x1, y1, x2, y2, paint);

        // Draw handles if selected
        if (IsSelected)
        {
            DrawHandle(context, p1.X, p1.Y);
            DrawHandle(context, p2.X, p2.Y);
        }
    }

    private void DrawHandle(IRenderContext context, float x, float y)
    {
        using var handlePaint = new SKPaint
        {
            Color = SKColors.Yellow,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        context.DrawCircle(x, y, 4, handlePaint);

        using var borderPaint = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            IsAntialias = true
        };

        context.DrawCircle(x, y, 4, borderPaint);
    }

    /// <inheritdoc/>
    public override bool HitTest(float x, float y, float tolerance = 5f)
    {
        if (Viewport == null)
            return false;

        var p1 = Viewport.DataToScreen(X1, Y1);
        var p2 = Viewport.DataToScreen(X2, Y2);

        return DistanceToLineSegment(x, y, p1.X, p1.Y, p2.X, p2.Y) <= tolerance;
    }

    /// <inheritdoc/>
    public override Dictionary<string, object> Serialize()
    {
        var data = base.Serialize();
        data["X1"] = X1;
        data["Y1"] = Y1;
        data["X2"] = X2;
        data["Y2"] = Y2;
        data["ExtendLine"] = ExtendLine;
        data["ExtendLeft"] = ExtendLeft;
        data["ExtendRight"] = ExtendRight;
        return data;
    }

    /// <inheritdoc/>
    public override void Deserialize(Dictionary<string, object> data)
    {
        base.Deserialize(data);

        if (data.TryGetValue("X1", out var x1))
            X1 = Convert.ToDouble(x1);

        if (data.TryGetValue("Y1", out var y1))
            Y1 = Convert.ToDouble(y1);

        if (data.TryGetValue("X2", out var x2))
            X2 = Convert.ToDouble(x2);

        if (data.TryGetValue("Y2", out var y2))
            Y2 = Convert.ToDouble(y2);

        if (data.TryGetValue("ExtendLine", out var extendLine))
            ExtendLine = Convert.ToBoolean(extendLine);

        if (data.TryGetValue("ExtendLeft", out var extendLeft))
            ExtendLeft = Convert.ToBoolean(extendLeft);

        if (data.TryGetValue("ExtendRight", out var extendRight))
            ExtendRight = Convert.ToBoolean(extendRight);
    }
}
