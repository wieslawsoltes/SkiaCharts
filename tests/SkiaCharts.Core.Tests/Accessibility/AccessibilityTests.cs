using SkiaCharts.Core.Accessibility;
using SkiaCharts.Core.Theming;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Accessibility;

public class AccessibilityTests
{
    #region Colorblind Palette Tests

    [Fact]
    public void ColorblindSafePalette_HasCorrectColors()
    {
        // Arrange & Act
        var palette = AccessibilityPalettes.ColorblindSafe;

        // Assert
        Assert.Equal("Colorblind Safe", palette.Name);
        Assert.Equal(ColorPaletteType.Categorical, palette.Type);
        Assert.Equal(7, palette.Colors.Count);
    }

    [Fact]
    public void AllColorblindPalettes_AreAccessible()
    {
        // Arrange & Act
        var palettes = AccessibilityPalettes.All.ToList();

        // Assert
        Assert.Equal(10, palettes.Count);
        Assert.Contains(palettes, p => p.Name == "Colorblind Safe");
        Assert.Contains(palettes, p => p.Name == "Okabe-Ito");
        Assert.Contains(palettes, p => p.Name == "Wong (Nature)");
    }

    [Fact]
    public void GetForColorblindType_ReturnsCorrectPalette()
    {
        // Assert
        Assert.Equal("Deuteranopia Safe", AccessibilityPalettes.GetForColorblindType(ColorblindType.Deuteranopia).Name);
        Assert.Equal("Protanopia Safe", AccessibilityPalettes.GetForColorblindType(ColorblindType.Protanopia).Name);
        Assert.Equal("Tritanopia Safe", AccessibilityPalettes.GetForColorblindType(ColorblindType.Tritanopia).Name);
    }

    #endregion

    #region Colorblind Simulator Tests

    [Fact]
    public void SimulateDeuteranopia_ChangesColor()
    {
        // Arrange
        var red = new SKColor(255, 0, 0);

        // Act
        var simulated = ColorblindSimulator.SimulateDeuteranopia(red);

        // Assert - Red should appear different (more brownish) to deuteranopes
        Assert.NotEqual(red, simulated);
    }

    [Fact]
    public void SimulateProtanopia_ChangesColor()
    {
        // Arrange
        var red = new SKColor(255, 0, 0);

        // Act
        var simulated = ColorblindSimulator.SimulateProtanopia(red);

        // Assert
        Assert.NotEqual(red, simulated);
    }

    [Fact]
    public void SimulateTritanopia_ChangesColor()
    {
        // Arrange - Use yellow which is affected by tritanopia
        var yellow = new SKColor(255, 255, 0);

        // Act
        var simulated = ColorblindSimulator.SimulateTritanopia(yellow);

        // Assert - Yellow appears different to tritanopes
        Assert.NotEqual(yellow, simulated);
    }

    [Fact]
    public void ToGrayscale_RemovesColor()
    {
        // Arrange
        var red = new SKColor(255, 0, 0);

        // Act
        var gray = ColorblindSimulator.ToGrayscale(red);

        // Assert
        Assert.Equal(gray.Red, gray.Green);
        Assert.Equal(gray.Green, gray.Blue);
    }

    [Fact]
    public void AreDistinguishable_DetectsDistinctColors()
    {
        // Arrange
        var blue = new SKColor(0, 114, 178);
        var orange = new SKColor(230, 159, 0);

        // Act & Assert
        Assert.True(ColorblindSimulator.AreDistinguishable(blue, orange, ColorblindType.Deuteranopia));
        Assert.True(ColorblindSimulator.AreDistinguishable(blue, orange, ColorblindType.Protanopia));
        Assert.True(ColorblindSimulator.AreDistinguishable(blue, orange, ColorblindType.Tritanopia));
    }

    #endregion

    #region Pattern Fill Tests

    [Fact]
    public void CreatePattern_GeneratesShader()
    {
        // Act
        using var shader = PatternFills.CreatePattern(
            PatternType.HorizontalLines,
            SKColors.Black,
            SKColors.White);

        // Assert
        Assert.NotNull(shader);
    }

    [Fact]
    public void CategoricalPatterns_HasEightPatterns()
    {
        // Act
        var patterns = PatternPresets.CategoricalPatterns;

        // Assert
        Assert.Equal(8, patterns.Length);
        Assert.Contains(PatternType.Solid, patterns);
        Assert.Contains(PatternType.HorizontalLines, patterns);
        Assert.Contains(PatternType.Checkerboard, patterns);
    }

    [Fact]
    public void GetCategoricalPattern_WrapsAround()
    {
        // Arrange
        var patternsCount = PatternPresets.CategoricalPatterns.Length;

        // Act
        var pattern0 = PatternPresets.GetCategoricalPattern(0);
        var patternWrapped = PatternPresets.GetCategoricalPattern(patternsCount);

        // Assert
        Assert.Equal(pattern0, patternWrapped);
    }

