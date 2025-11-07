using System.Text.Json;
using System.Text.Json.Serialization;
using SkiaSharp;

namespace SkiaCharts.Core.Theming;

/// <summary>
/// Handles theme serialization to/from JSON.
/// </summary>
public static class ThemeSerialization
{
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new SKColorJsonConverter(),
            new SKFontStyleJsonConverter(),
            new ColorPaletteJsonConverter()
        }
    };

    /// <summary>
    /// Serializes a theme to JSON string.
    /// </summary>
    public static string ToJson(ChartTheme theme)
    {
        return JsonSerializer.Serialize(theme, _options);
    }

    /// <summary>
    /// Deserializes a theme from JSON string.
    /// </summary>
    public static ChartTheme? FromJson(string json)
    {
        return JsonSerializer.Deserialize<ChartTheme>(json, _options);
    }

    /// <summary>
    /// Saves a theme to a JSON file.
    /// </summary>
    public static void SaveToFile(ChartTheme theme, string filePath)
    {
        var json = ToJson(theme);
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// Loads a theme from a JSON file.
    /// </summary>
    public static ChartTheme? LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            return null;

        var json = File.ReadAllText(filePath);
        return FromJson(json);
    }
}

/// <summary>
/// JSON converter for SKColor.
/// </summary>
public class SKColorJsonConverter : JsonConverter<SKColor>
{
    public override SKColor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var hex = reader.GetString();
            return ParseHexColor(hex ?? "#000000");
        }
        else if (reader.TokenType == JsonTokenType.StartObject)
        {
            byte r = 0, g = 0, b = 0, a = 255;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName?.ToLowerInvariant())
                    {
                        case "r": r = (byte)reader.GetInt32(); break;
                        case "g": g = (byte)reader.GetInt32(); break;
                        case "b": b = (byte)reader.GetInt32(); break;
                        case "a": a = (byte)reader.GetInt32(); break;
                    }
                }
            }

            return new SKColor(r, g, b, a);
        }

        return SKColors.Black;
    }

    public override void Write(Utf8JsonWriter writer, SKColor value, JsonSerializerOptions options)
    {
        // Write as hex string for compactness
        writer.WriteStringValue($"#{value.Red:X2}{value.Green:X2}{value.Blue:X2}{value.Alpha:X2}");
    }

    private static SKColor ParseHexColor(string hex)
    {
        hex = hex.TrimStart('#');

        if (hex.Length == 6)
            hex += "FF"; // Add alpha

        if (hex.Length != 8)
            return SKColors.Black;

        var r = Convert.ToByte(hex.Substring(0, 2), 16);
        var g = Convert.ToByte(hex.Substring(2, 2), 16);
        var b = Convert.ToByte(hex.Substring(4, 2), 16);
        var a = Convert.ToByte(hex.Substring(6, 2), 16);

        return new SKColor(r, g, b, a);
    }
}

/// <summary>
/// JSON converter for SKFontStyle.
/// </summary>
public class SKFontStyleJsonConverter : JsonConverter<SKFontStyle>
{
    public override SKFontStyle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString()?.ToLowerInvariant();
        return value switch
        {
            "bold" => SKFontStyle.Bold,
            "italic" => SKFontStyle.Italic,
            "bolditalic" => SKFontStyle.BoldItalic,
            _ => SKFontStyle.Normal
        };
    }

    public override void Write(Utf8JsonWriter writer, SKFontStyle value, JsonSerializerOptions options)
    {
        var str = value.Weight switch
        {
            >= 700 when value.Slant == SKFontStyleSlant.Italic => "BoldItalic",
            >= 700 => "Bold",
            _ when value.Slant == SKFontStyleSlant.Italic => "Italic",
            _ => "Normal"
        };
        writer.WriteStringValue(str);
    }
}

