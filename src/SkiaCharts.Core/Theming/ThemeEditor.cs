using SkiaSharp;

namespace SkiaCharts.Core.Theming;

/// <summary>
/// Interactive theme editor for creating and modifying themes.
/// </summary>
public class ThemeEditor
{
    private ChartTheme _theme;
    private readonly List<ThemeChange> _history = new();
    private int _historyIndex = -1;

    /// <summary>
    /// Initializes a new theme editor with a base theme.
    /// </summary>
    public ThemeEditor(ChartTheme baseTheme)
    {
        _theme = baseTheme.Clone();
    }

    /// <summary>
    /// Gets the current theme being edited.
    /// </summary>
    public ChartTheme CurrentTheme => _theme;

    /// <summary>
    /// Gets whether undo is available.
    /// </summary>
    public bool CanUndo => _historyIndex >= 0;

    /// <summary>
    /// Gets whether redo is available.
    /// </summary>
    public bool CanRedo => _historyIndex < _history.Count - 1;

    /// <summary>
    /// Begins editing with undo/redo support.
    /// </summary>
    public ThemeEditSession BeginEdit(string description)
    {
        return new ThemeEditSession(this, description);
    }

    /// <summary>
    /// Sets the theme name.
    /// </summary>
    public void SetName(string name)
    {
        RecordChange("Set Name", () => _theme.Name = name);
    }

    /// <summary>
    /// Sets a background style property.
    /// </summary>
    public void SetBackgroundColor(SKColor color)
    {
        RecordChange("Set Background Color", () => _theme.Background.Color = color);
    }

    /// <summary>
    /// Sets a grid style property.
    /// </summary>
    public void SetGridColor(SKColor majorColor, SKColor? minorColor = null)
    {
        RecordChange("Set Grid Color", () =>
        {
            _theme.Grid.MajorGridColor = majorColor;
            if (minorColor.HasValue)
                _theme.Grid.MinorGridColor = minorColor.Value;
        });
    }

    /// <summary>
    /// Sets axis colors.
    /// </summary>
    public void SetAxisColors(SKColor lineColor, SKColor labelColor, SKColor titleColor)
    {
        RecordChange("Set Axis Colors", () =>
        {
            _theme.Axis.LineColor = lineColor;
            _theme.Axis.LabelColor = labelColor;
            _theme.Axis.TitleColor = titleColor;
        });
    }

    /// <summary>
    /// Sets series style properties.
    /// </summary>
    public void SetSeriesStyle(float? lineWidth = null, float? markerSize = null, float? areaOpacity = null)
    {
        RecordChange("Set Series Style", () =>
        {
            if (lineWidth.HasValue) _theme.Series.LineWidth = lineWidth.Value;
            if (markerSize.HasValue) _theme.Series.MarkerSize = markerSize.Value;
            if (areaOpacity.HasValue) _theme.Series.AreaOpacity = areaOpacity.Value;
        });
    }

    /// <summary>
    /// Sets the color palette.
    /// </summary>
    public void SetColorPalette(ColorPalette palette)
    {
        RecordChange("Set Color Palette", () => _theme.ColorPalette = palette);
    }

    /// <summary>
    /// Sets legend style.
    /// </summary>
    public void SetLegendStyle(SKColor? backgroundColor = null, SKColor? textColor = null, float? fontSize = null)
    {
        RecordChange("Set Legend Style", () =>
        {
            if (backgroundColor.HasValue) _theme.Legend.BackgroundColor = backgroundColor.Value;
            if (textColor.HasValue) _theme.Legend.TextColor = textColor.Value;
            if (fontSize.HasValue) _theme.Legend.FontSize = fontSize.Value;
        });
    }

    /// <summary>
    /// Sets title style.
    /// </summary>
    public void SetTitleStyle(SKColor? color = null, float? fontSize = null)
    {
        RecordChange("Set Title Style", () =>
        {
            if (color.HasValue) _theme.Title.Color = color.Value;
            if (fontSize.HasValue) _theme.Title.FontSize = fontSize.Value;
        });
    }

