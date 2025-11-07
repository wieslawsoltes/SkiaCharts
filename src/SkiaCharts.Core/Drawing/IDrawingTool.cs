using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Drawing;

/// <summary>
/// Base interface for all drawing tools.
/// </summary>
public interface IDrawingTool
{
    /// <summary>
    /// Gets or sets the unique identifier for this drawing.
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// Gets or sets whether this drawing is visible.
    /// </summary>
    bool IsVisible { get; set; }

    /// <summary>
    /// Gets or sets whether this drawing is selected.
    /// </summary>
    bool IsSelected { get; set; }

    /// <summary>
    /// Gets or sets the color of the drawing.
    /// </summary>
    SKColor Color { get; set; }

    /// <summary>
    /// Gets or sets the line width.
    /// </summary>
    float LineWidth { get; set; }

    /// <summary>
    /// Gets or sets the dash pattern (null for solid line).
    /// </summary>
    float[]? DashPattern { get; set; }

    /// <summary>
    /// Renders the drawing tool.
    /// </summary>
    /// <param name="context">The render context.</param>
    void Render(IRenderContext context);

    /// <summary>
    /// Checks if a point hits this drawing (for selection).
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="tolerance">Hit test tolerance in pixels.</param>
    /// <returns>True if the point hits the drawing.</returns>
    bool HitTest(float x, float y, float tolerance = 5f);

    /// <summary>
    /// Serializes the drawing to a dictionary.
    /// </summary>
    Dictionary<string, object> Serialize();

    /// <summary>
    /// Deserializes the drawing from a dictionary.
    /// </summary>
    void Deserialize(Dictionary<string, object> data);
}
