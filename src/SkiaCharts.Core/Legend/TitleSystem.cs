using SkiaSharp;

namespace SkiaCharts.Core.Legend;

/// <summary>
/// Manages chart title and subtitle display.
/// </summary>
public class TitleManager
{
    private SKRect _titleBounds;
    private SKRect _subtitleBounds;

    /// <summary>
    /// Initializes a new instance of the <see cref="TitleManager"/> class.
    /// </summary>
    public TitleManager()
    {
        TitleFontSize = 16.0f;
        TitleFontFamily = "Arial";
        TitleColor = SKColors.Black;
        TitleFontStyle = SKFontStyle.Bold;

        SubtitleFontSize = 12.0f;
        SubtitleFontFamily = "Arial";
        SubtitleColor = new SKColor(100, 100, 100);
        SubtitleFontStyle = SKFontStyle.Normal;

        TitleAlignment = TextAlignment.Center;
        SubtitleAlignment = TextAlignment.Center;

        TitleMargin = 10.0f;
        SubtitleMargin = 5.0f;
        TitleSubtitleSpacing = 5.0f;
    }

    /// <summary>
    /// Gets or sets the title text.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the subtitle text.
    /// </summary>
    public string? Subtitle { get; set; }

    /// <summary>
    /// Gets or sets the title font size.
    /// </summary>
    public float TitleFontSize { get; set; }

    /// <summary>
    /// Gets or sets the title font family.
    /// </summary>
    public string TitleFontFamily { get; set; }

    /// <summary>
    /// Gets or sets the title color.
    /// </summary>
    public SKColor TitleColor { get; set; }

    /// <summary>
    /// Gets or sets the title font style.
    /// </summary>
    public SKFontStyle TitleFontStyle { get; set; }

    /// <summary>
    /// Gets or sets the subtitle font size.
    /// </summary>
    public float SubtitleFontSize { get; set; }

    /// <summary>
    /// Gets or sets the subtitle font family.
    /// </summary>
    public string SubtitleFontFamily { get; set; }

    /// <summary>
    /// Gets or sets the subtitle color.
    /// </summary>
    public SKColor SubtitleColor { get; set; }

    /// <summary>
    /// Gets or sets the subtitle font style.
    /// </summary>
    public SKFontStyle SubtitleFontStyle { get; set; }

    /// <summary>
    /// Gets or sets the title text alignment.
    /// </summary>
    public TextAlignment TitleAlignment { get; set; }

    /// <summary>
    /// Gets or sets the subtitle text alignment.
    /// </summary>
    public TextAlignment SubtitleAlignment { get; set; }

    /// <summary>
    /// Gets or sets the title margin (from top).
    /// </summary>
    public float TitleMargin { get; set; }

    /// <summary>
    /// Gets or sets the subtitle margin (from title).
    /// </summary>
    public float SubtitleMargin { get; set; }

    /// <summary>
    /// Gets or sets the spacing between title and subtitle.
    /// </summary>
    public float TitleSubtitleSpacing { get; set; }

    /// <summary>
    /// Gets whether a title is set.
    /// </summary>
    public bool HasTitle => !string.IsNullOrWhiteSpace(Title);

    /// <summary>
    /// Gets whether a subtitle is set.
    /// </summary>
    public bool HasSubtitle => !string.IsNullOrWhiteSpace(Subtitle);

    /// <summary>
    /// Gets the calculated title bounds.
    /// </summary>
    public SKRect TitleBounds => _titleBounds;

    /// <summary>
    /// Gets the calculated subtitle bounds.
    /// </summary>
    public SKRect SubtitleBounds => _subtitleBounds;

    /// <summary>
    /// Calculates the total height required for title and subtitle.
    /// </summary>
    /// <returns>The total height in pixels.</returns>
    public float CalculateTotalHeight()
    {
        float height = 0;

        if (HasTitle)
        {
            using var paint = CreateTitlePaint();
            var bounds = new SKRect();
            paint.MeasureText(Title, ref bounds);
            height += TitleMargin + bounds.Height;
        }

        if (HasSubtitle)
        {
            using var paint = CreateSubtitlePaint();
            var bounds = new SKRect();
            paint.MeasureText(Subtitle, ref bounds);
            height += (HasTitle ? TitleSubtitleSpacing : SubtitleMargin) + bounds.Height;
        }

        return height;
    }

    /// <summary>
    /// Calculates the layout for title and subtitle.
    /// </summary>
    /// <param name="chartBounds">The chart bounds.</param>
    public void CalculateLayout(SKRect chartBounds)
    {
        float currentY = chartBounds.Top;

        if (HasTitle)
        {
            using var paint = CreateTitlePaint();
            var textBounds = new SKRect();
            paint.MeasureText(Title, ref textBounds);

            currentY += TitleMargin;

            var x = CalculateXPosition(chartBounds, textBounds.Width, TitleAlignment);

            _titleBounds = new SKRect(
                x,
                currentY,
                x + textBounds.Width,
                currentY + textBounds.Height
            );

            currentY += textBounds.Height;
        }

        if (HasSubtitle)
        {
            using var paint = CreateSubtitlePaint();
            var textBounds = new SKRect();
            paint.MeasureText(Subtitle, ref textBounds);

            currentY += HasTitle ? TitleSubtitleSpacing : SubtitleMargin;

            var x = CalculateXPosition(chartBounds, textBounds.Width, SubtitleAlignment);

            _subtitleBounds = new SKRect(
                x,
                currentY,
                x + textBounds.Width,
                currentY + textBounds.Height
            );
        }
    }

    /// <summary>
    /// Renders the title and subtitle.
    /// </summary>
    /// <param name="canvas">The canvas to render on.</param>
    /// <param name="chartBounds">The chart bounds.</param>
    public void Render(SKCanvas canvas, SKRect chartBounds)
    {
        if (HasTitle)
        {
            using var paint = CreateTitlePaint();
            var textBounds = new SKRect();
            paint.MeasureText(Title, ref textBounds);

            canvas.DrawText(Title, _titleBounds.Left, _titleBounds.Bottom - textBounds.Bottom, paint);
        }

        if (HasSubtitle)
        {
            using var paint = CreateSubtitlePaint();
            var textBounds = new SKRect();
            paint.MeasureText(Subtitle, ref textBounds);

            canvas.DrawText(Subtitle, _subtitleBounds.Left, _subtitleBounds.Bottom - textBounds.Bottom, paint);
        }
    }

    private SKPaint CreateTitlePaint()
    {
        return new SKPaint
        {
            Color = TitleColor,
            TextSize = TitleFontSize,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName(TitleFontFamily, TitleFontStyle)
        };
    }

    private SKPaint CreateSubtitlePaint()
    {
        return new SKPaint
        {
            Color = SubtitleColor,
            TextSize = SubtitleFontSize,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName(SubtitleFontFamily, SubtitleFontStyle)
        };
    }

    private float CalculateXPosition(SKRect chartBounds, float textWidth, TextAlignment alignment)
    {
        return alignment switch
        {
            TextAlignment.Left => chartBounds.Left,
            TextAlignment.Center => chartBounds.MidX - textWidth / 2,
            TextAlignment.Right => chartBounds.Right - textWidth,
            _ => chartBounds.Left
        };
    }
}

/// <summary>
/// Text alignment enumeration.
/// </summary>
public enum TextAlignment
{
    /// <summary>Left-aligned text.</summary>
    Left,
    /// <summary>Center-aligned text.</summary>
    Center,
    /// <summary>Right-aligned text.</summary>
    Right
}
