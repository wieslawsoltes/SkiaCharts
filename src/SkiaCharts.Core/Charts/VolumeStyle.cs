using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Defines the coloring mode for volume bars.
/// </summary>
public enum VolumeColorMode
{
    /// <summary>
    /// Single color for all volume bars.
    /// </summary>
    Single,

    /// <summary>
    /// Color based on price direction (bullish/bearish).
    /// </summary>
    PriceDirection,

    /// <summary>
    /// Color based on volume increasing or decreasing.
    /// </summary>
    VolumeDirection
}

/// <summary>
/// Configuration for volume bar series style.
/// </summary>
public class VolumeSeriesStyle
{
    /// <summary>
    /// Gets or sets the color mode for volume bars.
    /// </summary>
    public VolumeColorMode ColorMode { get; set; } = VolumeColorMode.PriceDirection;

    /// <summary>
    /// Gets or sets the default color (used in Single mode).
    /// </summary>
    public SKColor DefaultColor { get; set; } = new SKColor(100, 100, 100);

    /// <summary>
    /// Gets or sets the color for bullish volume bars.
    /// </summary>
    public SKColor BullishColor { get; set; } = new SKColor(38, 166, 91);

    /// <summary>
    /// Gets or sets the color for bearish volume bars.
    /// </summary>
    public SKColor BearishColor { get; set; } = new SKColor(239, 83, 80);

    /// <summary>
    /// Gets or sets the color for increasing volume.
    /// </summary>
    public SKColor IncreasingColor { get; set; } = new SKColor(38, 166, 91);

    /// <summary>
    /// Gets or sets the color for decreasing volume.
    /// </summary>
    public SKColor DecreasingColor { get; set; } = new SKColor(239, 83, 80);

    /// <summary>
    /// Gets or sets the opacity for volume bars (0-255).
    /// </summary>
    public byte Opacity { get; set; } = 180;

    /// <summary>
    /// Gets or sets the bar width ratio (0-1).
    /// Default is 0.8 (80% of available space).
    /// </summary>
    public double BarWidthRatio { get; set; } = 0.8;

    /// <summary>
    /// Gets or sets the minimum bar width in pixels.
    /// </summary>
    public float MinimumBarWidth { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the maximum bar width in pixels (0 = no limit).
    /// </summary>
    public float MaximumBarWidth { get; set; } = 20f;

    /// <summary>
    /// Gets or sets the border color (null for no border).
    /// </summary>
    public SKColor? BorderColor { get; set; }

    /// <summary>
    /// Gets or sets the border width in pixels.
    /// </summary>
    public float BorderWidth { get; set; } = 1f;
}

/// <summary>
/// Configuration for volume chart layout.
/// </summary>
public class VolumeChartConfiguration
{
    /// <summary>
    /// Gets or sets the height ratio for the volume panel (0-1).
    /// When combined with price chart, this determines the relative height.
    /// Default is 0.25 (25% of chart height).
    /// </summary>
    public double VolumePanelRatio { get; set; } = 0.25;

    /// <summary>
    /// Gets or sets whether to show the Y-axis for volume.
    /// </summary>
    public bool ShowVolumeAxis { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show grid lines in the volume panel.
    /// </summary>
    public bool ShowGridLines { get; set; } = false;

    /// <summary>
    /// Gets or sets the baseline value for volume bars (typically 0).
    /// </summary>
    public double Baseline { get; set; } = 0.0;
}
