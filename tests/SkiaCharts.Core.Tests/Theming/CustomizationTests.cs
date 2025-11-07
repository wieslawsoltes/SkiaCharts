using SkiaCharts.Core.Theming;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Theming;

public class CustomizationTests
{
    #region Style Overrides Tests

    [Fact]
    public void StyleOverrides_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var overrides = new StyleOverrides();

        // Act
        overrides.Set("LineColor", SKColors.Red);
        overrides.Set("LineWidth", 2.5f);
        overrides.Set("ShowMarkers", true);

        // Assert
        Assert.Equal(SKColors.Red, overrides.Get<SKColor>("LineColor"));
        Assert.Equal(2.5f, overrides.Get<float>("LineWidth"));
        Assert.True(overrides.Get<bool>("ShowMarkers"));
    }

    [Fact]
    public void StyleOverrides_HasOverride_DetectsCorrectly()
    {
        // Arrange
        var overrides = new StyleOverrides();
        overrides.Set("LineColor", SKColors.Blue);

        // Assert
        Assert.True(overrides.HasOverride("LineColor"));
        Assert.False(overrides.HasOverride("LineWidth"));
    }

    [Fact]
    public void StyleOverrides_Clear_RemovesOverride()
    {
        // Arrange
        var overrides = new StyleOverrides();
        overrides.Set("LineColor", SKColors.Red);
        overrides.Set("LineWidth", 2.0f);

        // Act
        overrides.Clear("LineColor");

        // Assert
        Assert.False(overrides.HasOverride("LineColor"));
        Assert.True(overrides.HasOverride("LineWidth"));
    }

    [Fact]
    public void StyleOverrides_ClearAll_RemovesAllOverrides()
    {
        // Arrange
        var overrides = new StyleOverrides();
        overrides.Set("LineColor", SKColors.Red);
        overrides.Set("LineWidth", 2.0f);

        // Act
        overrides.ClearAll();

        // Assert
        Assert.False(overrides.HasOverride("LineColor"));
        Assert.False(overrides.HasOverride("LineWidth"));
    }

    [Fact]
    public void StyleOverrides_Clone_CreatesDeepCopy()
    {
        // Arrange
        var original = new StyleOverrides();
        original.Set("LineColor", SKColors.Red);

        // Act
        var clone = original.Clone();
        clone.Set("LineColor", SKColors.Blue);

        // Assert
        Assert.Equal(SKColors.Red, original.Get<SKColor>("LineColor"));
        Assert.Equal(SKColors.Blue, clone.Get<SKColor>("LineColor"));
    }

    [Fact]
    public void StyleExtensions_TypeSafeGetters_WorkCorrectly()
    {
        // Arrange
        var overrides = new StyleOverrides();

        // Act
        overrides.SetColor("LineColor", SKColors.Red);
        overrides.SetFloat("LineWidth", 2.5f);
        overrides.SetBool("ShowMarkers", true);

        // Assert
        Assert.Equal(SKColors.Red, overrides.GetColor("LineColor", SKColors.Black));
        Assert.Equal(2.5f, overrides.GetFloat("LineWidth", 1.0f));
        Assert.True(overrides.GetBool("ShowMarkers", false));
    }

    #endregion

    #region Style Selector Tests

    [Fact]
    public void StyleSelector_IdSelector_MatchesCorrectly()
    {
        // Arrange
        var selector = StyleSelector.Id("my-series");
        var element = new TestStyleableElement { StyleId = "my-series" };

        // Assert
        Assert.True(selector.Matches(element));
    }

    [Fact]
    public void StyleSelector_ClassSelector_MatchesCorrectly()
    {
        // Arrange
        var selector = StyleSelector.Class("highlight");
        var element = new TestStyleableElement();
        element.StyleClasses.Add("highlight");

        // Assert
        Assert.True(selector.Matches(element));
    }

    [Fact]
    public void StyleSelector_ElementSelector_MatchesCorrectly()
    {
        // Arrange
        var selector = StyleSelector.Element("TestStyleableElement");
        var element = new TestStyleableElement();

        // Assert
        Assert.True(selector.Matches(element));
    }

    #endregion

    #region StyleSheet Tests

    [Fact]
    public void StyleSheet_ApplyStyles_AppliesInCorrectOrder()
    {
        // Arrange
        var stylesheet = new StyleSheet();

        var elementSelector = StyleSelector.Element("TestStyleableElement");
        elementSelector.Overrides.SetColor("LineColor", SKColors.Black);

        var classSelector = StyleSelector.Class("highlight");
        classSelector.Overrides.SetColor("LineColor", SKColors.Yellow);

        var idSelector = StyleSelector.Id("special");
        idSelector.Overrides.SetColor("LineColor", SKColors.Red);

        stylesheet.AddSelector(elementSelector);
        stylesheet.AddSelector(classSelector);
        stylesheet.AddSelector(idSelector);

        var element = new TestStyleableElement { StyleId = "special" };
        element.StyleClasses.Add("highlight");

        // Act
        stylesheet.ApplyStyles(element);

        // Assert - ID selector (highest specificity) should win
        Assert.Equal(SKColors.Red, element.StyleOverrides.GetColor("LineColor", SKColors.White));
    }

    [Fact]
    public void StyleSheet_Clone_CreatesIndependentCopy()
    {
        // Arrange
        var original = new StyleSheet();
        var selector = StyleSelector.Class("test");
        selector.Overrides.SetColor("LineColor", SKColors.Red);
        original.AddSelector(selector);

        // Act
        var clone = original.Clone();
        clone.Clear();

        // Assert
        var element = new TestStyleableElement();
        element.StyleClasses.Add("test");
        Assert.Single(original.GetMatchingSelectors(element));
    }

    #endregion

    #region Custom Renderer Tests

    [Fact]
    public void RendererRegistry_RegisterAndGet_WorksCorrectly()
    {
        // Arrange
        var registry = new RendererRegistry();
        var markerRenderer = new DefaultMarkerRenderer();

        // Act
        registry.RegisterMarkerRenderer(markerRenderer);

        // Assert
        Assert.Same(markerRenderer, registry.GetMarkerRenderer());
    }

    [Fact]
    public void RendererRegistry_ClearAll_RemovesAllRenderers()
    {
        // Arrange
        var registry = new RendererRegistry();
        registry.RegisterMarkerRenderer(new DefaultMarkerRenderer());
        registry.RegisterMarkerRenderer(new SquareMarkerRenderer());

        // Act
        registry.ClearAll();

        // Assert
        Assert.Null(registry.GetMarkerRenderer());
        Assert.Null(registry.GetLineRenderer());
    }

    #endregion

    #region Print Themes Tests

    [Fact]
    public void PrintThemes_AllThemes_HaveCorrectProperties()
    {
        // Assert
        Assert.Equal("Print", PrintThemes.Print.Name);
        Assert.Equal("Grayscale", PrintThemes.Grayscale.Name);
        Assert.Equal("High DPI", PrintThemes.HighDpi.Name);
        Assert.Equal("Publication", PrintThemes.Publication.Name);
        Assert.Equal("Pattern Fill", PrintThemes.PatternFill.Name);
    }

    [Fact]
    public void PrintThemes_GetByName_FindsTheme()
    {
        // Act
        var theme = PrintThemes.GetByName("Print");

        // Assert
        Assert.NotNull(theme);
        Assert.Equal("Print", theme.Name);
    }

    [Fact]
    public void PrintThemes_GetByName_IsCaseInsensitive()
    {
        // Act
        var theme = PrintThemes.GetByName("print");

        // Assert
        Assert.NotNull(theme);
        Assert.Equal("Print", theme.Name);
    }

    [Fact]
    public void ExportSettings_ForPrint_HasCorrectDefaults()
    {
        // Act
        var settings = ExportSettings.ForPrint();

        // Assert
        Assert.Equal(300, settings.Dpi);
        Assert.Equal(ExportFormat.Pdf, settings.Format);
        Assert.NotNull(settings.ExportTheme);
    }

    [Fact]
    public void ExportSettings_ForWeb_HasCorrectDefaults()
    {
        // Act
        var settings = ExportSettings.ForWeb();

        // Assert
        Assert.Equal(96, settings.Dpi);
        Assert.Equal(ExportFormat.Png, settings.Format);
        Assert.False(settings.TransparentBackground);
    }

    #endregion

    #region Theme Serialization Tests

    [Fact]
    public void ThemeSerialization_ToJsonAndFromJson_RoundTrip()
    {
        // Arrange
        var original = ThemePresets.Light;

        // Act
        var json = ThemeSerialization.ToJson(original);
        var deserialized = ThemeSerialization.FromJson(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(original.Name, deserialized.Name);
        Assert.Equal(original.Background.Color, deserialized.Background.Color);
        Assert.Equal(original.Axis.LineWidth, deserialized.Axis.LineWidth);
    }

    [Fact]
    public void ThemeSerialization_SaveAndLoad_WorksCorrectly()
    {
        // Arrange
        var theme = ThemePresets.Dark;
        var tempFile = Path.Combine(Path.GetTempPath(), "test-theme.json");

        try
        {
            // Act
            ThemeSerialization.SaveToFile(theme, tempFile);
            var loaded = ThemeSerialization.LoadFromFile(tempFile);

            // Assert
            Assert.NotNull(loaded);
            Assert.Equal(theme.Name, loaded.Name);
            Assert.Equal(theme.Background.Color, loaded.Background.Color);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public void SKColorJsonConverter_SerializesAsHex()
    {
        // Arrange
        var theme = new ChartTheme();
        theme.Background.Color = new SKColor(255, 128, 64, 200);

        // Act
        var json = ThemeSerialization.ToJson(theme);

        // Assert
        Assert.Contains("#FF8040C8", json); // RGBA in hex
    }

    [Fact]
    public void ThemeLibrary_SaveAndLoad_WorksCorrectly()
    {
        // Arrange
        var libraryPath = Path.Combine(Path.GetTempPath(), "theme-library-test");
        var library = new ThemeLibrary(libraryPath);
        var theme = ThemePresets.Professional.Clone();
        theme.Name = "My Custom Theme";

        try
        {
            // Act
            library.SaveTheme(theme);
            var loaded = library.LoadTheme("My Custom Theme");

            // Assert
            Assert.NotNull(loaded);
            Assert.Equal("My Custom Theme", loaded.Name);
        }
        finally
        {
            if (Directory.Exists(libraryPath))
                Directory.Delete(libraryPath, true);
        }
    }

    [Fact]
    public void ThemeExport_ExportAndImportBundle_WorksCorrectly()
    {
        // Arrange
        var theme = ThemePresets.HighContrast;

        // Act
        var bundle = ThemeExport.ExportBundle(theme);
        var imported = ThemeExport.ImportBundle(bundle);

        // Assert
        Assert.NotNull(imported);
        Assert.Equal(theme.Name, imported.Name);
    }

    #endregion

    #region Theme Editor Tests

    [Fact]
    public void ThemeEditor_SetName_UpdatesTheme()
    {
        // Arrange
        var editor = new ThemeEditor(ThemePresets.Light);

        // Act
        editor.SetName("My Theme");

        // Assert
        Assert.Equal("My Theme", editor.CurrentTheme.Name);
    }

    [Fact]
    public void ThemeEditor_SetBackgroundColor_UpdatesTheme()
    {
        // Arrange
        var editor = new ThemeEditor(ThemePresets.Light);

        // Act
        editor.SetBackgroundColor(SKColors.Blue);

        // Assert
        Assert.Equal(SKColors.Blue, editor.CurrentTheme.Background.Color);
    }

    [Fact]
    public void ThemeEditor_Undo_RestoresPreviousState()
    {
        // Arrange
        var editor = new ThemeEditor(ThemePresets.Light);
        var originalColor = editor.CurrentTheme.Background.Color;

        // Act
        editor.SetBackgroundColor(SKColors.Red);
        editor.Undo();

        // Assert
        Assert.Equal(originalColor, editor.CurrentTheme.Background.Color);
    }

    [Fact]
    public void ThemeEditor_Redo_RestoresNextState()
    {
        // Arrange
        var editor = new ThemeEditor(ThemePresets.Light);

        // Act
        editor.SetBackgroundColor(SKColors.Red);
        editor.Undo();
        editor.Redo();

        // Assert
        Assert.Equal(SKColors.Red, editor.CurrentTheme.Background.Color);
    }

    [Fact]
    public void ThemeEditor_CanUndoRedo_ReflectsState()
    {
        // Arrange
        var editor = new ThemeEditor(ThemePresets.Light);

        // Assert - Initially no undo/redo
        Assert.False(editor.CanUndo);
        Assert.False(editor.CanRedo);

        // Act - Make a change
        editor.SetName("Test");

        // Assert - Can undo, cannot redo
        Assert.True(editor.CanUndo);
        Assert.False(editor.CanRedo);

        // Act - Undo
        editor.Undo();

        // Assert - Cannot undo, can redo
        Assert.False(editor.CanUndo);
        Assert.True(editor.CanRedo);
    }

    [Fact]
    public void ThemeEditor_MergeTheme_CombinesThemes()
    {
        // Arrange
        var editor = new ThemeEditor(ThemePresets.Light);
        var darkTheme = ThemePresets.Dark;

        // Act
        editor.MergeTheme(darkTheme);

        // Assert
        Assert.Equal(darkTheme.Background.Color, editor.CurrentTheme.Background.Color);
    }

    [Fact]
    public void ThemeEditor_ExportTheme_CreatesIndependentCopy()
    {
        // Arrange
        var editor = new ThemeEditor(ThemePresets.Light);
        editor.SetName("Original");

        // Act
        var exported = editor.ExportTheme();
        editor.SetName("Modified");

        // Assert
        Assert.Equal("Original", exported.Name);
        Assert.Equal("Modified", editor.CurrentTheme.Name);
    }

    #endregion

    #region Theme Validator Tests

    [Fact]
    public void ThemeValidator_ValidTheme_PassesValidation()
    {
        // Arrange
        var theme = ThemePresets.Light;

        // Act
        var result = ThemeValidator.Validate(theme);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void ThemeValidator_EmptyPalette_FailsValidation()
    {
        // Arrange
        var theme = ThemePresets.Light.Clone();
        theme.ColorPalette = new ColorPalette("Empty", ColorPaletteType.Categorical);

        // Act
        var result = ThemeValidator.Validate(theme);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Color palette must have at least one color"));
    }

    [Fact]
    public void ThemeValidator_LowContrast_GeneratesWarning()
    {
        // Arrange
        var theme = ThemePresets.Light.Clone();
        theme.Background.Color = SKColors.White;
        theme.Axis.LabelColor = new SKColor(240, 240, 240); // Very low contrast

        // Act
        var result = ThemeValidator.Validate(theme);

        // Assert
        Assert.Contains(result.Warnings, w => w.Contains("insufficient contrast"));
    }

    [Fact]
    public void ThemeValidator_SmallFonts_GeneratesWarning()
    {
        // Arrange
        var theme = ThemePresets.Light.Clone();
        theme.Axis.LabelFontSize = 6.0f;

        // Act
        var result = ThemeValidator.Validate(theme);

        // Assert
        Assert.Contains(result.Warnings, w => w.Contains("too small"));
    }

    #endregion

    #region Helper Classes

    private class TestStyleableElement : IStyleable
    {
        public StyleOverrides StyleOverrides { get; } = new();
        public List<string> StyleClasses { get; } = new();
        public string? StyleId { get; set; }
    }

    #endregion
}
