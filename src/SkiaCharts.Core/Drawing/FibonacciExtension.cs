using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Drawing;

/// <summary>
/// Fibonacci extension drawing tool.
/// Draws extension levels beyond the retracement range.
/// </summary>
public class FibonacciExtension : DrawingToolBase
{
    private static readonly double[] DefaultLevels = { 0, 0.618, 1.0, 1.272, 1.618, 2.0, 2.618 };

    /// <summary>
    /// Initializes a new instance of the <see cref="FibonacciExtension"/> class.
    /// </summary>
    public FibonacciExtension()
    {
        Levels = DefaultLevels.ToArray();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FibonacciExtension"/> class.
    /// </summary>
    /// <param name="x1">First point X coordinate (data space).</param>
    /// <param name="y1">First point Y coordinate (data space).</param>
    /// <param name="x2">Second point X coordinate (data space).</param>
    /// <param name="y2">Second point Y coordinate (data space).</param>
    /// <param name="x3">Third point X coordinate (data space).</param>
    /// <param name="y3">Third point Y coordinate (data space).</param>
    public FibonacciExtension(double x1, double y1, double x2, double y2, double x3, double y3) : this()
    {
        X1 = x1;
        Y1 = y1;
        X2 = x2;
        Y2 = y2;
        X3 = x3;
        Y3 = y3;
    }

    /// <summary>
    /// Gets or sets the first point X coordinate (swing high/low).
    /// </summary>
    public double X1 { get; set; }

    /// <summary>
    /// Gets or sets the first point Y coordinate (swing high/low).
    /// </summary>
    public double Y1 { get; set; }

    /// <summary>
    /// Gets or sets the second point X coordinate (swing low/high).
    /// </summary>
    public double X2 { get; set; }

    /// <summary>
    /// Gets or sets the second point Y coordinate (swing low/high).
    /// </summary>
    public double Y2 { get; set; }

    /// <summary>
    /// Gets or sets the third point X coordinate (retracement point).
    /// </summary>
    public double X3 { get; set; }

    /// <summary>
    /// Gets or sets the third point Y coordinate (retracement point).
    /// </summary>
    public double Y3 { get; set; }

    /// <summary>
    /// Gets or sets the Fibonacci extension levels.
    /// </summary>
    public double[] Levels { get; set; }

    /// <summary>
    /// Gets or sets whether to show labels.
    /// </summary>
    public bool ShowLabels { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show price levels.
    /// </summary>
    public bool ShowPrices { get; set; } = true;

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

        var p1 = Viewport.DataToScreen(X1, Y1);
        var p2 = Viewport.DataToScreen(X2, Y2);
        var p3 = Viewport.DataToScreen(X3, Y3);

        // Calculate swing range and direction
        double swingRange = Y2 - Y1;
        bool isUpward = Y2 > Y1;

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

        // Draw each extension level from the retracement point
        foreach (var level in Levels)
        {
            double extensionPrice = Y3 + (swingRange * level);
            var screenY = Viewport.DataToScreen(0, extensionPrice).Y;

            // Draw horizontal line
            context.DrawLine(Viewport.ScreenRect.Left, screenY, Viewport.ScreenRect.Right, screenY, paint);

            // Draw label
            if (ShowLabels || ShowPrices)
            {
                using var textPaint = new SKPaint
                {
                    Color = Color,
                    TextSize = LabelFontSize,
                    IsAntialias = true
                };

                string label = "";
                if (ShowLabels)
                    label = $"{level:F3}";
                if (ShowPrices)
                    label += ShowLabels ? $" ({extensionPrice:F2})" : $"{extensionPrice:F2}";

                context.DrawText(label, Viewport.ScreenRect.Left + 5, screenY - 5, textPaint);
            }
        }

        // Draw connecting lines between the three points
        using var linePaint = new SKPaint
        {
            Color = Color.WithAlpha(128),
            StrokeWidth = 1,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true
        };

        context.DrawLine(p1.X, p1.Y, p2.X, p2.Y, linePaint);
        context.DrawLine(p2.X, p2.Y, p3.X, p3.Y, linePaint);

        // Draw handles if selected
        if (IsSelected)
        {
            DrawHandle(context, p1.X, p1.Y);
            DrawHandle(context, p2.X, p2.Y);
            DrawHandle(context, p3.X, p3.Y);
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

        double swingRange = Y2 - Y1;

        // Check if clicking near any extension level
        foreach (var level in Levels)
        {
            double extensionPrice = Y3 + (swingRange * level);
            var screenY = Viewport.DataToScreen(0, extensionPrice).Y;

            if (Math.Abs(y - screenY) <= tolerance)
                return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public override Dictionary<string, object> Serialize()
    {
        var data = base.Serialize();
        data["X1"] = X1;
        data["Y1"] = Y1;
        data["X2"] = X2;
        data["Y2"] = Y2;
        data["X3"] = X3;
        data["Y3"] = Y3;
        data["Levels"] = Levels;
        data["ShowLabels"] = ShowLabels;
        data["ShowPrices"] = ShowPrices;
        data["LabelFontSize"] = LabelFontSize;
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

        if (data.TryGetValue("X3", out var x3))
            X3 = Convert.ToDouble(x3);

        if (data.TryGetValue("Y3", out var y3))
            Y3 = Convert.ToDouble(y3);

        if (data.TryGetValue("Levels", out var levels) && levels is double[] levelArray)
            Levels = levelArray;

        if (data.TryGetValue("ShowLabels", out var showLabels))
            ShowLabels = Convert.ToBoolean(showLabels);

        if (data.TryGetValue("ShowPrices", out var showPrices))
            ShowPrices = Convert.ToBoolean(showPrices);

        if (data.TryGetValue("LabelFontSize", out var labelFontSize))
            LabelFontSize = Convert.ToSingle(labelFontSize);
    }
}
