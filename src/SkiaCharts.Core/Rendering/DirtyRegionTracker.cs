using SkiaSharp;
using System;

namespace SkiaCharts.Core.Rendering;

/// <summary>
/// Tracks dirty regions per render layer to minimize redraw work.
/// </summary>
public sealed class DirtyRegionTracker
{
    private readonly Dictionary<RenderLayer, DirtyRegion> _dirtyRegions = new();

    /// <summary>
    /// Gets a value indicating whether any dirty regions are tracked.
    /// </summary>
    public bool HasDirtyRegions => _dirtyRegions.Count > 0;

    /// <summary>
    /// Marks an entire layer as dirty.
    /// </summary>
    /// <param name="layer">The render layer.</param>
    public void MarkDirty(RenderLayer layer)
    {
        _dirtyRegions[layer] = new DirtyRegion { IsFull = true };
    }

    /// <summary>
    /// Marks a region of a layer as dirty.
    /// </summary>
    /// <param name="layer">The render layer.</param>
    /// <param name="region">The dirty region in screen coordinates.</param>
    public void MarkDirty(RenderLayer layer, SKRect region)
    {
        if (region.IsEmpty)
        {
            MarkDirty(layer);
            return;
        }

        if (_dirtyRegions.TryGetValue(layer, out var existing))
        {
            if (existing.IsFull)
            {
                return;
            }

            existing.Bounds = Union(existing.Bounds, region);
            _dirtyRegions[layer] = existing;
        }
        else
        {
            _dirtyRegions[layer] = new DirtyRegion { Bounds = region };
        }
    }

    /// <summary>
    /// Marks all layers as dirty.
    /// </summary>
    public void MarkAllDirty()
    {
        foreach (RenderLayer layer in System.Enum.GetValues(typeof(RenderLayer)))
        {
            MarkDirty(layer);
        }
    }

    /// <summary>
    /// Attempts to get the dirty region for a layer.
    /// </summary>
    /// <param name="layer">The render layer.</param>
    /// <param name="region">The dirty region bounds (if any).</param>
    /// <param name="isFull">True if the entire layer is dirty.</param>
    /// <returns>True if a dirty region exists for the layer.</returns>
    public bool TryGetDirtyRegion(RenderLayer layer, out SKRect region, out bool isFull)
    {
        if (_dirtyRegions.TryGetValue(layer, out var dirty))
        {
            region = dirty.Bounds;
            isFull = dirty.IsFull;
            return true;
        }

        region = SKRect.Empty;
        isFull = false;
        return false;
    }

    /// <summary>
    /// Clears all tracked dirty regions.
    /// </summary>
    public void Clear()
    {
        _dirtyRegions.Clear();
    }

    /// <summary>
    /// Clears dirty tracking for a specific layer.
    /// </summary>
    /// <param name="layer">The render layer.</param>
    public void Clear(RenderLayer layer)
    {
        _dirtyRegions.Remove(layer);
    }

    private static SKRect Union(SKRect first, SKRect second)
    {
        if (first.IsEmpty)
        {
            return second;
        }

        if (second.IsEmpty)
        {
            return first;
        }

        var left = Math.Min(first.Left, second.Left);
        var top = Math.Min(first.Top, second.Top);
        var right = Math.Max(first.Right, second.Right);
        var bottom = Math.Max(first.Bottom, second.Bottom);

        return new SKRect(left, top, right, bottom);
    }

    private struct DirtyRegion
    {
        public bool IsFull;
        public SKRect Bounds;
    }
}
