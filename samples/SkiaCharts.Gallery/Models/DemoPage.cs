using System;

namespace SkiaCharts.Gallery.Models;

/// <summary>
/// Represents a single demo page.
/// </summary>
public class DemoPage
{
    /// <summary>
    /// Gets or sets the demo identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the demo title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the demo description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category ID this demo belongs to.
    /// </summary>
    public string CategoryId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the view model type for this demo.
    /// </summary>
    public Type? ViewModelType { get; set; }

    /// <summary>
    /// Gets or sets the view type for this demo.
    /// </summary>
    public Type? ViewType { get; set; }

    /// <summary>
    /// Gets or sets the tags for searching.
    /// </summary>
    public string[] Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets the difficulty level (1-5).
    /// </summary>
    public int Difficulty { get; set; } = 1;
}
