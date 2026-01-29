using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Tests.Rendering;

public class DirtyRegionTrackerTests
{
    [Fact]
    public void DirtyRegionTracker_ShouldUnionRegions()
    {
        var tracker = new DirtyRegionTracker();

        tracker.MarkDirty(RenderLayer.Data, new SKRect(0, 0, 10, 10));
        tracker.MarkDirty(RenderLayer.Data, new SKRect(5, 5, 20, 20));

        Assert.True(tracker.TryGetDirtyRegion(RenderLayer.Data, out var region, out var isFull));
        Assert.False(isFull);
        Assert.Equal(0, region.Left, 3);
        Assert.Equal(0, region.Top, 3);
        Assert.Equal(20, region.Right, 3);
        Assert.Equal(20, region.Bottom, 3);
    }

    [Fact]
    public void DirtyRegionTracker_ShouldMarkFullLayer()
    {
        var tracker = new DirtyRegionTracker();

        tracker.MarkDirty(RenderLayer.Grid);

        Assert.True(tracker.TryGetDirtyRegion(RenderLayer.Grid, out _, out var isFull));
        Assert.True(isFull);
    }
}
