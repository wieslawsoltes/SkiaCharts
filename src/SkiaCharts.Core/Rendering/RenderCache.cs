using SkiaSharp;
using System;

namespace SkiaCharts.Core.Rendering;

/// <summary>
/// Caches rendered layers into bitmaps for faster redraws.
/// </summary>
public sealed class RenderCache : IDisposable
{
    private readonly Dictionary<RenderLayer, LayerCache> _layers = new();

    /// <summary>
    /// Gets or sets whether caching is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets the set of layers that are eligible for caching.
    /// </summary>
    public HashSet<RenderLayer> CachedLayers { get; } = new()
    {
        RenderLayer.Background,
        RenderLayer.Grid,
        RenderLayer.Data,
        RenderLayer.Annotations
    };

    /// <summary>
    /// Invalidates a cached layer.
    /// </summary>
    /// <param name="layer">The render layer to invalidate.</param>
    public void Invalidate(RenderLayer layer)
    {
        if (_layers.TryGetValue(layer, out var cache))
        {
            cache.HasContent = false;
        }
    }

    /// <summary>
    /// Invalidates all cached layers.
    /// </summary>
    public void InvalidateAll()
    {
        foreach (var cache in _layers.Values)
        {
            cache.HasContent = false;
        }
    }

    /// <summary>
    /// Renders a layer using cached content when possible.
    /// </summary>
    /// <param name="targetContext">The target render context.</param>
    /// <param name="layer">The render layer.</param>
    /// <param name="renderAction">Action to render the layer when cache is dirty.</param>
    /// <param name="dirtyRegion">Optional dirty region for partial redraw.</param>
    /// <param name="fullDirty">Indicates a full redraw is required.</param>
    public void RenderLayer(
        IRenderContext targetContext,
        RenderLayer layer,
        Action<IRenderContext> renderAction,
        SKRect? dirtyRegion = null,
        bool fullDirty = false)
    {
        if (!IsEnabled || !CachedLayers.Contains(layer))
        {
            renderAction(targetContext);
            return;
        }

        var width = Math.Max(1, (int)Math.Ceiling(targetContext.Width));
        var height = Math.Max(1, (int)Math.Ceiling(targetContext.Height));

        var cache = GetLayerCache(layer, width, height, out var recreated);
        if (recreated)
        {
            fullDirty = true;
        }

        if (!cache.HasContent)
        {
            fullDirty = true;
        }

        if (fullDirty)
        {
            RenderFull(cache, width, height, renderAction);
        }
        else if (dirtyRegion.HasValue)
        {
            RenderPartial(cache, width, height, dirtyRegion.Value, renderAction);
        }

        DrawCachedLayer(targetContext, cache);
        cache.HasContent = true;
    }

    /// <summary>
    /// Clears and disposes all cached resources.
    /// </summary>
    public void Clear()
    {
        foreach (var cache in _layers.Values)
        {
            cache.Dispose();
        }

        _layers.Clear();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Clear();
    }

    private LayerCache GetLayerCache(RenderLayer layer, int width, int height, out bool recreated)
    {
        if (_layers.TryGetValue(layer, out var cache))
        {
            if (cache.Width == width && cache.Height == height)
            {
                recreated = false;
                return cache;
            }

            cache.Dispose();
        }

        var info = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
        var bitmap = new SKBitmap(info);
        var canvas = new SKCanvas(bitmap);
        cache = new LayerCache(bitmap, canvas, width, height);
        _layers[layer] = cache;
        recreated = true;
        return cache;
    }

    private static void RenderFull(LayerCache cache, int width, int height, Action<IRenderContext> renderAction)
    {
        cache.Canvas.Clear(SKColors.Transparent);
        var context = new RenderContext(cache.Canvas, width, height);
        renderAction(context);
    }

    private static void RenderPartial(LayerCache cache, int width, int height, SKRect region, Action<IRenderContext> renderAction)
    {
        var bounds = new SKRect(0, 0, width, height);
        var clipped = Intersect(bounds, region);
        if (clipped.IsEmpty)
        {
            return;
        }

        using var clearPaint = new SKPaint { BlendMode = SKBlendMode.Clear };
        cache.Canvas.DrawRect(clipped, clearPaint);

        var save = cache.Canvas.Save();
        cache.Canvas.ClipRect(clipped);

        var context = new RenderContext(cache.Canvas, width, height);
        renderAction(context);

        cache.Canvas.RestoreToCount(save);
    }

    private static void DrawCachedLayer(IRenderContext targetContext, LayerCache cache)
    {
        targetContext.Canvas.DrawBitmap(cache.Bitmap, 0, 0);
    }

    private static SKRect Intersect(SKRect first, SKRect second)
    {
        var left = Math.Max(first.Left, second.Left);
        var top = Math.Max(first.Top, second.Top);
        var right = Math.Min(first.Right, second.Right);
        var bottom = Math.Min(first.Bottom, second.Bottom);

        if (right <= left || bottom <= top)
        {
            return SKRect.Empty;
        }

        return new SKRect(left, top, right, bottom);
    }

    private sealed class LayerCache : IDisposable
    {
        public LayerCache(SKBitmap bitmap, SKCanvas canvas, int width, int height)
        {
            Bitmap = bitmap;
            Canvas = canvas;
            Width = width;
            Height = height;
            HasContent = false;
        }

        public SKBitmap Bitmap { get; }
        public SKCanvas Canvas { get; }
        public int Width { get; }
        public int Height { get; }
        public bool HasContent { get; set; }

        public void Dispose()
        {
            Canvas.Dispose();
            Bitmap.Dispose();
        }
    }
}
