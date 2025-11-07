using SkiaCharts.Core.Theming;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Theming;

public class ThemingTests
{
    // ChartTheme Tests
    [Fact]
    public void ChartTheme_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var theme = new ChartTheme();

        // Assert
        Assert.Equal("Default", theme.Name);
        Assert.NotNull(theme.Background);
        Assert.NotNull(theme.Grid);
        Assert.NotNull(theme.Axis);
        Assert.NotNull(theme.Series);
        Assert.NotNull(theme.Legend);
        Assert.NotNull(theme.Tooltip);
        Assert.NotNull(theme.Title);
        Assert.NotNull(theme.Fonts);
        Assert.NotNull(theme.ColorPalette);
    }

    [Fact]
    public void ChartTheme_Clone_ShouldCreateDeepCopy()
    {
        // Arrange
        var original = new ChartTheme
        {
            Name = "Test"
        };
        original.Background.Color = SKColors.Red;

        // Act
        var clone = original.Clone();
        clone.Background.Color = SKColors.Blue;

        // Assert
        Assert.Equal("Test", clone.Name);
        Assert.Equal(SKColors.Red, original.Background.Color);
        Assert.Equal(SKColors.Blue, clone.Background.Color);
    }

    [Fact]
    public void ChartTheme_Merge_ShouldOverrideProperties()
    {
        // Arrange
        var baseTheme = new ChartTheme();
        baseTheme.Background.Color = SKColors.White;
        baseTheme.Grid.ShowMajorGrid = true;

        var overlay = new ChartTheme();
        overlay.Background.Color = SKColors.Black;
        overlay.Grid.ShowMajorGrid = false;

        // Act
        baseTheme.Merge(overlay);

        // Assert
        Assert.Equal(SKColors.Black, baseTheme.Background.Color);
        Assert.False(baseTheme.Grid.ShowMajorGrid);
    }

    // Theme Presets Tests
    [Fact]
    public void ThemePresets_Light_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var theme = ThemePresets.Light;

        // Assert
        Assert.Equal("Light", theme.Name);
        Assert.Equal(SKColors.White, theme.Background.Color);
        Assert.NotNull(theme.ColorPalette);
    }

    [Fact]
    public void ThemePresets_Dark_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var theme = ThemePresets.Dark;

        // Assert
        Assert.Equal("Dark", theme.Name);
        Assert.Equal(new SKColor(30, 30, 30), theme.Background.Color);
        Assert.NotNull(theme.ColorPalette);
    }

    [Fact]
    public void ThemePresets_HighContrast_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var theme = ThemePresets.HighContrast;

        // Assert
        Assert.Equal("High Contrast", theme.Name);
        Assert.Equal(SKColors.White, theme.Background.Color);
        Assert.Equal(SKColors.Black, theme.Background.BorderColor);
        Assert.Equal(3.0f, theme.Background.BorderWidth);
    }

    [Fact]
    public void ThemePresets_Professional_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var theme = ThemePresets.Professional;

        // Assert
        Assert.Equal("Professional", theme.Name);
        Assert.NotEqual(SKColors.White, theme.Background.Color);
        Assert.NotNull(theme.ColorPalette);
    }

    [Fact]
    public void ThemePresets_All_ShouldReturnAllThemes()
    {
        // Arrange & Act
        var themes = ThemePresets.All.ToList();

        // Assert
        Assert.Equal(4, themes.Count);
        Assert.Contains(themes, t => t.Name == "Light");
        Assert.Contains(themes, t => t.Name == "Dark");
        Assert.Contains(themes, t => t.Name == "High Contrast");
        Assert.Contains(themes, t => t.Name == "Professional");
    }

    [Fact]
    public void ThemePresets_GetByName_ShouldFindTheme()
    {
        // Arrange & Act
        var theme = ThemePresets.GetByName("Dark");

        // Assert
        Assert.NotNull(theme);
        Assert.Equal("Dark", theme.Name);
    }

    [Fact]
    public void ThemePresets_GetByName_CaseInsensitive_ShouldWork()
    {
        // Arrange & Act
        var theme = ThemePresets.GetByName("LIGHT");

        // Assert
        Assert.NotNull(theme);
        Assert.Equal("Light", theme.Name);
    }

    [Fact]
    public void ThemePresets_GetByName_NotFound_ShouldReturnNull()
    {
        // Arrange & Act
        var theme = ThemePresets.GetByName("NonExistent");

        // Assert
        Assert.Null(theme);
    }

    // ColorPalette Tests
    [Fact]
    public void ColorPalette_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var palette = new ColorPalette(
            "Test",
            ColorPaletteType.Categorical,
            SKColors.Red, SKColors.Blue, SKColors.Green);

        // Assert
        Assert.Equal("Test", palette.Name);
        Assert.Equal(ColorPaletteType.Categorical, palette.Type);
        Assert.Equal(3, palette.Colors.Count);
    }

    [Fact]
    public void ColorPalette_GetColor_ShouldReturnColor()
    {
        // Arrange
        var palette = new ColorPalette(
            "Test",
            ColorPaletteType.Categorical,
            SKColors.Red, SKColors.Blue);

        // Act & Assert
        Assert.Equal(SKColors.Red, palette.GetColor(0));
        Assert.Equal(SKColors.Blue, palette.GetColor(1));
    }

    [Fact]
    public void ColorPalette_GetColor_ShouldWrapAround()
    {
        // Arrange
        var palette = new ColorPalette(
            "Test",
            ColorPaletteType.Categorical,
            SKColors.Red, SKColors.Blue);

        // Act & Assert
        Assert.Equal(SKColors.Red, palette.GetColor(2)); // Wraps to index 0
        Assert.Equal(SKColors.Blue, palette.GetColor(3)); // Wraps to index 1
    }

    [Fact]
    public void ColorPalette_GetColor_EmptyPalette_ShouldReturnGray()
    {
        // Arrange
        var palette = new ColorPalette("Empty", ColorPaletteType.Categorical);

        // Act
        var color = palette.GetColor(0);

        // Assert
        Assert.Equal(SKColors.Gray, color);
    }

    [Fact]
    public void ColorPalette_GetInterpolatedColor_ShouldInterpolate()
    {
        // Arrange
        var palette = new ColorPalette(
            "Test",
            ColorPaletteType.Sequential,
            SKColors.Black, SKColors.White);

        // Act
        var startColor = palette.GetInterpolatedColor(0.0);
        var midColor = palette.GetInterpolatedColor(0.5);
        var endColor = palette.GetInterpolatedColor(1.0);

        // Assert
        Assert.Equal(SKColors.Black, startColor);
        Assert.Equal(SKColors.White, endColor);
        // Mid color should be gray (128, 128, 128, 255)
        Assert.True(midColor.Red > 100 && midColor.Red < 155);
        Assert.True(midColor.Green > 100 && midColor.Green < 155);
        Assert.True(midColor.Blue > 100 && midColor.Blue < 155);
    }

    [Fact]
    public void ColorPalette_GetInterpolatedColor_Clamping_ShouldWork()
    {
        // Arrange
        var palette = new ColorPalette(
            "Test",
            ColorPaletteType.Sequential,
            SKColors.Black, SKColors.White);

        // Act
        var belowZero = palette.GetInterpolatedColor(-0.5);
        var aboveOne = palette.GetInterpolatedColor(1.5);

        // Assert
        Assert.Equal(SKColors.Black, belowZero);
        Assert.Equal(SKColors.White, aboveOne);
    }

    [Fact]
    public void ColorPalette_Clone_ShouldCreateCopy()
    {
        // Arrange
        var original = new ColorPalette(
            "Test",
            ColorPaletteType.Categorical,
            SKColors.Red);

        // Act
        var clone = original.Clone();

        // Assert
        Assert.Equal(original.Name, clone.Name);
        Assert.Equal(original.Type, clone.Type);
        Assert.Equal(original.Colors.Count, clone.Colors.Count);
    }

    // Predefined Palettes Tests
    [Fact]
    public void ColorPalettes_Default_ShouldHave8Colors()
    {
        // Arrange & Act
        var palette = ColorPalettes.Default;

        // Assert
        Assert.Equal("Default", palette.Name);
        Assert.Equal(ColorPaletteType.Categorical, palette.Type);
        Assert.Equal(8, palette.Colors.Count);
    }

    [Fact]
    public void ColorPalettes_Vibrant_ShouldHave10Colors()
    {
        // Arrange & Act
        var palette = ColorPalettes.Vibrant;

        // Assert
        Assert.Equal("Vibrant", palette.Name);
        Assert.Equal(ColorPaletteType.Categorical, palette.Type);
        Assert.Equal(10, palette.Colors.Count);
    }

    [Fact]
    public void ColorPalettes_Pastel_ShouldHave8Colors()
    {
        // Arrange & Act
        var palette = ColorPalettes.Pastel;

        // Assert
        Assert.Equal("Pastel", palette.Name);
        Assert.Equal(ColorPaletteType.Categorical, palette.Type);
        Assert.Equal(8, palette.Colors.Count);
    }

    [Fact]
    public void ColorPalettes_Professional_ShouldHave6Colors()
    {
        // Arrange & Act
        var palette = ColorPalettes.Professional;

        // Assert
        Assert.Equal("Professional", palette.Name);
        Assert.Equal(ColorPaletteType.Categorical, palette.Type);
        Assert.Equal(6, palette.Colors.Count);
    }

    [Fact]
    public void ColorPalettes_BluesSequential_ShouldBeSequential()
    {
        // Arrange & Act
        var palette = ColorPalettes.BluesSequential;

        // Assert
        Assert.Equal("Blues", palette.Name);
        Assert.Equal(ColorPaletteType.Sequential, palette.Type);
        Assert.Equal(8, palette.Colors.Count);
    }

    [Fact]
    public void ColorPalettes_GreensSequential_ShouldBeSequential()
    {
        // Arrange & Act
        var palette = ColorPalettes.GreensSequential;

        // Assert
        Assert.Equal("Greens", palette.Name);
        Assert.Equal(ColorPaletteType.Sequential, palette.Type);
        Assert.Equal(8, palette.Colors.Count);
    }

    [Fact]
    public void ColorPalettes_RedsSequential_ShouldBeSequential()
    {
        // Arrange & Act
        var palette = ColorPalettes.RedsSequential;

        // Assert
        Assert.Equal("Reds", palette.Name);
        Assert.Equal(ColorPaletteType.Sequential, palette.Type);
        Assert.Equal(8, palette.Colors.Count);
    }

    [Fact]
    public void ColorPalettes_Heat_ShouldBeSequential()
    {
        // Arrange & Act
        var palette = ColorPalettes.Heat;

        // Assert
        Assert.Equal("Heat", palette.Name);
        Assert.Equal(ColorPaletteType.Sequential, palette.Type);
        Assert.Equal(8, palette.Colors.Count);
    }

    [Fact]
    public void ColorPalettes_RedBlue_ShouldBeDiverging()
    {
        // Arrange & Act
        var palette = ColorPalettes.RedBlue;

        // Assert
        Assert.Equal("Red-Blue", palette.Name);
        Assert.Equal(ColorPaletteType.Diverging, palette.Type);
        Assert.Equal(9, palette.Colors.Count);
    }

    [Fact]
    public void ColorPalettes_PurpleGreen_ShouldBeDiverging()
    {
        // Arrange & Act
        var palette = ColorPalettes.PurpleGreen;

        // Assert
        Assert.Equal("Purple-Green", palette.Name);
        Assert.Equal(ColorPaletteType.Diverging, palette.Type);
        Assert.Equal(9, palette.Colors.Count);
    }

    [Fact]
    public void ColorPalettes_BrownTeal_ShouldBeDiverging()
    {
        // Arrange & Act
        var palette = ColorPalettes.BrownTeal;

        // Assert
        Assert.Equal("Brown-Teal", palette.Name);
        Assert.Equal(ColorPaletteType.Diverging, palette.Type);
        Assert.Equal(9, palette.Colors.Count);
    }

    [Fact]
    public void ColorPalettes_Spectral_ShouldBeDiverging()
    {
        // Arrange & Act
        var palette = ColorPalettes.Spectral;

        // Assert
        Assert.Equal("Spectral", palette.Name);
        Assert.Equal(ColorPaletteType.Diverging, palette.Type);
        Assert.Equal(11, palette.Colors.Count);
    }

    [Fact]
    public void ColorPalettes_All_ShouldReturn12Palettes()
    {
        // Arrange & Act
        var palettes = ColorPalettes.All.ToList();

        // Assert
        Assert.Equal(12, palettes.Count);
    }

    [Fact]
    public void ColorPalettes_Categorical_ShouldReturn4Palettes()
    {
        // Arrange & Act
        var palettes = ColorPalettes.Categorical.ToList();

        // Assert
        Assert.Equal(4, palettes.Count);
        Assert.All(palettes, p => Assert.Equal(ColorPaletteType.Categorical, p.Type));
    }

    [Fact]
    public void ColorPalettes_Sequential_ShouldReturn4Palettes()
    {
        // Arrange & Act
        var palettes = ColorPalettes.Sequential.ToList();

        // Assert
        Assert.Equal(4, palettes.Count);
        Assert.All(palettes, p => Assert.Equal(ColorPaletteType.Sequential, p.Type));
    }

    [Fact]
    public void ColorPalettes_Diverging_ShouldReturn4Palettes()
    {
        // Arrange & Act
        var palettes = ColorPalettes.Diverging.ToList();

        // Assert
        Assert.Equal(4, palettes.Count);
        Assert.All(palettes, p => Assert.Equal(ColorPaletteType.Diverging, p.Type));
    }

    // Style Components Tests
    [Fact]
    public void BackgroundStyle_Clone_ShouldWork()
    {
        // Arrange
        var original = new BackgroundStyle
        {
            Color = SKColors.Red,
            BorderColor = SKColors.Blue,
            BorderWidth = 2.0f,
            CornerRadius = 5.0f
        };

        // Act
        var clone = original.Clone();

        // Assert
        Assert.Equal(original.Color, clone.Color);
        Assert.Equal(original.BorderColor, clone.BorderColor);
        Assert.Equal(original.BorderWidth, clone.BorderWidth);
        Assert.Equal(original.CornerRadius, clone.CornerRadius);
    }

    [Fact]
    public void GridStyle_Clone_ShouldWork()
    {
        // Arrange
        var original = new GridStyle
        {
            ShowMajorGrid = false,
            MajorGridColor = SKColors.Red
        };

        // Act
        var clone = original.Clone();

        // Assert
        Assert.Equal(original.ShowMajorGrid, clone.ShowMajorGrid);
        Assert.Equal(original.MajorGridColor, clone.MajorGridColor);
    }

    [Fact]
    public void AxisStyle_Clone_ShouldWork()
    {
        // Arrange
        var original = new AxisStyle
        {
            LineColor = SKColors.Red,
            LineWidth = 3.0f
        };

        // Act
        var clone = original.Clone();

        // Assert
        Assert.Equal(original.LineColor, clone.LineColor);
        Assert.Equal(original.LineWidth, clone.LineWidth);
    }

    [Fact]
    public void SeriesStyle_Clone_ShouldWork()
    {
        // Arrange
        var original = new SeriesStyle
        {
            LineWidth = 3.0f,
            MarkerSize = 10.0f
        };

        // Act
        var clone = original.Clone();

        // Assert
        Assert.Equal(original.LineWidth, clone.LineWidth);
        Assert.Equal(original.MarkerSize, clone.MarkerSize);
    }

    [Fact]
    public void LegendStyle_Clone_ShouldWork()
    {
        // Arrange
        var original = new LegendStyle
        {
            BackgroundColor = SKColors.Red,
            FontSize = 15.0f
        };

        // Act
        var clone = original.Clone();

        // Assert
        Assert.Equal(original.BackgroundColor, clone.BackgroundColor);
        Assert.Equal(original.FontSize, clone.FontSize);
    }

    [Fact]
    public void TooltipStyle_Clone_ShouldWork()
    {
        // Arrange
        var original = new TooltipStyle
        {
            BackgroundColor = SKColors.Red,
            CornerRadius = 10.0f
        };

        // Act
        var clone = original.Clone();

        // Assert
        Assert.Equal(original.BackgroundColor, clone.BackgroundColor);
        Assert.Equal(original.CornerRadius, clone.CornerRadius);
    }

    [Fact]
    public void TitleStyle_Clone_ShouldWork()
    {
        // Arrange
        var original = new TitleStyle
        {
            Color = SKColors.Red,
            FontSize = 20.0f
        };

        // Act
        var clone = original.Clone();

        // Assert
        Assert.Equal(original.Color, clone.Color);
        Assert.Equal(original.FontSize, clone.FontSize);
    }

    [Fact]
    public void FontStyle_Clone_ShouldWork()
    {
        // Arrange
        var original = new FontStyle
        {
            DefaultFontFamily = "Helvetica",
            TitleFontFamily = "Times"
        };

        // Act
        var clone = original.Clone();

        // Assert
        Assert.Equal(original.DefaultFontFamily, clone.DefaultFontFamily);
        Assert.Equal(original.TitleFontFamily, clone.TitleFontFamily);
    }

    // Integration Tests
    [Fact]
    public void Theme_CanBeMergedWithCustomizations()
    {
        // Arrange
        var baseTheme = ThemePresets.Light.Clone();
        var customizations = new ChartTheme();
        customizations.Background.Color = SKColors.LightGray;
        customizations.Series.LineWidth = 3.0f;

        // Act
        baseTheme.Merge(customizations);

        // Assert
        Assert.Equal(SKColors.LightGray, baseTheme.Background.Color);
        Assert.Equal(3.0f, baseTheme.Series.LineWidth);
        // Other properties should remain from Light theme
        Assert.Equal("Light", baseTheme.Name);
    }

    [Fact]
    public void Theme_ColorPalette_CanBeUsedForSeries()
    {
        // Arrange
        var theme = ThemePresets.Light;
        var palette = theme.ColorPalette;

        // Act & Assert
        for (int i = 0; i < 10; i++)
        {
            var color = palette.GetColor(i);
            Assert.NotEqual(SKColors.Empty, color);
        }
    }

    [Fact]
    public void Theme_SequentialPalette_CanGenerateGradient()
    {
        // Arrange
        var palette = ColorPalettes.Heat;

        // Act
        var colors = new List<SKColor>();
        for (double i = 0; i <= 1.0; i += 0.1)
        {
            colors.Add(palette.GetInterpolatedColor(i));
        }

        // Assert
        Assert.Equal(11, colors.Count);
        Assert.All(colors, c => Assert.NotEqual(SKColors.Empty, c));
        // First should be lightest, last should be darkest (for Heat palette)
        Assert.True(colors[0].Red > colors[^1].Red);
    }
}
