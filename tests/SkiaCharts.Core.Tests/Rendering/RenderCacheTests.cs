using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Tests.Rendering;

public class RenderCacheTests
{
    [Fact]
    public void RenderCache_ShouldReuseLayerWhenNotDirty()
    {
        using var targetBitmap = new SKBitmap(20, 20);
        using var canvas = new SKCanvas(targetBitmap);
        var context = new RenderContext(canvas, 20, 20);
        using var cache = new RenderCache();

        int renderCount = 0;

        cache.RenderLayer(
            context,
            RenderLayer.Data,
            ctx =>
            {
                renderCount++;
                using var paint = new SKPaint { Color = SKColors.Red };
                ctx.DrawRect(new SKRect(0, 0, 10, 10), paint);
            });

        cache.RenderLayer(
            context,
            RenderLayer.Data,
            _ => renderCount++);

        Assert.Equal(1, renderCount);
    }
}
