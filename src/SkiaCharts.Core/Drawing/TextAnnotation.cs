using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Drawing;

/// <summary>
/// Text annotation drawing tool.
/// Draws text at a specified position on the chart.
/// </summary>
public class TextAnnotation : DrawingToolBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextAnnotation"/> class.
    /// </summary>
    public TextAnnotation()
    {
        Text = "Text";
        FontSize = 14f;
        TextColor = SKColors.Black;
        BackgroundColor = new SKColor(255, 255, 255, 200);
        ShowBackground = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextAnnotation"/> class.
    /// </summary>
    /// <param name="x">X coordinate (data space).</param>
    /// <param name="y">Y coordinate (data space).</param>
    /// <param name="text">The text to display.</param>
    public TextAnnotation(double x, double y, string text) : this()
    {
        X = x;
        Y = y;
        Text = text;
    }

    /// <summary>
    /// Gets or sets the X coordinate (data space).
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Gets or sets the Y coordinate (data space).
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Gets or sets the text to display.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the font size.
    /// </summary>
    public float FontSize { get; set; }

    /// <summary>
    /// Gets or sets the text color.
    /// </summary>
    public SKColor TextColor { get; set; }

    /// <summary>
    /// Gets or sets whether to show a background.
    /// </summary>
    public bool ShowBackground { get; set; }

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    public SKColor BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the padding around the text.
    /// </summary>
    public float Padding { get; set; } = 4f;

    /// <summary>
    /// Gets or sets the border radius.
    /// </summary>
    public float BorderRadius { get; set; } = 4f;

    /// <summary>
    /// Gets or sets whether the text is bold.
    /// </summary>
    public bool Bold { get; set; }

    /// <summary>
    /// Gets or sets whether the text is italic.
    /// </summary>
    public bool Italic { get; set; }

    /// <summary>
    /// Sets the viewport for coordinate transformation.
    /// </summary>
    public ViewportManager? Viewport { get; set; }

    /// <inheritdoc/>
    public override void Render(IRenderContext context)
    {
        if (!IsVisible || Viewport == null || string.IsNullOrEmpty(Text))
            return;

        var screenPos = Viewport.DataToScreen(X, Y);

        using var textPaint = new SKPaint
        {
            Color = IsSelected ? SKColors.Yellow : TextColor,
            TextSize = FontSize,
            IsAntialias = true,
            FakeBoldText = Bold
        };

        if (Italic)
        {
            textPaint.TextSkewX = -0.25f;
        }

        // Measure text
        var bounds = new SKRect();
        textPaint.MeasureText(Text, ref bounds);

        float textWidth = bounds.Width;
        float textHeight = bounds.Height;

        // Draw background
        if (ShowBackground)
        {
            var bgRect = new SKRect(
                screenPos.X - Padding,
                screenPos.Y - textHeight - Padding,
                screenPos.X + textWidth + Padding,
                screenPos.Y + Padding
            );

            using var bgPaint = new SKPaint
            {
                Color = BackgroundColor,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            if (BorderRadius > 0)
            {
                context.DrawRoundRect(bgRect, BorderRadius, BorderRadius, bgPaint);
            }
            else
            {
                context.DrawRect(bgRect, bgPaint);
            }

            // Draw border
            using var borderPaint = new SKPaint
            {
                Color = IsSelected ? SKColors.Yellow : Color,
                StrokeWidth = 1,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            if (BorderRadius > 0)
            {
                context.DrawRoundRect(bgRect, BorderRadius, BorderRadius, borderPaint);
            }
            else
            {
                context.DrawRect(bgRect, borderPaint);
            }
        }

        // Draw text
        context.DrawText(Text, screenPos.X, screenPos.Y, textPaint);

        // Draw handle if selected
        if (IsSelected)
        {
            DrawHandle(context, screenPos.X, screenPos.Y);
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
        if (Viewport == null || string.IsNullOrEmpty(Text))
            return false;

        var screenPos = Viewport.DataToScreen(X, Y);

        using var textPaint = new SKPaint
        {
            TextSize = FontSize
        };

        var bounds = new SKRect();
        textPaint.MeasureText(Text, ref bounds);

        var hitRect = new SKRect(
            screenPos.X - Padding,
            screenPos.Y - bounds.Height - Padding,
            screenPos.X + bounds.Width + Padding,
            screenPos.Y + Padding
        );

        return hitRect.Contains(x, y);
    }

    /// <inheritdoc/>
    public override Dictionary<string, object> Serialize()
    {
        var data = base.Serialize();
        data["X"] = X;
        data["Y"] = Y;
        data["Text"] = Text;
        data["FontSize"] = FontSize;
        data["TextColor"] = TextColor.ToString();
        data["ShowBackground"] = ShowBackground;
        data["BackgroundColor"] = BackgroundColor.ToString();
        data["Padding"] = Padding;
        data["BorderRadius"] = BorderRadius;
        data["Bold"] = Bold;
        data["Italic"] = Italic;
        return data;
    }

    /// <inheritdoc/>
    public override void Deserialize(Dictionary<string, object> data)
    {
        base.Deserialize(data);

        if (data.TryGetValue("X", out var x))
            X = Convert.ToDouble(x);

        if (data.TryGetValue("Y", out var y))
            Y = Convert.ToDouble(y);

        if (data.TryGetValue("Text", out var text))
            Text = text.ToString() ?? "Text";

        if (data.TryGetValue("FontSize", out var fontSize))
            FontSize = Convert.ToSingle(fontSize);

        if (data.TryGetValue("TextColor", out var textColor))
            TextColor = SKColor.Parse(textColor.ToString() ?? "#000000");

        if (data.TryGetValue("ShowBackground", out var showBackground))
            ShowBackground = Convert.ToBoolean(showBackground);

        if (data.TryGetValue("BackgroundColor", out var backgroundColor))
            BackgroundColor = SKColor.Parse(backgroundColor.ToString() ?? "#FFFFFFC8");

        if (data.TryGetValue("Padding", out var padding))
            Padding = Convert.ToSingle(padding);

        if (data.TryGetValue("BorderRadius", out var borderRadius))
            BorderRadius = Convert.ToSingle(borderRadius);

        if (data.TryGetValue("Bold", out var bold))
            Bold = Convert.ToBoolean(bold);

        if (data.TryGetValue("Italic", out var italic))
            Italic = Convert.ToBoolean(italic);
    }
}
