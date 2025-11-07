using SkiaSharp;

namespace SkiaCharts.Core.Theming;

/// <summary>
/// Pattern fill types for accessibility (B&amp;W printing and colorblind users).
/// </summary>
public enum PatternType
{
    /// <summary>Solid fill (no pattern).</summary>
    Solid,

    /// <summary>Horizontal lines.</summary>
    HorizontalLines,

    /// <summary>Vertical lines.</summary>
    VerticalLines,

    /// <summary>Diagonal lines (45° right).</summary>
    DiagonalRight,

    /// <summary>Diagonal lines (45° left).</summary>
    DiagonalLeft,

    /// <summary>Crosshatch (horizontal + vertical).</summary>
    Crosshatch,

    /// <summary>Diagonal crosshatch (X pattern).</summary>
    DiagonalCrosshatch,

    /// <summary>Dots pattern.</summary>
    Dots,

    /// <summary>Small checkerboard.</summary>
    Checkerboard,

    /// <summary>Large checkerboard.</summary>
    LargeCheckerboard,

    /// <summary>Brick pattern.</summary>
    Brick,

    /// <summary>Weave pattern.</summary>
    Weave,

    /// <summary>Dense dots.</summary>
    DenseDots,

    /// <summary>Sparse dots.</summary>
    SparseDots,

    /// <summary>Zigzag pattern.</summary>
    Zigzag,

    /// <summary>Scales pattern.</summary>
    Scales
}

