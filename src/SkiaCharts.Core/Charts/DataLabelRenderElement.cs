using SkiaCharts.Core.Legend;
using SkiaCharts.Core.Rendering;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Renders data labels.
/// </summary>
internal sealed class DataLabelRenderElement : ChartElement
{
    private readonly DataLabelManager _dataLabels;

    public DataLabelRenderElement(DataLabelManager dataLabels)
    {
        _dataLabels = dataLabels;
        Layer = RenderLayer.Annotations;
    }

    public override void Render(IRenderContext context)
    {
        _dataLabels.Render(context.Canvas);
    }
}
