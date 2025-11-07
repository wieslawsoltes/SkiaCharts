using SkiaSharp;

namespace SkiaCharts.Core.Theming;

/// <summary>
/// WCAG contrast ratio checker and color utilities.
/// </summary>
public static class ContrastChecker
{
    /// <summary>
    /// WCAG AA contrast ratio requirement for normal text (4.5:1).
    /// </summary>
    public const double WcagAA = 4.5;

    /// <summary>
    /// WCAG AA contrast ratio requirement for large text (3:1).
    /// </summary>
    public const double WcagAALarge = 3.0;

    /// <summary>
    /// WCAG AAA contrast ratio requirement for normal text (7:1).
    /// </summary>
    public const double WcagAAA = 7.0;

    /// <summary>
    /// WCAG AAA contrast ratio requirement for large text (4.5:1).
    /// </summary>
    public const double WcagAAALarge = 4.5;

    /// <summary>
    /// Calculates the WCAG contrast ratio between two colors.
    /// </summary>
    /// <param name="foreground">Foreground color.</param>
    /// <param name="background">Background color.</param>
    /// <returns>Contrast ratio (1:1 to 21:1).</returns>
    public static double GetContrastRatio(SKColor foreground, SKColor background)
    {
        var l1 = GetRelativeLuminance(foreground);
        var l2 = GetRelativeLuminance(background);

        var lighter = Math.Max(l1, l2);
        var darker = Math.Min(l1, l2);

        return (lighter + 0.05) / (darker + 0.05);
    }

    /// <summary>
    /// Gets the relative luminance of a color (0-1).
    /// </summary>
    public static double GetRelativeLuminance(SKColor color)
    {
        var r = GetLuminanceComponent(color.Red / 255.0);
        var g = GetLuminanceComponent(color.Green / 255.0);
        var b = GetLuminanceComponent(color.Blue / 255.0);

        return 0.2126 * r + 0.7152 * g + 0.0722 * b;
    }

    private static double GetLuminanceComponent(double component)
    {
        return component <= 0.03928
            ? component / 12.92
            : Math.Pow((component + 0.055) / 1.055, 2.4);
    }

    /// <summary>
    /// Checks if a color combination meets WCAG AA contrast requirements.
    /// </summary>
    public static bool MeetsWcagAA(SKColor foreground, SKColor background, bool isLargeText = false)
    {
        var ratio = GetContrastRatio(foreground, background);
        return ratio >= (isLargeText ? WcagAALarge : WcagAA);
    }

    /// <summary>
    /// Checks if a color combination meets WCAG AAA contrast requirements.
    /// </summary>
    public static bool MeetsWcagAAA(SKColor foreground, SKColor background, bool isLargeText = false)
    {
        var ratio = GetContrastRatio(foreground, background);
        return ratio >= (isLargeText ? WcagAAALarge : WcagAAA);
    }

    /// <summary>
    /// Checks if a color combination meets a custom contrast ratio.
    /// </summary>
    public static bool MeetsContrastRatio(SKColor foreground, SKColor background, double minimumRatio)
    {
        var ratio = GetContrastRatio(foreground, background);
        return ratio >= minimumRatio;
    }

    /// <summary>
    /// Finds the nearest color that meets the contrast requirement.
    /// </summary>
    public static SKColor AdjustForContrast(SKColor foreground, SKColor background, double targetRatio)
    {
        var currentRatio = GetContrastRatio(foreground, background);

        if (currentRatio >= targetRatio)
            return foreground;

        // Try darkening or lightening to meet contrast
        var darkened = DarkenUntilContrast(foreground, background, targetRatio);
        var lightened = LightenUntilContrast(foreground, background, targetRatio);

        var darkenedRatio = GetContrastRatio(darkened, background);
        var lightenedRatio = GetContrastRatio(lightened, background);

        // Return whichever is closer to target and meets requirement
        return Math.Abs(darkenedRatio - targetRatio) < Math.Abs(lightenedRatio - targetRatio)
            ? darkened
            : lightened;
    }

    /// <summary>
    /// Darkens a color until it meets the contrast requirement.
    /// </summary>
    public static SKColor DarkenUntilContrast(SKColor foreground, SKColor background, double targetRatio)
    {
        var color = foreground;
        var steps = 0;
        const int maxSteps = 100;

        while (GetContrastRatio(color, background) < targetRatio && steps < maxSteps)
        {
            color = Darken(color, 0.02f);
            steps++;
        }

        return color;
    }

    /// <summary>
    /// Lightens a color until it meets the contrast requirement.
    /// </summary>
    public static SKColor LightenUntilContrast(SKColor foreground, SKColor background, double targetRatio)
    {
        var color = foreground;
        var steps = 0;
        const int maxSteps = 100;

        while (GetContrastRatio(color, background) < targetRatio && steps < maxSteps)
        {
            color = Lighten(color, 0.02f);
            steps++;
        }

        return color;
    }

    /// <summary>
    /// Darkens a color by a percentage (0-1).
    /// </summary>
    public static SKColor Darken(SKColor color, float amount)
    {
        var factor = 1.0f - amount;
        return new SKColor(
            (byte)(color.Red * factor),
            (byte)(color.Green * factor),
            (byte)(color.Blue * factor),
            color.Alpha
        );
    }

    /// <summary>
    /// Lightens a color by a percentage (0-1).
    /// </summary>
    public static SKColor Lighten(SKColor color, float amount)
    {
        return new SKColor(
            (byte)Math.Min(255, color.Red + (255 - color.Red) * amount),
            (byte)Math.Min(255, color.Green + (255 - color.Green) * amount),
            (byte)Math.Min(255, color.Blue + (255 - color.Blue) * amount),
            color.Alpha
        );
    }