/// <summary>
/// JSON converter for ColorPalette.
/// </summary>
public class ColorPaletteJsonConverter : JsonConverter<ColorPalette>
{
    public override ColorPalette Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var name = "Custom";
        var type = ColorPaletteType.Categorical;
        var colors = new List<SKColor>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName?.ToLowerInvariant())
                {
                    case "name":
                        name = reader.GetString() ?? "Custom";
                        break;

                    case "type":
                        var typeStr = reader.GetString()?.ToLowerInvariant();
                        type = typeStr switch
                        {
                            "sequential" => ColorPaletteType.Sequential,
                            "diverging" => ColorPaletteType.Diverging,
                            _ => ColorPaletteType.Categorical
                        };
                        break;

                    case "colors":
                        if (reader.TokenType == JsonTokenType.StartArray)
                        {
                            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                            {
                                if (reader.TokenType == JsonTokenType.String)
                                {
                                    var colorStr = reader.GetString();
                                    if (!string.IsNullOrEmpty(colorStr))
                                    {
                                        colors.Add(ParseHexColor(colorStr));
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }

        return new ColorPalette(name, type, colors.ToArray());
    }

    public override void Write(Utf8JsonWriter writer, ColorPalette value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("name", value.Name);
        writer.WriteString("type", value.Type.ToString());

        writer.WritePropertyName("colors");
        writer.WriteStartArray();
        foreach (var color in value.Colors)
        {
            writer.WriteStringValue($"#{color.Red:X2}{color.Green:X2}{color.Blue:X2}{color.Alpha:X2}");
        }
        writer.WriteEndArray();

        writer.WriteEndObject();
    }

    private static SKColor ParseHexColor(string hex)
    {
        hex = hex.TrimStart('#');

        if (hex.Length == 6)
            hex += "FF";

        if (hex.Length != 8)
            return SKColors.Black;

        var r = Convert.ToByte(hex.Substring(0, 2), 16);
        var g = Convert.ToByte(hex.Substring(2, 2), 16);
        var b = Convert.ToByte(hex.Substring(4, 2), 16);
        var a = Convert.ToByte(hex.Substring(6, 2), 16);

        return new SKColor(r, g, b, a);
    }
}

/// <summary>
/// Theme library for managing saved themes.
/// </summary>
public class ThemeLibrary
{
    private readonly Dictionary<string, ChartTheme> _themes = new();
    private readonly string _libraryPath;

    /// <summary>
    /// Initializes a new theme library.
    /// </summary>
    /// <param name="libraryPath">Directory path for storing themes.</param>
    public ThemeLibrary(string libraryPath)
    {
        _libraryPath = libraryPath;
        Directory.CreateDirectory(libraryPath);
        LoadAllThemes();
    }

    /// <summary>
    /// Saves a theme to the library.
    /// </summary>
    public void SaveTheme(ChartTheme theme)
    {
        _themes[theme.Name] = theme;
        var filePath = Path.Combine(_libraryPath, $"{SanitizeFileName(theme.Name)}.json");
        ThemeSerialization.SaveToFile(theme, filePath);
    }

    /// <summary>
    /// Loads a theme from the library.
    /// </summary>
    public ChartTheme? LoadTheme(string name)
    {
        if (_themes.TryGetValue(name, out var theme))
            return theme;

        var filePath = Path.Combine(_libraryPath, $"{SanitizeFileName(name)}.json");
        theme = ThemeSerialization.LoadFromFile(filePath);

        if (theme != null)
            _themes[name] = theme;

        return theme;
    }

    /// <summary>
    /// Deletes a theme from the library.
    /// </summary>
    public bool DeleteTheme(string name)
    {
        _themes.Remove(name);
        var filePath = Path.Combine(_libraryPath, $"{SanitizeFileName(name)}.json");

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets all theme names in the library.
    /// </summary>
    public IEnumerable<string> GetThemeNames()
    {
        return _themes.Keys;
    }

    /// <summary>
    /// Gets all themes in the library.
    /// </summary>
    public IEnumerable<ChartTheme> GetAllThemes()
    {
        return _themes.Values;
    }

    /// <summary>
    /// Loads all themes from the library directory.
    /// </summary>
    private void LoadAllThemes()
    {
        var files = Directory.GetFiles(_libraryPath, "*.json");

        foreach (var file in files)
        {
            var theme = ThemeSerialization.LoadFromFile(file);
            if (theme != null)
            {
                _themes[theme.Name] = theme;
            }
        }
    }

    private static string SanitizeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Join("_", name.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
    }
}

/// <summary>
/// Theme export/import utilities.
/// </summary>
public static class ThemeExport
{
    /// <summary>
    /// Exports a theme bundle (theme + palette) to JSON.
    /// </summary>
    public static string ExportBundle(ChartTheme theme)
    {
        var bundle = new
        {
            theme = theme,
            exportedAt = DateTime.UtcNow,
            version = "1.0"
        };

        return JsonSerializer.Serialize(bundle, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new SKColorJsonConverter(),
                new SKFontStyleJsonConverter(),
                new ColorPaletteJsonConverter()
            }
        });
    }

    /// <summary>
    /// Imports a theme bundle from JSON.
    /// </summary>
    public static ChartTheme? ImportBundle(string json)
    {
        try
        {
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("theme", out var themeElement))
            {
                var themeJson = themeElement.GetRawText();
                return ThemeSerialization.FromJson(themeJson);
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
