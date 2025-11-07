using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Drawing;

/// <summary>
/// Base class for all drawing tools.
/// </summary>
public abstract class DrawingToolBase : IDrawingTool
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawingToolBase"/> class.
    /// </summary>
    protected DrawingToolBase()
    {
        Id = Guid.NewGuid().ToString();
        IsVisible = true;
        IsSelected = false;
        Color = SKColors.Blue;
        LineWidth = 2f;
    }

    /// <inheritdoc/>
    public string Id { get; set; }

    /// <inheritdoc/>
    public bool IsVisible { get; set; }

    /// <inheritdoc/>
    public bool IsSelected { get; set; }

    /// <inheritdoc/>
    public SKColor Color { get; set; }

    /// <inheritdoc/>
    public float LineWidth { get; set; }

    /// <inheritdoc/>
    public float[]? DashPattern { get; set; }

    /// <inheritdoc/>
    public abstract void Render(IRenderContext context);

    /// <inheritdoc/>
    public abstract bool HitTest(float x, float y, float tolerance = 5f);

    /// <inheritdoc/>
    public virtual Dictionary<string, object> Serialize()
    {
        return new Dictionary<string, object>
        {
            ["Id"] = Id,
            ["Type"] = GetType().Name,
            ["IsVisible"] = IsVisible,
            ["Color"] = Color.ToString(),
            ["LineWidth"] = LineWidth,
            ["DashPattern"] = DashPattern ?? Array.Empty<float>()
        };
    }

    /// <inheritdoc/>
    public virtual void Deserialize(Dictionary<string, object> data)
    {
        if (data.TryGetValue("Id", out var id))
            Id = id.ToString() ?? Guid.NewGuid().ToString();

        if (data.TryGetValue("IsVisible", out var isVisible))
            IsVisible = Convert.ToBoolean(isVisible);

        if (data.TryGetValue("Color", out var color))
            Color = SKColor.Parse(color.ToString() ?? "#0000FF");

        if (data.TryGetValue("LineWidth", out var lineWidth))
            LineWidth = Convert.ToSingle(lineWidth);

        if (data.TryGetValue("DashPattern", out var dashPattern) && dashPattern is float[] pattern && pattern.Length > 0)
            DashPattern = pattern;
    }

    /// <summary>
    /// Helper method to calculate distance from a point to a line segment.
    /// </summary>
    protected float DistanceToLineSegment(float px, float py, float x1, float y1, float x2, float y2)
    {
        float dx = x2 - x1;
        float dy = y2 - y1;
        float lengthSquared = dx * dx + dy * dy;

        if (lengthSquared == 0)
            return (float)Math.Sqrt((px - x1) * (px - x1) + (py - y1) * (py - y1));

        float t = Math.Max(0, Math.Min(1, ((px - x1) * dx + (py - y1) * dy) / lengthSquared));
        float projX = x1 + t * dx;
        float projY = y1 + t * dy;

        return (float)Math.Sqrt((px - projX) * (px - projX) + (py - projY) * (py - projY));
    }
}