    [Fact]
    public void AccessibilityOptions_ForBlackAndWhite_EnablesPatterns()
    {
        // Act
        var options = AccessibilityOptions.ForBlackAndWhite();

        // Assert
        Assert.True(options.UsePatternFills);
        Assert.True(options.UseHighContrast);
        Assert.True(options.ShowDataLabels);
    }

    #endregion

    #region Contrast Checker Tests

    [Fact]
    public void GetContrastRatio_BlackOnWhite_Returns21()
    {
        // Act
        var ratio = ContrastChecker.GetContrastRatio(SKColors.Black, SKColors.White);

        // Assert
        Assert.InRange(ratio, 20.9, 21.1); // Allow small floating-point error
    }

    [Fact]
    public void GetContrastRatio_WhiteOnWhite_Returns1()
    {
        // Act
        var ratio = ContrastChecker.GetContrastRatio(SKColors.White, SKColors.White);

        // Assert
        Assert.InRange(ratio, 0.9, 1.1);
    }

    [Fact]
    public void MeetsWcagAA_BlackOnWhite_ReturnsTrue()
    {
        // Assert
        Assert.True(ContrastChecker.MeetsWcagAA(SKColors.Black, SKColors.White));
    }

    [Fact]
    public void MeetsWcagAAA_BlackOnWhite_ReturnsTrue()
    {
        // Assert
        Assert.True(ContrastChecker.MeetsWcagAAA(SKColors.Black, SKColors.White));
    }

    [Fact]
    public void MeetsWcagAA_LowContrast_ReturnsFalse()
    {
        // Arrange
        var lightGray = new SKColor(200, 200, 200);
        var white = SKColors.White;

        // Assert
        Assert.False(ContrastChecker.MeetsWcagAA(lightGray, white));
    }

    [Fact]
    public void GetContrastingTextColor_DarkBackground_ReturnsWhite()
    {
        // Arrange
        var darkGray = new SKColor(50, 50, 50);

        // Act
        var textColor = ContrastChecker.GetContrastingTextColor(darkGray);

        // Assert
        Assert.Equal(SKColors.White, textColor);
    }

    [Fact]
    public void GetContrastingTextColor_LightBackground_ReturnsBlack()
    {
        // Arrange
        var lightGray = new SKColor(200, 200, 200);

        // Act
        var textColor = ContrastChecker.GetContrastingTextColor(lightGray);

        // Assert
        Assert.Equal(SKColors.Black, textColor);
    }

    [Fact]
    public void DarkenUntilContrast_ImproveContrast()
    {
        // Arrange
        var gray = new SKColor(150, 150, 150);
        var white = SKColors.White;

        // Act
        var darkened = ContrastChecker.DarkenUntilContrast(gray, white, ContrastChecker.WcagAA);

        // Assert
        var ratio = ContrastChecker.GetContrastRatio(darkened, white);
        Assert.True(ratio >= ContrastChecker.WcagAA);
    }

    [Fact]
    public void ValidateThemeContrast_LightTheme_PassesValidation()
    {
        // Arrange
        var theme = ThemePresets.Light;

        // Act
        var validation = ContrastChecker.ValidateThemeContrast(theme);

        // Assert
        Assert.True(validation.AllPassed);
        Assert.Empty(validation.FailedChecks);
    }

    [Fact]
    public void EnsureAccessibleContrast_FixesLowContrast()
    {
        // Arrange
        var theme = ThemePresets.Light.Clone();
        theme.Axis.LabelColor = new SKColor(240, 240, 240); // Very low contrast

        // Act
        var adjusted = ContrastChecker.EnsureAccessibleContrast(theme);

        // Assert
        var ratio = ContrastChecker.GetContrastRatio(adjusted.Axis.LabelColor, adjusted.Background.Color);
        Assert.True(ratio >= ContrastChecker.WcagAA);
    }

    #endregion

    #region Keyboard Navigation Tests

    [Fact]
    public void KeyboardNavigation_RegisterElement_AddsToList()
    {
        // Arrange
        var nav = new KeyboardNavigation();
        var element = new TestNavigableElement("Element 1");

        // Act
        nav.RegisterElement(element);

        // Assert
        Assert.Single(nav.Elements);
        Assert.Contains(element, nav.Elements);
    }

    [Fact]
    public void KeyboardNavigation_MoveToNext_ChangesFocus()
    {
        // Arrange
        var nav = new KeyboardNavigation();
        var element1 = new TestNavigableElement("Element 1");
        var element2 = new TestNavigableElement("Element 2");
        nav.RegisterElement(element1);
        nav.RegisterElement(element2);

        // Act
        nav.MoveToNext();

        // Assert
        Assert.Equal(element1, nav.FocusedElement);
        Assert.True(element1.IsFocused);
    }

    [Fact]
    public void KeyboardNavigation_MoveToNext_WrapsAround()
    {
        // Arrange
        var nav = new KeyboardNavigation();
        var element1 = new TestNavigableElement("Element 1");
        var element2 = new TestNavigableElement("Element 2");
        nav.RegisterElement(element1);
        nav.RegisterElement(element2);

        // Act
        nav.MoveToNext(); // -> element1
        nav.MoveToNext(); // -> element2
        nav.MoveToNext(); // -> element1 (wrap)

        // Assert
        Assert.Equal(element1, nav.FocusedElement);
    }

