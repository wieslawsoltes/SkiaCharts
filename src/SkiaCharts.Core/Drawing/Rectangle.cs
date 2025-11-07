using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Drawing;

/// <summary>
/// Rectangle drawing tool.
/// Draws a rectangle between two corner points.
/// </summary>
public class Rectangle : DrawingToolBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Rectangle"/> class.
    /// </summary>
    public Rectangle()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Rectangle"/> class.
    /// </summary>
    /// <param name="x1">First corner X coordinate (data space).</param>
    /// <param name="y1">First corner Y coordinate (data space).</param>
    /// <param name="x2">Second corner X coordinate (data space).</param>
    /// <param name="y2">Second corner Y coordinate (data space).</param>
    public Rectangle(double x1, double y1, double x2, double y2)
    {
        X1 = x1;
        Y1 = y1;
        X2 = x2;
        Y2 = y2;
    }

    /// <summary>
    /// Gets or sets the first corner X coordinate (data space).
    /// </summary>
    public double X1 { get; set; }

    /// <summary>
    /// Gets or sets the first corner Y coordinate (data space).
    /// </summary>
    public double Y1 { get; set; }

    /// <summary>
    /// Gets or sets the second corner X coordinate (data space).
    /// </summary>
    public double X2 { get; set; }

    /// <summary>
    /// Gets or sets the second corner Y coordinate (data space).
    /// </summary>
    public double Y2 { get; set; }

    /// <summary>
    /// Gets or sets whether to fill the rectangle.
    /// </summary>
    public bool Fill { get; set; }

    /// <summary>
    /// Gets or sets the fill color.
    /// </summary>
    public SKColor FillColor { get; set; } = new SKColor(0, 0, 255, 50);

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

        var rect = new SKRect(
            Math.Min(p1.X, p2.X),
            Math.Min(p1.Y, p2.Y),
            Math.Max(p1.X, p2.X),
            Math.Max(p1.Y, p2.Y)
        );

        // Fill
        if (Fill)
        {
            using var fillPaint = new SKPaint
            {
                Color = FillColor,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            context.DrawRect(rect, fillPaint);
        }

        // Border
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

        context.DrawRect(rect, paint);

        // Draw handles if selected
        if (IsSelected)
        {
            DrawHandle(context, rect.Left, rect.Top);
            DrawHandle(context, rect.Right, rect.Top);
            DrawHandle(context, rect.Left, rect.Bottom);
            DrawHandle(context, rect.Right, rect.Bottom);
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

        var rect = new SKRect(
            Math.Min(p1.X, p2.X),
            Math.Min(p1.Y, p2.Y),
            Math.Max(p1.X, p2.X),
            Math.Max(p1.Y, p2.Y)
        );

        // Check if inside rectangle or near border
        bool inside = rect.Contains(x, y);
        bool nearBorder =
            Math.Abs(x - rect.Left) <= tolerance ||
            Math.Abs(x - rect.Right) <= tolerance ||
            Math.Abs(y - rect.Top) <= tolerance ||
            Math.Abs(y - rect.Bottom) <= tolerance;

        return inside || nearBorder;
    }

    /// <inheritdoc/>
    public override Dictionary<string, object> Serialize()
    {
        var data = base.Serialize();
        data["X1"] = X1;
        data["Y1"] = Y1;
        data["X2"] = X2;
        data["Y2"] = Y2;
        data["Fill"] = Fill;
        data["FillColor"] = FillColor.ToString();
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

        if (data.TryGetValue("Fill", out var fill))
            Fill = Convert.ToBoolean(fill);

        if (data.TryGetValue("FillColor", out var fillColor))
            FillColor = SKColor.Parse(fillColor.ToString() ?? "#0000FF32");
    }
}

/// <summary>
/// Ellipse drawing tool.
/// Draws an ellipse within a bounding rectangle.
/// </summary>
public class Ellipse : DrawingToolBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Ellipse"/> class.
    /// </summary>
    public Ellipse()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Ellipse"/> class.
    /// </summary>
    /// <param name="x1">First corner X coordinate (data space).</param>
    /// <param name="y1">First corner Y coordinate (data space).</param>
    /// <param name="x2">Second corner X coordinate (data space).</param>
    /// <param name="y2">Second corner Y coordinate (data space).</param>
    public Ellipse(double x1, double y1, double x2, double y2)
    {
        X1 = x1;
        Y1 = y1;
        X2 = x2;
        Y2 = y2;
    }

    /// <summary>
    /// Gets or sets the first corner X coordinate (data space).
    /// </summary>
    public double X1 { get; set; }

    /// <summary>
    /// Gets or sets the first corner Y coordinate (data space).
    /// </summary>
    public double Y1 { get; set; }

    /// <summary>
    /// Gets or sets the second corner X coordinate (data space).
    /// </summary>
    public double X2 { get; set; }

    /// <summary>
    /// Gets or sets the second corner Y coordinate (data space).
    /// </summary>
    public double Y2 { get; set; }

    /// <summary>
    /// Gets or sets whether to fill the ellipse.
    /// </summary>
    public bool Fill { get; set; }

    /// <summary>
    /// Gets or sets the fill color.
    /// </summary>
    public SKColor FillColor { get; set; } = new SKColor(0, 0, 255, 50);

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

        var rect = new SKRect(
            Math.Min(p1.X, p2.X),
            Math.Min(p1.Y, p2.Y),
            Math.Max(p1.X, p2.X),
            Math.Max(p1.Y, p2.Y)
        );

        // Fill
        if (Fill)
        {
            using var fillPaint = new SKPaint
            {
                Color = FillColor,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            context.Canvas.DrawOval(rect, fillPaint);
        }

        // Border
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

        context.Canvas.DrawOval(rect, paint);

        // Draw handles if selected
        if (IsSelected)
        {
            DrawHandle(context, rect.Left, rect.Top);
            DrawHandle(context, rect.Right, rect.Top);
            DrawHandle(context, rect.Left, rect.Bottom);
            DrawHandle(context, rect.Right, rect.Bottom);
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

        float centerX = (p1.X + p2.X) / 2;
        float centerY = (p1.Y + p2.Y) / 2;
        float radiusX = Math.Abs(p2.X - p1.X) / 2;
        float radiusY = Math.Abs(p2.Y - p1.Y) / 2;

        // Check if inside ellipse (approximate)
        float dx = (x - centerX) / radiusX;
        float dy = (y - centerY) / radiusY;
        float distanceSquared = dx * dx + dy * dy;

        return distanceSquared <= 1.0f + tolerance / Math.Min(radiusX, radiusY);
    }

    /// <inheritdoc/>
    public override Dictionary<string, object> Serialize()
    {
        var data = base.Serialize();
        data["X1"] = X1;
        data["Y1"] = Y1;
        data["X2"] = X2;
        data["Y2"] = Y2;
        data["Fill"] = Fill;
        data["FillColor"] = FillColor.ToString();
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

        if (data.TryGetValue("Fill", out var fill))
            Fill = Convert.ToBoolean(fill);

        if (data.TryGetValue("FillColor", out var fillColor))
            FillColor = SKColor.Parse(fillColor.ToString() ?? "#0000FF32");
    }
}
