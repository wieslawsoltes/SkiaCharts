using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Drawing;

/// <summary>
/// Fibonacci retracement drawing tool.
/// Draws horizontal levels at key Fibonacci ratios.
/// </summary>
public class FibonacciRetracement : DrawingToolBase
{
    private static readonly double[] DefaultLevels = { 0, 0.236, 0.382, 0.5, 0.618, 0.786, 1.0 };

    /// <summary>
    /// Initializes a new instance of the <see cref="FibonacciRetracement"/> class.
    /// </summary>
    public FibonacciRetracement()
    {
        Levels = DefaultLevels.ToArray();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FibonacciRetracement"/> class.
    /// </summary>
    /// <param name="x1">First point X coordinate (data space).</param>
    /// <param name="y1">First point Y coordinate (data space).</param>
    /// <param name="x2">Second point X coordinate (data space).</param>
    /// <param name="y2">Second point Y coordinate (data space).</param>
    public FibonacciRetracement(double x1, double y1, double x2, double y2) : this()
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
    /// Gets or sets the Fibonacci levels (0 = start, 1 = end).
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

        double range = Y2 - Y1;

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

        // Draw each Fibonacci level
        foreach (var level in Levels)
        {
            double priceLevel = Y1 + (range * level);
            var screenY = Viewport.DataToScreen(0, priceLevel).Y;

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
                    label = $"{level:P1}";
                if (ShowPrices)
                    label += ShowLabels ? $" ({priceLevel:F2})" : $"{priceLevel:F2}";

                context.DrawText(label, Viewport.ScreenRect.Left + 5, screenY - 5, textPaint);
            }
        }

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

        double range = Y2 - Y1;

        // Check if clicking near any Fibonacci level
        foreach (var level in Levels)
        {
            double priceLevel = Y1 + (range * level);
            var screenY = Viewport.DataToScreen(0, priceLevel).Y;

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
