using System.Collections.Generic;

namespace SkiaCharts.Gallery.Models;

/// <summary>
/// Represents a category of demo pages.
/// </summary>
public class DemoCategory
{
    /// <summary>
    /// Gets or sets the category identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category icon (Material Design Icons name).
    /// </summary>
    public string Icon { get; set; } = "📊";

    /// <summary>
    /// Gets or sets the list of demos in this category.
    /// </summary>
    public List<DemoPage> Demos { get; set; } = new();
}
