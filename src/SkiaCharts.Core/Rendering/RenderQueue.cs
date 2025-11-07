namespace SkiaCharts.Core.Rendering;

/// <summary>
/// Manages a queue of renderable elements organized by layers.
/// Ensures elements are rendered in the correct order.
/// </summary>
public class RenderQueue
{
    private readonly List<IRenderable> _elements = new();
    private bool _isSorted;

    /// <summary>
    /// Adds a renderable element to the queue.
    /// </summary>
    /// <param name="element">The element to add.</param>
    public void Add(IRenderable element)
    {
        _elements.Add(element);
        _isSorted = false;
    }

    /// <summary>
    /// Adds multiple renderable elements to the queue.
    /// </summary>
    /// <param name="elements">The elements to add.</param>
    public void AddRange(IEnumerable<IRenderable> elements)
    {
        _elements.AddRange(elements);
        _isSorted = false;
    }

    /// <summary>
    /// Removes an element from the queue.
    /// </summary>
    /// <param name="element">The element to remove.</param>
    /// <returns>True if the element was removed; otherwise, false.</returns>
    public bool Remove(IRenderable element)
    {
        return _elements.Remove(element);
    }

    /// <summary>
    /// Clears all elements from the queue.
    /// </summary>
    public void Clear()
    {
        _elements.Clear();
        _isSorted = true;
    }

    /// <summary>
    /// Renders all visible elements in layer order.
    /// </summary>
    /// <param name="context">The render context.</param>
    public void RenderAll(IRenderContext context)
    {
        EnsureSorted();

        foreach (var element in _elements)
        {
            if (element.IsVisible)
            {
                element.Render(context);
            }
        }
    }

    /// <summary>
    /// Renders elements in a specific layer.
    /// </summary>
    /// <param name="context">The render context.</param>
    /// <param name="layer">The layer to render.</param>
    public void RenderLayer(IRenderContext context, RenderLayer layer)
    {
        EnsureSorted();

        foreach (var element in _elements)
        {
            if (element.IsVisible && element.Layer == layer)
            {
                element.Render(context);
            }
        }
    }

    /// <summary>
    /// Gets the count of elements in the queue.
    /// </summary>
    public int Count => _elements.Count;

    private void EnsureSorted()
    {
        if (!_isSorted)
        {
            _elements.Sort((a, b) => a.Layer.CompareTo(b.Layer));
            _isSorted = true;
        }
    }
}
