using System.Text.Json;

namespace SkiaCharts.Core.Drawing;

/// <summary>
/// Manages a collection of drawing tools with persistence support.
/// </summary>
public class DrawingManager
{
    private readonly List<IDrawingTool> _drawings = new();

    /// <summary>
    /// Gets the collection of drawings.
    /// </summary>
    public IReadOnlyList<IDrawingTool> Drawings => _drawings.AsReadOnly();

    /// <summary>
    /// Adds a drawing to the collection.
    /// </summary>
    /// <param name="drawing">The drawing to add.</param>
    public void AddDrawing(IDrawingTool drawing)
    {
        _drawings.Add(drawing);
    }

    /// <summary>
    /// Removes a drawing from the collection.
    /// </summary>
    /// <param name="drawing">The drawing to remove.</param>
    /// <returns>True if the drawing was removed.</returns>
    public bool RemoveDrawing(IDrawingTool drawing)
    {
        return _drawings.Remove(drawing);
    }

    /// <summary>
    /// Removes a drawing by ID.
    /// </summary>
    /// <param name="id">The ID of the drawing to remove.</param>
    /// <returns>True if the drawing was removed.</returns>
    public bool RemoveDrawingById(string id)
    {
        var drawing = _drawings.FirstOrDefault(d => d.Id == id);
        if (drawing != null)
        {
            return _drawings.Remove(drawing);
        }
        return false;
    }

    /// <summary>
    /// Clears all drawings.
    /// </summary>
    public void Clear()
    {
        _drawings.Clear();
    }

    /// <summary>
    /// Gets a drawing by ID.
    /// </summary>
    /// <param name="id">The ID of the drawing.</param>
    /// <returns>The drawing, or null if not found.</returns>
    public IDrawingTool? GetDrawingById(string id)
    {
        return _drawings.FirstOrDefault(d => d.Id == id);
    }

    /// <summary>
    /// Hit tests all drawings and returns the first hit.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="tolerance">Hit test tolerance.</param>
    /// <returns>The first drawing hit, or null if none.</returns>
    public IDrawingTool? HitTest(float x, float y, float tolerance = 5f)
    {
        // Test in reverse order so top-most drawings are hit first
        for (int i = _drawings.Count - 1; i >= 0; i--)
        {
            if (_drawings[i].IsVisible && _drawings[i].HitTest(x, y, tolerance))
            {
                return _drawings[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Selects a drawing and deselects all others.
    /// </summary>
    /// <param name="drawing">The drawing to select, or null to deselect all.</param>
    public void SelectDrawing(IDrawingTool? drawing)
    {
        foreach (var d in _drawings)
        {
            d.IsSelected = d == drawing;
        }
    }

    /// <summary>
    /// Gets the currently selected drawing.
    /// </summary>
    /// <returns>The selected drawing, or null if none selected.</returns>
    public IDrawingTool? GetSelectedDrawing()
    {
        return _drawings.FirstOrDefault(d => d.IsSelected);
    }

    /// <summary>
    /// Serializes all drawings to JSON.
    /// </summary>
    /// <returns>JSON string containing all drawings.</returns>
    public string SerializeToJson()
    {
        var drawingData = _drawings.Select(d => d.Serialize()).ToList();
        return JsonSerializer.Serialize(drawingData, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    /// <summary>
    /// Deserializes drawings from JSON.
    /// </summary>
    /// <param name="json">JSON string containing drawings.</param>
    /// <returns>True if deserialization was successful.</returns>
    public bool DeserializeFromJson(string json)
    {
        try
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
            if (jsonElement.ValueKind != JsonValueKind.Array)
                return false;

            _drawings.Clear();

            foreach (var element in jsonElement.EnumerateArray())
            {
                if (element.ValueKind != JsonValueKind.Object)
                    continue;

                var data = new Dictionary<string, object>();
                foreach (var property in element.EnumerateObject())
                {
                    data[property.Name] = ConvertJsonElement(property.Value);
                }

                if (!data.TryGetValue("Type", out var typeObj))
                    continue;

                string? typeName = typeObj.ToString();
                if (string.IsNullOrEmpty(typeName))
                    continue;

                IDrawingTool? drawing = typeName switch
                {
                    "TrendLine" => new TrendLine(),
                    "FibonacciRetracement" => new FibonacciRetracement(),
                    "FibonacciExtension" => new FibonacciExtension(),
                    "HorizontalLine" => new HorizontalLine(),
                    "VerticalLine" => new VerticalLine(),
                    "Rectangle" => new Rectangle(),
                    "Ellipse" => new Ellipse(),
                    "TextAnnotation" => new TextAnnotation(),
                    _ => null
                };

                if (drawing != null)
                {
                    drawing.Deserialize(data);
                    _drawings.Add(drawing);
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static object ConvertJsonElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? string.Empty,
            JsonValueKind.Number => element.TryGetInt32(out var intValue) ? intValue : element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Array => element.EnumerateArray().Select(ConvertJsonElement).ToArray(),
            JsonValueKind.Object => element.EnumerateObject().ToDictionary(p => p.Name, p => ConvertJsonElement(p.Value)),
            _ => element.ToString() ?? string.Empty
        };
    }

    /// <summary>
    /// Exports drawings to a dictionary for custom serialization.
    /// </summary>
    /// <returns>List of drawing dictionaries.</returns>
    public List<Dictionary<string, object>> ExportToData()
    {
        return _drawings.Select(d => d.Serialize()).ToList();
    }

    /// <summary>
    /// Imports drawings from a dictionary.
    /// </summary>
    /// <param name="drawingDataList">List of drawing dictionaries.</param>
    /// <returns>True if import was successful.</returns>
    public bool ImportFromData(List<Dictionary<string, object>> drawingDataList)
    {
        try
        {
            _drawings.Clear();

            foreach (var data in drawingDataList)
            {
                if (!data.TryGetValue("Type", out var typeObj))
                    continue;

                string? typeName = typeObj.ToString();
                if (string.IsNullOrEmpty(typeName))
                    continue;

                IDrawingTool? drawing = typeName switch
                {
                    "TrendLine" => new TrendLine(),
                    "FibonacciRetracement" => new FibonacciRetracement(),
                    "FibonacciExtension" => new FibonacciExtension(),
                    "HorizontalLine" => new HorizontalLine(),
                    "VerticalLine" => new VerticalLine(),
                    "Rectangle" => new Rectangle(),
                    "Ellipse" => new Ellipse(),
                    "TextAnnotation" => new TextAnnotation(),
                    _ => null
                };

                if (drawing != null)
                {
                    drawing.Deserialize(data);
                    _drawings.Add(drawing);
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
