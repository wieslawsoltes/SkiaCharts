using SkiaCharts.Core.Rendering;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Base class for all chart elements that can be rendered.
/// </summary>
public abstract class ChartElement : IRenderable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartElement"/> class.
    /// </summary>
    protected ChartElement()
    {
        IsVisible = true;
        Layer = RenderLayer.Data;
    }

    /// <inheritdoc/>
    public virtual bool IsVisible { get; set; }

    /// <inheritdoc/>
    public virtual RenderLayer Layer { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this element is enabled for hit testing.
    /// </summary>
    public bool IsHitTestVisible { get; set; } = true;

    /// <inheritdoc/>
    public abstract void Render(IRenderContext context);

    /// <summary>
    /// Performs hit testing to determine if a point intersects this element.
    /// </summary>
    /// <param name="x">The X coordinate in screen space.</param>
    /// <param name="y">The Y coordinate in screen space.</param>
    /// <returns>True if the point hits this element; otherwise, false.</returns>
    public virtual bool HitTest(float x, float y)
    {
        return false;
    }
}