    [Fact]
    public void KeyboardNavigation_HandleKey_Tab_MovesNext()
    {
        // Arrange
        var nav = new KeyboardNavigation();
        nav.RegisterElement(new TestNavigableElement("Element 1"));
        nav.RegisterElement(new TestNavigableElement("Element 2"));

        // Act
        var handled = nav.HandleKey(KeyboardKey.Tab, KeyModifiers.None);

        // Assert
        Assert.True(handled);
        Assert.NotNull(nav.FocusedElement);
    }

    [Fact]
    public void KeyboardNavigation_HandleKey_ShiftTab_MovesPrevious()
    {
        // Arrange
        var nav = new KeyboardNavigation();
        nav.RegisterElement(new TestNavigableElement("Element 1"));
        nav.RegisterElement(new TestNavigableElement("Element 2"));
        nav.MoveToLast();

        // Act
        var handled = nav.HandleKey(KeyboardKey.Tab, KeyModifiers.Shift);

        // Assert
        Assert.True(handled);
    }

    [Fact]
    public void KeyboardNavigation_ActivateFocused_CallsActivate()
    {
        // Arrange
        var nav = new KeyboardNavigation();
        var element = new TestNavigableElement("Element 1");
        nav.RegisterElement(element);
        nav.MoveToNext();

        // Act
        nav.ActivateFocused();

        // Assert
        Assert.True(element.WasActivated);
    }

    #endregion

    #region Screen Reader Tests

    [Fact]
    public void GenerateChartDescription_CreatesAccessibleText()
    {
        // Arrange
        var chart = new ChartDescriptor
        {
            ChartType = "Line",
            Title = "Sales Over Time",
            XAxisLabel = "Date",
            YAxisLabel = "Sales ($)",
            SeriesCount = 2,
            TotalDataPoints = 100,
            MinValue = 0,
            MaxValue = 1000
        };

        // Act
        var description = ScreenReaderSupport.GenerateChartDescription(chart);

        // Assert
        Assert.Contains("Line chart", description);
        Assert.Contains("Sales Over Time", description);
        Assert.Contains("X-axis shows Date", description);
        Assert.Contains("2 data series", description);
    }

    [Fact]
    public void GenerateSeriesDescription_CreatesAccessibleText()
    {
        // Arrange
        var series = new SeriesDescriptor
        {
            Name = "Product A",
            DataPointCount = 50,
            MinValue = 10,
            MaxValue = 100,
            Average = 55,
            Trend = "increasing"
        };

        // Act
        var description = ScreenReaderSupport.GenerateSeriesDescription(series);

        // Assert
        Assert.Contains("Product A", description);
        Assert.Contains("50 data points", description);
        Assert.Contains("Average: 55", description);
        Assert.Contains("increasing", description);
    }

    [Fact]
    public void GenerateDataPointDescription_CreatesAccessibleText()
    {
        // Arrange
        var point = new DataPointDescriptor
        {
            Label = "January",
            Value = 500,
            SeriesName = "Sales",
            AdditionalInfo = "Peak month"
        };

        // Act
        var description = ScreenReaderSupport.GenerateDataPointDescription(point);

        // Assert
        Assert.Contains("January", description);
        Assert.Contains("500", description);
        Assert.Contains("Sales", description);
        Assert.Contains("Peak month", description);
    }

    [Fact]
    public void AriaLabelBuilder_ForChart_CreatesCorrectAttributes()
    {
        // Act
        var attributes = AriaLabelBuilder.ForChart("Sales Chart", "Line").Build();

        // Assert
        Assert.Contains("role", attributes.Keys);
        Assert.Contains("aria-label", attributes.Keys);
        Assert.Contains("Line chart", attributes["aria-label"]);
    }

    [Fact]
    public void AccessibilityAnnouncer_Announce_RaisesEvent()
    {
        // Arrange
        var announcer = new AccessibilityAnnouncer();
        var eventRaised = false;
        announcer.AnnouncementRequested += (_, _) => eventRaised = true;

        // Act
        announcer.Announce("Test message", AnnouncementPriority.Assertive);

        // Assert
        Assert.True(eventRaised);
    }

    [Fact]
    public void AccessibilityAnnouncer_Polite_QueuesMessage()
    {
        // Arrange
        var announcer = new AccessibilityAnnouncer();

        // Act
        announcer.Announce("Message 1", AnnouncementPriority.Polite);
        announcer.Announce("Message 2", AnnouncementPriority.Polite);

        // Assert
        Assert.Equal("Message 1", announcer.GetNextAnnouncement());
        Assert.Equal("Message 2", announcer.GetNextAnnouncement());
    }

    #endregion

    #region Helper Classes

    private class TestNavigableElement : NavigableElementBase
    {
        private readonly string _name;
        public bool WasActivated { get; private set; }

        public TestNavigableElement(string name)
        {
            _name = name;
        }

        public override string AccessibleName => _name;

        public override void Activate()
        {
            WasActivated = true;
        }
    }

    #endregion
}
