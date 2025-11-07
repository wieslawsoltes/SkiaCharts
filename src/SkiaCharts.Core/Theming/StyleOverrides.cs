using SkiaSharp;

namespace SkiaCharts.Core.Theming;

/// <summary>
/// Allows per-element style overrides (CSS-like specific styling).
/// </summary>
public class StyleOverrides
{
    private readonly Dictionary<string, object?> _overrides = new();

    /// <summary>
    /// Sets a style override for a specific property.
    /// </summary>
    public void Set(string propertyName, object? value)
    {
        _overrides[propertyName] = value;
    }

    /// <summary>
    /// Gets a style override value if it exists.
    /// </summary>
    public T? Get<T>(string propertyName, T? defaultValue = default)
    {
        if (_overrides.TryGetValue(propertyName, out var value) && value is T typedValue)
            return typedValue;
        return defaultValue;
    }

    /// <summary>
    /// Checks if a property has been overridden.
    /// </summary>
    public bool HasOverride(string propertyName) => _overrides.ContainsKey(propertyName);

    /// <summary>
    /// Removes a style override.
    /// </summary>
    public void Clear(string propertyName) => _overrides.Remove(propertyName);

    /// <summary>
    /// Removes all style overrides.
    /// </summary>
    public void ClearAll() => _overrides.Clear();

    /// <summary>
    /// Gets all override property names.
    /// </summary>
    public IEnumerable<string> GetOverrideKeys() => _overrides.Keys;

    /// <summary>
    /// Creates a deep copy of these overrides.
    /// </summary>
    public StyleOverrides Clone()
    {
        var clone = new StyleOverrides();
        foreach (var kvp in _overrides)
        {
            clone._overrides[kvp.Key] = kvp.Value;
        }
        return clone;
    }
}

/// <summary>
/// Interface for elements that support style overrides.
/// </summary>
public interface IStyleable
{
    /// <summary>
    /// Gets the style overrides for this element.
    /// </summary>
    StyleOverrides StyleOverrides { get; }

    /// <summary>
    /// Gets or sets the CSS-like class names for this element.
    /// </summary>
    List<string> StyleClasses { get; }

    /// <summary>
    /// Gets or sets the unique style identifier for this element.
    /// </summary>
    string? StyleId { get; set; }
}

/// <summary>
/// Style selector for applying styles based on class/id (CSS-like).
/// </summary>
public class StyleSelector
{
    /// <summary>
    /// Selector type (Id, Class, Element).
    /// </summary>
    public StyleSelectorType Type { get; set; }

    /// <summary>
    /// Selector value (e.g., "my-series", "highlight", "LineSeries").
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Style overrides to apply when selector matches.
    /// </summary>
    public StyleOverrides Overrides { get; set; } = new();

    /// <summary>
    /// Creates a new style selector.
    /// </summary>
    public StyleSelector(StyleSelectorType type, string value)
    {
        Type = type;
        Value = value;
    }

    /// <summary>
    /// Checks if this selector matches the given element.
    /// </summary>
    public bool Matches(IStyleable element)
    {
        return Type switch
        {
            StyleSelectorType.Id => element.StyleId == Value,
            StyleSelectorType.Class => element.StyleClasses.Contains(Value),
            StyleSelectorType.Element => element.GetType().Name == Value,
            _ => false
        };
    }

    /// <summary>
    /// Creates an ID selector (#my-id).
    /// </summary>
    public static StyleSelector Id(string id) => new(StyleSelectorType.Id, id);

    /// <summary>
    /// Creates a class selector (.my-class).
    /// </summary>
    public static StyleSelector Class(string className) => new(StyleSelectorType.Class, className);

    /// <summary>
    /// Creates an element selector (LineSeries).
    /// </summary>
    public static StyleSelector Element(string elementType) => new(StyleSelectorType.Element, elementType);
}

/// <summary>
/// Style selector type.
/// </summary>
public enum StyleSelectorType
{
    /// <summary>Select by unique ID (#id).</summary>
    Id,
    /// <summary>Select by class name (.class).</summary>
    Class,
    /// <summary>Select by element type (LineSeries).</summary>
    Element
}

/// <summary>
/// Manages style selectors and applies them to elements (CSS-like cascade).
/// </summary>
public class StyleSheet
{
    private readonly List<StyleSelector> _selectors = new();

    /// <summary>
    /// Adds a style selector to the stylesheet.
    /// </summary>
    public void AddSelector(StyleSelector selector)
    {
        _selectors.Add(selector);
    }

    /// <summary>
    /// Removes a style selector from the stylesheet.
    /// </summary>
    public void RemoveSelector(StyleSelector selector)
    {
        _selectors.Remove(selector);
    }

    /// <summary>
    /// Clears all style selectors.
    /// </summary>
    public void Clear()
    {
        _selectors.Clear();
    }

    /// <summary>
    /// Gets all matching selectors for an element.
    /// </summary>
    public IEnumerable<StyleSelector> GetMatchingSelectors(IStyleable element)
    {
        return _selectors.Where(s => s.Matches(element));
    }

    /// <summary>
    /// Applies all matching styles to an element (in order of specificity).
    /// </summary>
    public void ApplyStyles(IStyleable element)
    {
        // Apply in order: Element -> Class -> Id (increasing specificity)
        // Enum is ordered Id=0, Class=1, Element=2, so we reverse it
        var selectors = GetMatchingSelectors(element)
            .OrderByDescending(s => s.Type);

        foreach (var selector in selectors)
        {
            foreach (var key in selector.Overrides.GetOverrideKeys())
            {
                var value = selector.Overrides.Get<object>(key);
                element.StyleOverrides.Set(key, value);
            }
        }
    }

    /// <summary>
    /// Creates a deep copy of this stylesheet.
    /// </summary>
    public StyleSheet Clone()
    {
        var clone = new StyleSheet();
        foreach (var selector in _selectors)
        {
            clone.AddSelector(new StyleSelector(selector.Type, selector.Value)
            {
                Overrides = selector.Overrides.Clone()
            });
        }
        return clone;
    }
}

/// <summary>
/// Helper methods for applying styles with type safety.
/// </summary>
public static class StyleExtensions
{
    /// <summary>
    /// Sets a color override with type safety.
    /// </summary>
    public static void SetColor(this StyleOverrides overrides, string propertyName, SKColor color)
    {
        overrides.Set(propertyName, color);
    }

    /// <summary>
    /// Gets a color override with type safety.
    /// </summary>
    public static SKColor GetColor(this StyleOverrides overrides, string propertyName, SKColor defaultValue)
    {
        return overrides.Get(propertyName, defaultValue);
    }

    /// <summary>
    /// Sets a float override with type safety.
    /// </summary>
    public static void SetFloat(this StyleOverrides overrides, string propertyName, float value)
    {
        overrides.Set(propertyName, value);
    }

    /// <summary>
    /// Gets a float override with type safety.
    /// </summary>
    public static float GetFloat(this StyleOverrides overrides, string propertyName, float defaultValue)
    {
        return overrides.Get(propertyName, defaultValue);
    }

    /// <summary>
    /// Sets a bool override with type safety.
    /// </summary>
    public static void SetBool(this StyleOverrides overrides, string propertyName, bool value)
    {
        overrides.Set(propertyName, value);
    }

    /// <summary>
    /// Gets a bool override with type safety.
    /// </summary>
    public static bool GetBool(this StyleOverrides overrides, string propertyName, bool defaultValue)
    {
        return overrides.Get(propertyName, defaultValue);
    }
}
