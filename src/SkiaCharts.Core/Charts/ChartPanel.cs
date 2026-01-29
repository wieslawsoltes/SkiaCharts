using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Represents a single panel in a multi-panel chart container.
/// Each panel can contain its own chart with independent Y-axis.
/// </summary>
public class ChartPanel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartPanel"/> class.
    /// </summary>
    public ChartPanel()
    {
        Id = Guid.NewGuid().ToString();
        Height = 1.0; // Default height ratio
        IsVisible = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartPanel"/> class.
    /// </summary>
    /// <param name="chart">The chart to display in this panel.</param>
    public ChartPanel(ChartBase chart) : this()
    {
        Chart = chart;
    }

    /// <summary>
    /// Gets or sets the unique identifier for this panel.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the chart displayed in this panel.
    /// </summary>
    public ChartBase? Chart { get; set; }

    /// <summary>
    /// Gets or sets the height ratio of this panel relative to other panels.
    /// Default is 1.0. A value of 2.0 would make this panel twice as tall.
    /// </summary>
    public double Height { get; set; }

    /// <summary>
    /// Gets or sets whether this panel is visible.
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Gets or sets the title of this panel.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the background color of this panel.
    /// </summary>
    public SKColor BackgroundColor { get; set; } = SKColors.White;

    /// <summary>
    /// Gets or sets the minimum height in pixels for this panel.
    /// </summary>
    public float MinHeight { get; set; } = 50f;

    /// <summary>
    /// Gets or sets the maximum height in pixels for this panel (0 = no limit).
    /// </summary>
    public float MaxHeight { get; set; } = 0f;

    /// <summary>
    /// Gets or sets the computed bounds of this panel in screen coordinates.
    /// This is set by the container during layout.
    /// </summary>
    public SKRect Bounds { get; set; }

    /// <summary>
    /// Gets or sets whether this panel shares the X-axis with other panels.
    /// When true, the X-axis is synchronized across all panels.
    /// </summary>
    public bool ShareXAxis { get; set; } = true;

    /// <summary>
    /// Renders this panel to the specified context.
    /// </summary>
    /// <param name="canvas">The canvas to render to.</param>
    public void Render(SKCanvas canvas)
    {
        if (!IsVisible || Chart == null || Bounds.IsEmpty)
            return;

        // Save canvas state
        canvas.Save();

        // Translate to panel origin and clip to local bounds
        canvas.Translate(Bounds.Left, Bounds.Top);
        canvas.ClipRect(new SKRect(0, 0, Bounds.Width, Bounds.Height));

        // Render chart
        Chart.Render(canvas, Bounds.Width, Bounds.Height);

        // Restore canvas state
        canvas.Restore();
    }
}
