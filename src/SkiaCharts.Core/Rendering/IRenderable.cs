namespace SkiaCharts.Core.Rendering;

/// <summary>
/// Represents an object that can be rendered on a chart.
/// </summary>
public interface IRenderable
{
    /// <summary>
    /// Gets the render layer this element should be drawn on.
    /// </summary>
    RenderLayer Layer { get; }

    /// <summary>
    /// Gets a value indicating whether this element is visible.
    /// </summary>
    bool IsVisible { get; }

    /// <summary>
    /// Renders the element using the specified context.
    /// </summary>
    /// <param name="context">The render context.</param>
    void Render(IRenderContext context);
}
