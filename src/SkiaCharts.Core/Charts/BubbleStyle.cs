using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Defines the size scaling algorithm for bubble charts.
/// </summary>
public enum BubbleSizeScale
{
    /// <summary>
    /// Linear scaling - size is proportional to data value.
    /// </summary>
    Linear,

    /// <summary>
    /// Area scaling - bubble area is proportional to data value.
    /// Size is sqrt(value) so area scales linearly.
    /// </summary>
    Area,

    /// <summary>
    /// Logarithmic scaling - size is proportional to log(value).
    /// Useful for values spanning multiple orders of magnitude.
    /// </summary>
    Logarithmic
}

/// <summary>
/// Configuration for a bubble series style.
/// </summary>
public class BubbleSeriesStyle
{
    /// <summary>
    /// Gets or sets the default fill color for bubbles.
    /// </summary>
    public SKColor FillColor { get; set; } = new SKColor(0, 122, 255);

    /// <summary>
    /// Gets or sets the opacity for bubbles (0-255).
    /// </summary>
    public byte Opacity { get; set; } = 180;

    /// <summary>
    /// Gets or sets the minimum bubble radius in pixels.
    /// </summary>
    public float MinBubbleSize { get; set; } = 3f;

    /// <summary>
    /// Gets or sets the maximum bubble radius in pixels.
    /// </summary>
    public float MaxBubbleSize { get; set; } = 40f;

    /// <summary>
    /// Gets or sets the size scaling algorithm.
    /// </summary>
    public BubbleSizeScale SizeScale { get; set; } = BubbleSizeScale.Area;

    /// <summary>
    /// Gets or sets whether to use variable colors based on ColorValue.
    /// </summary>
    public bool UseColorMapping { get; set; } = false;

    /// <summary>
    /// Gets or sets the color scale for color mapping.
    /// If null, uses a default blue-to-red scale.
    /// </summary>
    public SKColor[]? ColorScale { get; set; }

    /// <summary>
    /// Gets or sets the border color (null for no border).
    /// </summary>
    public SKColor? BorderColor { get; set; }

    /// <summary>
    /// Gets or sets the border width in pixels.
    /// </summary>
    public float BorderWidth { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the border opacity (0-255).
    /// </summary>
    public byte BorderOpacity { get; set; } = 255;

    /// <summary>
    /// Gets or sets whether to show labels on bubbles.
    /// </summary>
    public bool ShowLabels { get; set; } = false;

    /// <summary>
    /// Gets or sets the label format string.
    /// Default is "F1" for one decimal place.
    /// Use "{0}" for X, "{1}" for Y, "{2}" for Size in the format.
    /// </summary>
    public string LabelFormat { get; set; } = "{2:F1}";

    /// <summary>
    /// Gets or sets the label color.
    /// </summary>
    public SKColor LabelColor { get; set; } = SKColors.White;

    /// <summary>
    /// Gets or sets the label font size.
    /// </summary>
    public float LabelFontSize { get; set; } = 10f;

    /// <summary>
    /// Gets or sets the minimum bubble size to show labels.
    /// Labels are hidden for smaller bubbles to avoid clutter.
    /// </summary>
    public float MinLabelSize { get; set; } = 15f;
}

/// <summary>
/// Configuration for bubble chart layout.
/// </summary>
public class BubbleChartConfiguration
{
    /// <summary>
    /// Gets or sets whether to enable label collision detection.
    /// When true, overlapping labels are hidden.
    /// </summary>
    public bool EnableLabelCollisionDetection { get; set; } = true;

    /// <summary>
    /// Gets or sets the padding around labels for collision detection.
    /// </summary>
    public float LabelCollisionPadding { get; set; } = 2f;

    /// <summary>
    /// Gets or sets whether bubbles can overlap.
    /// When false, bubble sizes are adjusted to prevent overlap (future enhancement).
    /// </summary>
    public bool AllowBubbleOverlap { get; set; } = true;
}