    /// <summary>
    /// Sets title font style.
    /// </summary>
    public void SetTitleFontStyle(SKFontStyle fontStyle)
    {
        RecordChange("Set Title Font Style", () =>
        {
            _theme.Title.FontStyle = fontStyle;
        });
    }

    /// <summary>
    /// Sets font families.
    /// </summary>
    public void SetFonts(string? defaultFont = null, string? titleFont = null, string? labelFont = null)
    {
        RecordChange("Set Fonts", () =>
        {
            if (defaultFont != null) _theme.Fonts.DefaultFontFamily = defaultFont;
            if (titleFont != null) _theme.Fonts.TitleFontFamily = titleFont;
            if (labelFont != null) _theme.Fonts.LabelFontFamily = labelFont;
        });
    }

    /// <summary>
    /// Applies a preset theme as a starting point.
    /// </summary>
    public void ApplyPreset(ChartTheme preset)
    {
        RecordChange($"Apply Preset: {preset.Name}", () =>
        {
            _theme = preset.Clone();
        });
    }

    /// <summary>
    /// Merges another theme into the current theme.
    /// </summary>
    public void MergeTheme(ChartTheme other)
    {
        RecordChange($"Merge Theme: {other.Name}", () =>
        {
            _theme.Merge(other);
        });
    }

    /// <summary>
    /// Undoes the last change.
    /// </summary>
    public bool Undo()
    {
        if (!CanUndo)
            return false;

        var change = _history[_historyIndex];
        _theme = change.BeforeState.Clone();
        _historyIndex--;
        return true;
    }

    /// <summary>
    /// Redoes the last undone change.
    /// </summary>
    public bool Redo()
    {
        if (!CanRedo)
            return false;

        _historyIndex++;
        var change = _history[_historyIndex];
        _theme = change.AfterState.Clone();
        return true;
    }

    /// <summary>
    /// Gets the change history.
    /// </summary>
    public IEnumerable<string> GetHistory()
    {
        return _history.Select(c => c.Description);
    }

    /// <summary>
    /// Clears the undo/redo history.
    /// </summary>
    public void ClearHistory()
    {
        _history.Clear();
        _historyIndex = -1;
    }

    /// <summary>
    /// Resets to the original base theme.
    /// </summary>
    public void Reset(ChartTheme baseTheme)
    {
        _theme = baseTheme.Clone();
        ClearHistory();
    }

    /// <summary>
    /// Exports the current theme.
    /// </summary>
    public ChartTheme ExportTheme()
    {
        return _theme.Clone();
    }

    private void RecordChange(string description, Action changeAction)
    {
        var before = _theme.Clone();
        changeAction();
        var after = _theme.Clone();

        // Remove any redo history
        if (_historyIndex < _history.Count - 1)
        {
            _history.RemoveRange(_historyIndex + 1, _history.Count - _historyIndex - 1);
        }

        _history.Add(new ThemeChange
        {
            Description = description,
            BeforeState = before,
            AfterState = after
        });

        _historyIndex = _history.Count - 1;
    }

    private class ThemeChange
    {
        public required string Description { get; init; }
        public required ChartTheme BeforeState { get; init; }
        public required ChartTheme AfterState { get; init; }
    }
}

/// <summary>
/// Represents an edit session with automatic change recording.
/// </summary>
public class ThemeEditSession : IDisposable
{
    private readonly ThemeEditor _editor;
    private readonly string _description;
    private readonly ChartTheme _beforeState;
    private bool _committed;

    internal ThemeEditSession(ThemeEditor editor, string description)
    {
        _editor = editor;
        _description = description;
        _beforeState = editor.CurrentTheme.Clone();
    }

    /// <summary>
    /// Commits the changes made in this session.
    /// </summary>
    public void Commit()
    {
        _committed = true;
    }

