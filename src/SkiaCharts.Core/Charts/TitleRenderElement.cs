using SkiaCharts.Core.Legend;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Renders chart titles and subtitles.
/// </summary>
internal sealed class TitleRenderElement : ChartElement
{
    private readonly TitleManager _titleManager;
    private readonly SKRect _chartBounds;

    public TitleRenderElement(TitleManager titleManager, SKRect chartBounds)
    {
        _titleManager = titleManager;
        _chartBounds = chartBounds;
        Layer = RenderLayer.Annotations;
    }

    public override void Render(IRenderContext context)
    {
        _titleManager.Render(context.Canvas, _chartBounds);
    }
}