/// <summary>
/// Creates pattern fills for accessibility.
/// </summary>
public static class PatternFills
{
    /// <summary>
    /// Creates a pattern shader for the specified pattern type.
    /// </summary>
    /// <param name="pattern">The pattern type.</param>
    /// <param name="foreground">Foreground color.</param>
    /// <param name="background">Background color.</param>
    /// <param name="scale">Pattern scale (default: 1.0).</param>
    /// <returns>A shader that can be used with SKPaint.</returns>
    public static SKShader CreatePattern(PatternType pattern, SKColor foreground, SKColor background, float scale = 1.0f)
    {
        var size = (int)(20 * scale);
        var bitmap = new SKBitmap(size, size);

        using (var canvas = new SKCanvas(bitmap))
        {
            // Fill background
            canvas.Clear(background);

            using var paint = new SKPaint
            {
                Color = foreground,
                IsAntialias = false,
                StrokeWidth = Math.Max(1, scale)
            };

            switch (pattern)
            {
                case PatternType.Solid:
                    canvas.Clear(foreground);
                    break;

                case PatternType.HorizontalLines:
                    DrawHorizontalLines(canvas, paint, size);
                    break;

                case PatternType.VerticalLines:
                    DrawVerticalLines(canvas, paint, size);
                    break;

                case PatternType.DiagonalRight:
                    DrawDiagonalRight(canvas, paint, size);
                    break;

                case PatternType.DiagonalLeft:
                    DrawDiagonalLeft(canvas, paint, size);
                    break;

                case PatternType.Crosshatch:
                    DrawHorizontalLines(canvas, paint, size);
                    DrawVerticalLines(canvas, paint, size);
                    break;

                case PatternType.DiagonalCrosshatch:
                    DrawDiagonalRight(canvas, paint, size);
                    DrawDiagonalLeft(canvas, paint, size);
                    break;

                case PatternType.Dots:
                    DrawDots(canvas, paint, size, 2);
                    break;

                case PatternType.DenseDots:
                    DrawDots(canvas, paint, size, 1);
                    break;

                case PatternType.SparseDots:
                    DrawDots(canvas, paint, size, 3);
                    break;

                case PatternType.Checkerboard:
                    DrawCheckerboard(canvas, paint, size, 2);
                    break;

                case PatternType.LargeCheckerboard:
                    DrawCheckerboard(canvas, paint, size, 1);
                    break;

                case PatternType.Brick:
                    DrawBrick(canvas, paint, size);
                    break;

                case PatternType.Weave:
                    DrawWeave(canvas, paint, size);
                    break;

                case PatternType.Zigzag:
                    DrawZigzag(canvas, paint, size);
                    break;

                case PatternType.Scales:
                    DrawScales(canvas, paint, size);
                    break;
            }
        }

        return SKShader.CreateBitmap(bitmap, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
    }

    private static void DrawHorizontalLines(SKCanvas canvas, SKPaint paint, int size)
    {
        paint.Style = SKPaintStyle.Stroke;
        for (int y = 0; y < size; y += size / 4)
        {
            canvas.DrawLine(0, y, size, y, paint);
        }
    }

    private static void DrawVerticalLines(SKCanvas canvas, SKPaint paint, int size)
    {
        paint.Style = SKPaintStyle.Stroke;
        for (int x = 0; x < size; x += size / 4)
        {
            canvas.DrawLine(x, 0, x, size, paint);
        }
    }

    private static void DrawDiagonalRight(SKCanvas canvas, SKPaint paint, int size)
    {
        paint.Style = SKPaintStyle.Stroke;
        for (int i = -size; i < size * 2; i += size / 4)
        {
            canvas.DrawLine(i, 0, i + size, size, paint);
        }
    }

    private static void DrawDiagonalLeft(SKCanvas canvas, SKPaint paint, int size)
    {
        paint.Style = SKPaintStyle.Stroke;
        for (int i = 0; i < size * 2; i += size / 4)
        {
            canvas.DrawLine(i, 0, i - size, size, paint);
        }
    }

    private static void DrawDots(SKCanvas canvas, SKPaint paint, int size, int spacing)
    {
        paint.Style = SKPaintStyle.Fill;
        var step = size / (2 * spacing);
        var radius = Math.Max(1, size / 20);

        for (int x = step; x < size; x += step)
        {
            for (int y = step; y < size; y += step)
            {
                canvas.DrawCircle(x, y, radius, paint);
            }
        }
    }

    private static void DrawCheckerboard(SKCanvas canvas, SKPaint paint, int size, int divisions)
    {
        paint.Style = SKPaintStyle.Fill;
        var cellSize = size / divisions;

        for (int x = 0; x < divisions; x++)
        {
            for (int y = 0; y < divisions; y++)
            {
                if ((x + y) % 2 == 0)
                {
                    canvas.DrawRect(x * cellSize, y * cellSize, cellSize, cellSize, paint);
                }
            }
        }
    }

    private static void DrawBrick(SKCanvas canvas, SKPaint paint, int size)
    {
        paint.Style = SKPaintStyle.Stroke;
        var brickHeight = size / 4;
        var brickWidth = size / 2;

        for (int y = 0; y < size; y += brickHeight)
        {
            var row = y / brickHeight;
            var offset = (row % 2 == 0) ? 0 : brickWidth / 2;

            canvas.DrawLine(0, y, size, y, paint);

            for (int x = offset; x < size; x += brickWidth)
            {
                canvas.DrawLine(x, y, x, y + brickHeight, paint);
            }
        }
    }

    private static void DrawWeave(SKCanvas canvas, SKPaint paint, int size)
    {
        paint.Style = SKPaintStyle.Fill;
        var cellSize = size / 4;

        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if ((x % 2 == 0 && y % 2 == 0) || (x % 2 == 1 && y % 2 == 1))
                {
                    canvas.DrawRect(x * cellSize, y * cellSize, cellSize, cellSize, paint);
                }
            }
        }
    }

    private static void DrawZigzag(SKCanvas canvas, SKPaint paint, int size)
    {
        paint.Style = SKPaintStyle.Stroke;
        using var path = new SKPath();

        var step = size / 4;
        path.MoveTo(0, step);

        for (int x = 0; x < size; x += step)
        {
            path.LineTo(x + step / 2, 0);
            path.LineTo(x + step, step);
        }

        for (int y = 0; y < size; y += step * 2)
        {
            canvas.Save();
            canvas.Translate(0, y);
            canvas.DrawPath(path, paint);
            canvas.Restore();
        }
    }

    private static void DrawScales(SKCanvas canvas, SKPaint paint, int size)
    {
        paint.Style = SKPaintStyle.Stroke;
        var radius = size / 4;

        for (int y = 0; y < size + radius; y += radius)
        {
            for (int x = -radius; x < size + radius; x += radius * 2)
            {
                var offset = (y / radius) % 2 == 0 ? 0 : radius;
                canvas.DrawArc(
                    new SKRect(x + offset - radius, y - radius, x + offset + radius, y + radius),
                    0, 180, false, paint);
            }
        }
    }
}

