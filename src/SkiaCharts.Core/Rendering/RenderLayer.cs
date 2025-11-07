namespace SkiaCharts.Core.Rendering;

/// <summary>
/// Defines the rendering layers for chart elements.
/// Elements are rendered in order from lowest to highest layer.
/// </summary>
public enum RenderLayer
{
    /// <summary>
    /// Background layer (drawn first).
    /// </summary>
    Background = 0,

    /// <summary>
    /// Grid layer (axis lines and grid).
    /// </summary>
    Grid = 10,

    /// <summary>
    /// Data layer (chart data series).
    /// </summary>
    Data = 20,

    /// <summary>
    /// Annotation layer (labels, markers).
    /// </summary>
    Annotations = 30,

    /// <summary>
    /// Overlay layer (tooltips, crosshairs, drawn last).
    /// </summary>
    Overlay = 40
}
