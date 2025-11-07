using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Represents a multi-panel chart container that manages multiple chart panels
/// with synchronized X-axis and independent Y-axes.
/// </summary>
public class MultiPanelChart
{
    private readonly List<ChartPanel> _panels = new();
    private readonly ViewportManager _sharedViewport = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiPanelChart"/> class.
    /// </summary>
    public MultiPanelChart()
    {
        BackgroundColor = SKColors.White;
        PanelSpacing = 5f;
        Margin = new ChartMargin(10, 10, 10, 10);
    }

    /// <summary>
    /// Gets the collection of panels in this chart.
    /// </summary>
    public IReadOnlyList<ChartPanel> Panels => _panels.AsReadOnly();

    /// <summary>
    /// Gets or sets the background color of the entire chart.
    /// </summary>
    public SKColor BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the spacing between panels in pixels.
    /// </summary>
    public float PanelSpacing { get; set; }

    /// <summary>
    /// Gets or sets the outer margin of the chart.
    /// </summary>
    public ChartMargin Margin { get; set; }

    /// <summary>
    /// Gets or sets the title of the chart.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets whether to synchronize X-axis across all panels.
    /// </summary>
    public bool SynchronizeXAxis { get; set; } = true;

    /// <summary>
    /// Gets the shared viewport manager for synchronized X-axis.
    /// </summary>
    public ViewportManager SharedViewport => _sharedViewport;

    /// <summary>
    /// Adds a panel to the chart.
    /// </summary>
    /// <param name="panel">The panel to add.</param>
    public void AddPanel(ChartPanel panel)
    {
        _panels.Add(panel);
    }

    /// <summary>
    /// Removes a panel from the chart.
    /// </summary>
    /// <param name="panel">The panel to remove.</param>
    /// <returns>True if the panel was removed.</returns>
    public bool RemovePanel(ChartPanel panel)
    {
        return _panels.Remove(panel);
    }

    /// <summary>
    /// Removes a panel by ID.
    /// </summary>
    /// <param name="panelId">The ID of the panel to remove.</param>
    /// <returns>True if the panel was removed.</returns>
    public bool RemovePanelById(string panelId)
    {
        var panel = _panels.FirstOrDefault(p => p.Id == panelId);
        if (panel != null)
        {
            return _panels.Remove(panel);
        }
        return false;
    }

    /// <summary>
    /// Gets a panel by ID.
    /// </summary>
    /// <param name="panelId">The ID of the panel.</param>
    /// <returns>The panel, or null if not found.</returns>
    public ChartPanel? GetPanelById(string panelId)
    {
        return _panels.FirstOrDefault(p => p.Id == panelId);
    }

    /// <summary>
    /// Clears all panels from the chart.
    /// </summary>
    public void ClearPanels()
    {
        _panels.Clear();
    }

    /// <summary>
    /// Moves a panel to a new position.
    /// </summary>
    /// <param name="panelId">The ID of the panel to move.</param>
    /// <param name="newIndex">The new index position.</param>
    /// <returns>True if the panel was moved.</returns>
    public bool MovePanelTo(string panelId, int newIndex)
    {
        var panel = _panels.FirstOrDefault(p => p.Id == panelId);
        if (panel == null || newIndex < 0 || newIndex >= _panels.Count)
            return false;

        _panels.Remove(panel);
        _panels.Insert(newIndex, panel);
        return true;
    }

    /// <summary>
    /// Sets the height ratio for a panel.
    /// </summary>
    /// <param name="panelId">The ID of the panel.</param>
    /// <param name="heightRatio">The new height ratio.</param>
    /// <returns>True if the height was set.</returns>
    public bool SetPanelHeight(string panelId, double heightRatio)
    {
        var panel = _panels.FirstOrDefault(p => p.Id == panelId);
        if (panel == null)
            return false;

        panel.Height = Math.Max(0.1, heightRatio);
        return true;
    }

    /// <summary>
    /// Renders the entire multi-panel chart.
    /// </summary>
    /// <param name="canvas">The SkiaSharp canvas to render to.</param>
    /// <param name="width">The width of the canvas.</param>
    /// <param name="height">The height of the canvas.</param>
    public void Render(SKCanvas canvas, float width, float height)
    {
        var context = new RenderContext(canvas, width, height);

        // Clear background
        context.Clear(BackgroundColor);

        // Calculate layout
        var totalBounds = new SKRect(
            Margin.Left,
            Margin.Top,
            width - Margin.Right,
            height - Margin.Bottom
        );

        // Calculate panel layout
        CalculatePanelLayout(totalBounds);

        // Synchronize X-axis if enabled
        if (SynchronizeXAxis)
        {
            SynchronizeXAxisAcrossPanels();
        }

        // Render each panel
        foreach (var panel in _panels.Where(p => p.IsVisible))
        {
            panel.Render(canvas);
        }

        // Draw title if present
        if (!string.IsNullOrEmpty(Title))
        {
            DrawTitle(context, width);
        }
    }