/// <summary>
/// Pattern fill presets for common use cases.
/// </summary>
public static class PatternPresets
{
    /// <summary>
    /// Gets a set of distinct patterns for categorical data (B&amp;W printing).
    /// </summary>
    public static PatternType[] CategoricalPatterns => new[]
    {
        PatternType.Solid,
        PatternType.HorizontalLines,
        PatternType.VerticalLines,
        PatternType.DiagonalRight,
        PatternType.Crosshatch,
        PatternType.Dots,
        PatternType.Checkerboard,
        PatternType.DiagonalCrosshatch
    };

    /// <summary>
    /// Gets a set of density-based patterns for sequential data.
    /// </summary>
    public static PatternType[] SequentialPatterns => new[]
    {
        PatternType.SparseDots,
        PatternType.Dots,
        PatternType.DenseDots,
        PatternType.HorizontalLines,
        PatternType.Crosshatch,
        PatternType.Checkerboard,
        PatternType.Solid
    };

    /// <summary>
    /// Creates a paint with pattern fill.
    /// </summary>
    public static SKPaint CreatePatternPaint(PatternType pattern, SKColor foreground, SKColor background, float scale = 1.0f)
    {
        var shader = PatternFills.CreatePattern(pattern, foreground, background, scale);
        return new SKPaint
        {
            Shader = shader,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
    }

    /// <summary>
    /// Gets the pattern for a specific index (wraps around).
    /// </summary>
    public static PatternType GetCategoricalPattern(int index)
    {
        return CategoricalPatterns[index % CategoricalPatterns.Length];
    }
}

/// <summary>
/// Accessibility options for chart rendering.
/// </summary>
public class AccessibilityOptions
{
    /// <summary>
    /// Gets or sets whether to use pattern fills instead of solid colors.
    /// </summary>
    public bool UsePatternFills { get; set; } = false;

    /// <summary>
    /// Gets or sets the pattern scale factor.
    /// </summary>
    public float PatternScale { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets whether to use high contrast colors.
    /// </summary>
    public bool UseHighContrast { get; set; } = false;

    /// <summary>
    /// Gets or sets the colorblind type to optimize for.
    /// </summary>
    public ColorblindType ColorblindType { get; set; } = ColorblindType.None;

    /// <summary>
    /// Gets or sets whether to show data labels by default.
    /// </summary>
    public bool ShowDataLabels { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to use larger fonts for readability.
    /// </summary>
    public bool UseLargeFonts { get; set; } = false;

    /// <summary>
    /// Gets or sets the minimum contrast ratio (WCAG AA = 4.5, AAA = 7.0).
    /// </summary>
    public double MinimumContrastRatio { get; set; } = 4.5;

    /// <summary>
    /// Gets or sets whether to enable keyboard navigation.
    /// </summary>
    public bool EnableKeyboardNavigation { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to provide audio descriptions.
    /// </summary>
    public bool EnableAudioDescriptions { get; set; } = false;

    /// <summary>
    /// Creates default accessibility options.
    /// </summary>
    public static AccessibilityOptions Default => new();

    /// <summary>
    /// Creates accessibility options optimized for colorblind users.
    /// </summary>
    public static AccessibilityOptions ForColorblind(ColorblindType type) => new()
    {
        ColorblindType = type,
        UseHighContrast = true,
        ShowDataLabels = true
    };

    /// <summary>
    /// Creates accessibility options optimized for B&amp;W printing.
    /// </summary>
    public static AccessibilityOptions ForBlackAndWhite() => new()
    {
        UsePatternFills = true,
        PatternScale = 1.0f,
        ShowDataLabels = true,
        UseHighContrast = true
    };

    /// <summary>
    /// Creates accessibility options optimized for screen readers.
    /// </summary>
    public static AccessibilityOptions ForScreenReaders() => new()
    {
        EnableKeyboardNavigation = true,
        EnableAudioDescriptions = true,
        ShowDataLabels = true,
        UseLargeFonts = true
    };

    /// <summary>
    /// Creates maximum accessibility options (all features enabled).
    /// </summary>
    public static AccessibilityOptions Maximum() => new()
    {
        UsePatternFills = true,
        PatternScale = 1.2f,
        UseHighContrast = true,
        ColorblindType = ColorblindType.Deuteranopia,
        ShowDataLabels = true,
        UseLargeFonts = true,
        MinimumContrastRatio = 7.0,
        EnableKeyboardNavigation = true,
        EnableAudioDescriptions = true
    };
}