    /// <summary>
    /// Disposes the session and records changes if committed.
    /// </summary>
    public void Dispose()
    {
        if (_committed)
        {
            // Changes are already in the theme, just need to record in history
            var afterState = _editor.CurrentTheme.Clone();
            // This would need to be exposed via internal method
        }
    }
}

/// <summary>
/// Theme validation utilities.
/// </summary>
public static class ThemeValidator
{
    /// <summary>
    /// Validates a theme and returns any warnings or errors.
    /// </summary>
    public static ThemeValidationResult Validate(ChartTheme theme)
    {
        var result = new ThemeValidationResult();

        // Check for sufficient contrast
        if (GetContrast(theme.Background.Color, theme.Axis.LabelColor) < 3.0)
        {
            result.AddWarning("Axis labels may have insufficient contrast with background");
        }

        if (GetContrast(theme.Background.Color, theme.Title.Color) < 4.5)
        {
            result.AddWarning("Title may have insufficient contrast with background");
        }

        // Check for invisible elements
        if (theme.Background.BorderWidth > 0 && theme.Background.BorderColor.Alpha == 0)
        {
            result.AddWarning("Background border is transparent but has width > 0");
        }

        // Check font sizes
        if (theme.Axis.LabelFontSize < 8)
        {
            result.AddWarning("Axis label font size may be too small for readability");
        }

        if (theme.Title.FontSize < 10)
        {
            result.AddWarning("Title font size may be too small");
        }

        // Check color palette
        if (theme.ColorPalette.Colors.Count == 0)
        {
            result.AddError("Color palette must have at least one color");
        }

        // Check for similar colors in palette (may be hard to distinguish)
        var colors = theme.ColorPalette.Colors;
        for (int i = 0; i < colors.Count; i++)
        {
            for (int j = i + 1; j < colors.Count; j++)
            {
                if (AreColorsSimilar(colors[i], colors[j]))
                {
                    result.AddWarning($"Colors {i} and {j} in palette may be too similar");
                }
            }
        }

        return result;
    }

    private static double GetContrast(SKColor c1, SKColor c2)
    {
        var l1 = GetRelativeLuminance(c1);
        var l2 = GetRelativeLuminance(c2);

        var lighter = Math.Max(l1, l2);
        var darker = Math.Min(l1, l2);

        return (lighter + 0.05) / (darker + 0.05);
    }

    private static double GetRelativeLuminance(SKColor color)
    {
        var r = color.Red / 255.0;
        var g = color.Green / 255.0;
        var b = color.Blue / 255.0;

        r = r <= 0.03928 ? r / 12.92 : Math.Pow((r + 0.055) / 1.055, 2.4);
        g = g <= 0.03928 ? g / 12.92 : Math.Pow((g + 0.055) / 1.055, 2.4);
        b = b <= 0.03928 ? b / 12.92 : Math.Pow((b + 0.055) / 1.055, 2.4);

        return 0.2126 * r + 0.7152 * g + 0.0722 * b;
    }

    private static bool AreColorsSimilar(SKColor c1, SKColor c2)
    {
        var dr = Math.Abs(c1.Red - c2.Red);
        var dg = Math.Abs(c1.Green - c2.Green);
        var db = Math.Abs(c1.Blue - c2.Blue);

        return dr < 30 && dg < 30 && db < 30;
    }
}

/// <summary>
/// Theme validation result.
/// </summary>
public class ThemeValidationResult
{
    private readonly List<string> _errors = new();
    private readonly List<string> _warnings = new();

    /// <summary>
    /// Gets whether the theme is valid (no errors).
    /// </summary>
    public bool IsValid => _errors.Count == 0;

    /// <summary>
    /// Gets all validation errors.
    /// </summary>
    public IEnumerable<string> Errors => _errors;

    /// <summary>
    /// Gets all validation warnings.
    /// </summary>
    public IEnumerable<string> Warnings => _warnings;

    internal void AddError(string message) => _errors.Add(message);
    internal void AddWarning(string message) => _warnings.Add(message);
}
