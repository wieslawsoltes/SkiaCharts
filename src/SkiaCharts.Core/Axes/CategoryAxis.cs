using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;

namespace SkiaCharts.Core.Axes;

/// <summary>
/// Represents a categorical axis with discrete, evenly-spaced categories.
/// Used for bar charts, column charts, and other categorical visualizations.
/// Internally maps category indices (0, 1, 2, ...) to double values.
/// </summary>
public class CategoryAxis : IAxis
{
    private readonly List<string> _categories;
    private DataRange _visibleRange;

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryAxis"/> class.
    /// </summary>
    public CategoryAxis()
    {
        _categories = new List<string>();
        _visibleRange = new DataRange(0, 1);
        Position = AxisPosition.Bottom;
        AutoScale = true;
        ShowGridLines = true;
        ShowLabels = true;
        IsVisible = true;
        Layer = RenderLayer.Grid;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryAxis"/> class with categories.
    /// </summary>
    /// <param name="categories">The initial categories.</param>
    public CategoryAxis(IEnumerable<string> categories) : this()
    {
        _categories.AddRange(categories);
        UpdateVisibleRange();
    }

    /// <inheritdoc/>
    public string? Title { get; set; }

    /// <inheritdoc/>
    public AxisPosition Position { get; set; }

    /// <inheritdoc/>
    public DataRange VisibleRange
    {
        get => _visibleRange;
        set => _visibleRange = value;
    }

    /// <inheritdoc/>
    public bool AutoScale { get; set; }

    /// <inheritdoc/>
    public bool ShowGridLines { get; set; }

    /// <inheritdoc/>
    public bool ShowLabels { get; set; }

    /// <inheritdoc/>
    public double? MinValue { get; set; }

    /// <inheritdoc/>
    public double? MaxValue { get; set; }

    /// <inheritdoc/>
    public bool IsVisible { get; set; }

    /// <inheritdoc/>
    public RenderLayer Layer { get; }

    /// <summary>
    /// Gets the number of categories.
    /// </summary>
    public int CategoryCount => _categories.Count;

    /// <summary>
    /// Gets the read-only list of categories.
    /// </summary>
    public IReadOnlyList<string> Categories => _categories.AsReadOnly();

    /// <summary>
    /// Gets or sets the maximum number of labels to show (to prevent overcrowding).
    /// If there are more categories, labels will be skipped.
    /// </summary>
    public int MaxLabelsToShow { get; set; } = 50;

    /// <summary>
    /// Adds a category to the axis.
    /// </summary>
    /// <param name="category">The category label.</param>
    public void AddCategory(string category)
    {
        _categories.Add(category);
        UpdateVisibleRange();
    }

    /// <summary>
    /// Adds multiple categories to the axis.
    /// </summary>
    /// <param name="categories">The categories to add.</param>
    public void AddCategories(IEnumerable<string> categories)
    {
        _categories.AddRange(categories);
        UpdateVisibleRange();
    }

    /// <summary>
    /// Clears all categories.
    /// </summary>
    public void ClearCategories()
    {
        _categories.Clear();
        UpdateVisibleRange();
    }

    /// <summary>
    /// Gets the category at the specified index.
    /// </summary>
    /// <param name="index">The zero-based category index.</param>
    /// <returns>The category label.</returns>
    public string GetCategory(int index)
    {
        if (index < 0 || index >= _categories.Count)
        {
            return string.Empty;
        }
        return _categories[index];
    }

    /// <summary>
    /// Gets the index of a category by label.
    /// </summary>
    /// <param name="category">The category label.</param>
    /// <returns>The index, or -1 if not found.</returns>
    public int GetCategoryIndex(string category)
    {
        return _categories.IndexOf(category);
    }

    /// <inheritdoc/>
    public IReadOnlyList<TickInfo> GenerateTicks()
    {
        var ticks = new List<TickInfo>();

        if (_categories.Count == 0)
        {
            return ticks;
        }

        // Calculate how many labels to skip to avoid overcrowding
        int skip = CalculateLabelSkip();

        // Generate a tick for each category (or skip some if there are too many)
        for (int i = 0; i < _categories.Count; i++)
        {
            var value = (double)i;

            if (value >= _visibleRange.Min && value <= _visibleRange.Max)
            {
                bool isMajor = (i % skip == 0);
                ticks.Add(new TickInfo(value, _categories[i], isMajor));
            }
        }

        return ticks;
    }

    /// <inheritdoc/>
    public string FormatValue(double value)
    {
        var index = (int)Math.Round(value);

        if (index < 0 || index >= _categories.Count)
        {
            return string.Empty;
        }

        return _categories[index];
    }

    /// <inheritdoc/>
    public DataRange CalculateOptimalRange(DataRange dataRange)
    {
        if (_categories.Count == 0)
        {
            return new DataRange(0, 1);
        }

        // For category axes, range is always from -0.5 to (count - 0.5)
        // This centers the bars/columns on integer positions
        return new DataRange(-0.5, _categories.Count - 0.5);
    }

    /// <inheritdoc/>
    public void Render(IRenderContext context)
    {
        // Basic rendering implementation will be enhanced later
        // For now, this is a placeholder
    }

    /// <summary>
    /// Updates the visible range based on the current categories.
    /// </summary>
    private void UpdateVisibleRange()
    {
        if (_categories.Count == 0)
        {
            _visibleRange = new DataRange(0, 1);
        }
        else
        {
            _visibleRange = new DataRange(-0.5, _categories.Count - 0.5);
        }
    }

    /// <summary>
    /// Calculates how many labels to skip to avoid overcrowding.
    /// </summary>
    /// <returns>The skip interval (1 = show all, 2 = show every other, etc.).</returns>
    private int CalculateLabelSkip()
    {
        if (_categories.Count <= MaxLabelsToShow)
        {
            return 1; // Show all labels
        }

        // Calculate skip factor to stay under MaxLabelsToShow
        int skip = (int)Math.Ceiling((double)_categories.Count / MaxLabelsToShow);
        return skip;
    }
}