    /// <summary>
    /// Gets a contrasting text color (black or white) for the given background.
    /// </summary>
    public static SKColor GetContrastingTextColor(SKColor background)
    {
        var luminance = GetRelativeLuminance(background);

        // Use white text on dark backgrounds, black text on light backgrounds
        return luminance > 0.5 ? SKColors.Black : SKColors.White;
    }

    /// <summary>
    /// Validates all text colors in a theme against their backgrounds.
    /// </summary>
    public static ThemeContrastValidation ValidateThemeContrast(ChartTheme theme, double minimumRatio = WcagAA)
    {
        var validation = new ThemeContrastValidation();

        // Check axis labels
        var axisRatio = GetContrastRatio(theme.Axis.LabelColor, theme.Background.Color);
        validation.AddCheck("Axis Labels", axisRatio, minimumRatio, axisRatio >= minimumRatio);

        // Check axis title
        var axisTitleRatio = GetContrastRatio(theme.Axis.TitleColor, theme.Background.Color);
        validation.AddCheck("Axis Title", axisTitleRatio, minimumRatio, axisTitleRatio >= minimumRatio);

        // Check chart title
        var titleRatio = GetContrastRatio(theme.Title.Color, theme.Background.Color);
        validation.AddCheck("Chart Title", titleRatio, minimumRatio, titleRatio >= minimumRatio);

        // Check legend text
        var legendRatio = GetContrastRatio(theme.Legend.TextColor, theme.Legend.BackgroundColor);
        validation.AddCheck("Legend Text", legendRatio, minimumRatio, legendRatio >= minimumRatio);

        // Check tooltip text
        var tooltipRatio = GetContrastRatio(theme.Tooltip.TextColor, theme.Tooltip.BackgroundColor);
        validation.AddCheck("Tooltip Text", tooltipRatio, minimumRatio, tooltipRatio >= minimumRatio);

        return validation;
    }

    /// <summary>
    /// Automatically adjusts theme colors to meet contrast requirements.
    /// </summary>
    public static ChartTheme EnsureAccessibleContrast(ChartTheme theme, double minimumRatio = WcagAA)
    {
        var adjusted = theme.Clone();

        // Adjust axis labels
        if (GetContrastRatio(adjusted.Axis.LabelColor, adjusted.Background.Color) < minimumRatio)
        {
            adjusted.Axis.LabelColor = AdjustForContrast(
                adjusted.Axis.LabelColor,
                adjusted.Background.Color,
                minimumRatio);
        }

        // Adjust axis title
        if (GetContrastRatio(adjusted.Axis.TitleColor, adjusted.Background.Color) < minimumRatio)
        {
            adjusted.Axis.TitleColor = AdjustForContrast(
                adjusted.Axis.TitleColor,
                adjusted.Background.Color,
                minimumRatio);
        }

        // Adjust chart title
        if (GetContrastRatio(adjusted.Title.Color, adjusted.Background.Color) < minimumRatio)
        {
            adjusted.Title.Color = AdjustForContrast(
                adjusted.Title.Color,
                adjusted.Background.Color,
                minimumRatio);
        }

        // Adjust legend text
        if (GetContrastRatio(adjusted.Legend.TextColor, adjusted.Legend.BackgroundColor) < minimumRatio)
        {
            adjusted.Legend.TextColor = AdjustForContrast(
                adjusted.Legend.TextColor,
                adjusted.Legend.BackgroundColor,
                minimumRatio);
        }

        // Adjust tooltip text
        if (GetContrastRatio(adjusted.Tooltip.TextColor, adjusted.Tooltip.BackgroundColor) < minimumRatio)
        {
            adjusted.Tooltip.TextColor = AdjustForContrast(
                adjusted.Tooltip.TextColor,
                adjusted.Tooltip.BackgroundColor,
                minimumRatio);
        }

        return adjusted;
    }
}

/// <summary>
/// Represents a theme contrast validation result.
/// </summary>
public class ThemeContrastValidation
{
    private readonly List<ContrastCheck> _checks = new();

    /// <summary>
    /// Gets whether all contrast checks passed.
    /// </summary>
    public bool AllPassed => _checks.All(c => c.Passed);

    /// <summary>
    /// Gets all contrast checks.
    /// </summary>
    public IReadOnlyList<ContrastCheck> Checks => _checks;

    /// <summary>
    /// Gets failed checks.
    /// </summary>
    public IEnumerable<ContrastCheck> FailedChecks => _checks.Where(c => !c.Passed);

    internal void AddCheck(string element, double actualRatio, double requiredRatio, bool passed)
    {
        _checks.Add(new ContrastCheck
        {
            Element = element,
            ActualRatio = actualRatio,
            RequiredRatio = requiredRatio,
            Passed = passed
        });
    }

    /// <summary>
    /// Gets a summary of the validation results.
    /// </summary>
    public string GetSummary()
    {
        var passed = _checks.Count(c => c.Passed);
        var total = _checks.Count;
        var summary = $"Contrast Validation: {passed}/{total} checks passed\n";

        foreach (var check in FailedChecks)
        {
            summary += $"  ✗ {check.Element}: {check.ActualRatio:F2}:1 (required: {check.RequiredRatio:F2}:1)\n";
        }

        return summary;
    }
}

/// <summary>
/// Represents a single contrast check result.
/// </summary>
public class ContrastCheck
{
    /// <summary>
    /// Gets the element being checked.
    /// </summary>
    public required string Element { get; init; }

    /// <summary>
    /// Gets the actual contrast ratio.
    /// </summary>
    public double ActualRatio { get; init; }

    /// <summary>
    /// Gets the required contrast ratio.
    /// </summary>
    public double RequiredRatio { get; init; }

    /// <summary>
    /// Gets whether the check passed.
    /// </summary>
    public bool Passed { get; init; }
}