    private void CalculatePanelLayout(SKRect availableBounds)
    {
        var visiblePanels = _panels.Where(p => p.IsVisible).ToList();
        if (visiblePanels.Count == 0)
            return;

        // Calculate total height ratio
        double totalHeightRatio = visiblePanels.Sum(p => p.Height);

        // Calculate available height (minus spacing)
        float availableHeight = availableBounds.Height - (PanelSpacing * (visiblePanels.Count - 1));

        // Assign bounds to each panel
        float currentY = availableBounds.Top;

        foreach (var panel in visiblePanels)
        {
            // Calculate panel height based on ratio
            float panelHeight = (float)(availableHeight * (panel.Height / totalHeightRatio));

            // Apply min/max constraints
            if (panelHeight < panel.MinHeight)
                panelHeight = panel.MinHeight;

            if (panel.MaxHeight > 0 && panelHeight > panel.MaxHeight)
                panelHeight = panel.MaxHeight;

            // Set panel bounds
            panel.Bounds = new SKRect(
                availableBounds.Left,
                currentY,
                availableBounds.Right,
                currentY + panelHeight
            );

            currentY += panelHeight + PanelSpacing;
        }
    }

    private void SynchronizeXAxisAcrossPanels()
    {
        // Find the combined X range across all panels
        double minX = double.MaxValue;
        double maxX = double.MinValue;

        foreach (var panel in _panels.Where(p => p.IsVisible && p.Chart != null))
        {
            var xRange = panel.Chart!.Series.XRange;
            if (xRange.Min < minX) minX = xRange.Min;
            if (xRange.Max > maxX) maxX = xRange.Max;
        }

        if (minX == double.MaxValue || maxX == double.MinValue)
            return;

        // Update shared viewport
        _sharedViewport.XDataRange = new DataRange(minX, maxX);

        // Apply to all panels that share X-axis
        foreach (var panel in _panels.Where(p => p.IsVisible && p.Chart != null && p.ShareXAxis))
        {
            if (panel.Chart!.XAxis.AutoScale)
            {
                var xRange = panel.Chart.XAxis.CalculateOptimalRange(new DataRange(minX, maxX));
                panel.Chart.XAxis.VisibleRange = xRange;
            }
        }
    }

    private void DrawTitle(IRenderContext context, float width)
    {
        using var paint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 18,
            IsAntialias = true
        };

        var textBounds = context.MeasureText(Title!, paint);
        float x = (width - textBounds.Width) / 2;
        float y = Margin.Top / 2 + textBounds.Height / 2;

        context.DrawText(Title!, x, y, paint);
    }

    /// <summary>
    /// Creates a preset layout with a single panel.
    /// </summary>
    /// <param name="chart">The chart to display.</param>
    /// <returns>A configured multi-panel chart.</returns>
    public static MultiPanelChart CreateSinglePanelLayout(ChartBase chart)
    {
        var multiPanel = new MultiPanelChart();
        multiPanel.AddPanel(new ChartPanel(chart) { Height = 1.0 });
        return multiPanel;
    }

    /// <summary>
    /// Creates a preset layout with two panels (e.g., price + volume).
    /// </summary>
    /// <param name="topChart">The chart for the top panel.</param>
    /// <param name="bottomChart">The chart for the bottom panel.</param>
    /// <param name="topRatio">Height ratio for top panel (default 3.0).</param>
    /// <param name="bottomRatio">Height ratio for bottom panel (default 1.0).</param>
    /// <returns>A configured multi-panel chart.</returns>
    public static MultiPanelChart CreateDualPanelLayout(
        ChartBase topChart,
        ChartBase bottomChart,
        double topRatio = 3.0,
        double bottomRatio = 1.0)
    {
        var multiPanel = new MultiPanelChart();
        multiPanel.AddPanel(new ChartPanel(topChart) { Height = topRatio, Title = "Main" });
        multiPanel.AddPanel(new ChartPanel(bottomChart) { Height = bottomRatio, Title = "Indicator" });
        return multiPanel;
    }

    /// <summary>
    /// Creates a preset layout with three panels (e.g., price + indicator + volume).
    /// </summary>
    /// <param name="topChart">The chart for the top panel.</param>
    /// <param name="middleChart">The chart for the middle panel.</param>
    /// <param name="bottomChart">The chart for the bottom panel.</param>
    /// <param name="topRatio">Height ratio for top panel (default 3.0).</param>
    /// <param name="middleRatio">Height ratio for middle panel (default 1.5).</param>
    /// <param name="bottomRatio">Height ratio for bottom panel (default 1.0).</param>
    /// <returns>A configured multi-panel chart.</returns>
    public static MultiPanelChart CreateTriplePanelLayout(
        ChartBase topChart,
        ChartBase middleChart,
        ChartBase bottomChart,
        double topRatio = 3.0,
        double middleRatio = 1.5,
        double bottomRatio = 1.0)
    {
        var multiPanel = new MultiPanelChart();
        multiPanel.AddPanel(new ChartPanel(topChart) { Height = topRatio, Title = "Main" });
        multiPanel.AddPanel(new ChartPanel(middleChart) { Height = middleRatio, Title = "Indicator 1" });
        multiPanel.AddPanel(new ChartPanel(bottomChart) { Height = bottomRatio, Title = "Indicator 2" });
        return multiPanel;
    }
}
