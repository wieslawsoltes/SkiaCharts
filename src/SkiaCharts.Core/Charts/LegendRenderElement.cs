using SkiaCharts.Core.Legend;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Renders the chart legend.
/// </summary>
internal sealed class LegendRenderElement : ChartElement
{
    private readonly LegendManager _legend;
    private readonly SKRect _chartBounds;

    public LegendRenderElement(LegendManager legend, SKRect chartBounds)
    {
        _legend = legend;
        _chartBounds = chartBounds;
        Layer = RenderLayer.Overlay;
    }

    public override void Render(IRenderContext context)
    {
        _legend.Render(context.Canvas, _chartBounds);
    }
}
