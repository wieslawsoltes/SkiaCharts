using SkiaSharp;

namespace SkiaCharts.Core.Theming;

/// <summary>
/// Predefined theme presets.
/// </summary>
public static class ThemePresets
{
    /// <summary>
    /// Light theme (default white background).
    /// </summary>
    public static ChartTheme Light => new()
    {
        Name = "Light",
        Background = new BackgroundStyle
        {
            Color = SKColors.White,
            BorderColor = new SKColor(200, 200, 200),
            BorderWidth = 1.0f,
            CornerRadius = 0.0f
        },
        Grid = new GridStyle
        {
            ShowMajorGrid = true,
            ShowMinorGrid = false,
            MajorGridColor = new SKColor(220, 220, 220),
            MinorGridColor = new SKColor(240, 240, 240),
            MajorGridWidth = 1.0f,
            MinorGridWidth = 0.5f
        },
        Axis = new AxisStyle
        {
            LineColor = new SKColor(100, 100, 100),
            LineWidth = 1.5f,
            LabelColor = new SKColor(70, 70, 70),
            LabelFontSize = 11.0f,
            TitleColor = SKColors.Black,
            TitleFontSize = 12.0f,
            TickColor = new SKColor(100, 100, 100),
            TickLength = 5.0f,
            TickWidth = 1.0f
        },
        Series = new SeriesStyle
        {
            LineWidth = 2.0f,
            MarkerSize = 6.0f,
            AreaOpacity = 0.3f,
            ShowMarkers = true,
            EnableAntiAlias = true
        },
        Legend = new LegendStyle
        {
            BackgroundColor = new SKColor(255, 255, 255, 230),
            BorderColor = new SKColor(200, 200, 200),
            BorderWidth = 1.0f,
            TextColor = SKColors.Black,
            FontSize = 11.0f,
            Padding = 10.0f,
            CornerRadius = 4.0f
        },
        Tooltip = new TooltipStyle
        {
            BackgroundColor = new SKColor(50, 50, 50, 240),
            TextColor = SKColors.White,
            FontSize = 11.0f,
            Padding = 8.0f,
            CornerRadius = 4.0f,
            BorderColor = new SKColor(100, 100, 100),
            BorderWidth = 1.0f
        },
        Title = new TitleStyle
        {
            Color = SKColors.Black,
            FontSize = 16.0f,
            FontStyle = SKFontStyle.Bold,
            SubtitleColor = new SKColor(100, 100, 100),
            SubtitleFontSize = 12.0f
        },
        Fonts = new FontStyle
        {
            DefaultFontFamily = "Arial",
            TitleFontFamily = "Arial",
            LabelFontFamily = "Arial",
            MonospaceFontFamily = "Courier New"
        },
        ColorPalette = ColorPalettes.Default
    };

    /// <summary>
    /// Dark theme (dark background for low-light environments).
    /// </summary>
    public static ChartTheme Dark => new()
    {
        Name = "Dark",
        Background = new BackgroundStyle
        {
            Color = new SKColor(30, 30, 30),
            BorderColor = new SKColor(80, 80, 80),
            BorderWidth = 1.0f,
            CornerRadius = 0.0f
        },
        Grid = new GridStyle
        {
            ShowMajorGrid = true,
            ShowMinorGrid = false,
            MajorGridColor = new SKColor(60, 60, 60),
            MinorGridColor = new SKColor(50, 50, 50),
            MajorGridWidth = 1.0f,
            MinorGridWidth = 0.5f
        },
        Axis = new AxisStyle
        {
            LineColor = new SKColor(150, 150, 150),
            LineWidth = 1.5f,
            LabelColor = new SKColor(200, 200, 200),
            LabelFontSize = 11.0f,
            TitleColor = new SKColor(220, 220, 220),
            TitleFontSize = 12.0f,
            TickColor = new SKColor(150, 150, 150),
            TickLength = 5.0f,
            TickWidth = 1.0f
        },
        Series = new SeriesStyle
        {
            LineWidth = 2.0f,
            MarkerSize = 6.0f,
            AreaOpacity = 0.4f,
            ShowMarkers = true,
            EnableAntiAlias = true
        },
        Legend = new LegendStyle
        {
            BackgroundColor = new SKColor(50, 50, 50, 230),
            BorderColor = new SKColor(100, 100, 100),
            BorderWidth = 1.0f,
            TextColor = new SKColor(220, 220, 220),
            FontSize = 11.0f,
            Padding = 10.0f,
            CornerRadius = 4.0f
        },
        Tooltip = new TooltipStyle
        {
            BackgroundColor = new SKColor(240, 240, 240, 240),
            TextColor = new SKColor(30, 30, 30),
            FontSize = 11.0f,
            Padding = 8.0f,
            CornerRadius = 4.0f,
            BorderColor = new SKColor(200, 200, 200),
            BorderWidth = 1.0f
        },
        Title = new TitleStyle
        {
            Color = new SKColor(220, 220, 220),
            FontSize = 16.0f,
            FontStyle = SKFontStyle.Bold,
            SubtitleColor = new SKColor(180, 180, 180),
            SubtitleFontSize = 12.0f
        },
        Fonts = new FontStyle
        {
            DefaultFontFamily = "Arial",
            TitleFontFamily = "Arial",
            LabelFontFamily = "Arial",
            MonospaceFontFamily = "Courier New"
        },
        ColorPalette = ColorPalettes.Vibrant
    };

    /// <summary>
    /// High contrast theme (for accessibility).
    /// </summary>
    public static ChartTheme HighContrast => new()
    {
        Name = "High Contrast",
        Background = new BackgroundStyle
        {
            Color = SKColors.White,
            BorderColor = SKColors.Black,
            BorderWidth = 3.0f,
            CornerRadius = 0.0f
        },
        Grid = new GridStyle
        {
            ShowMajorGrid = true,
            ShowMinorGrid = false,
            MajorGridColor = new SKColor(100, 100, 100),
            MinorGridColor = new SKColor(150, 150, 150),
            MajorGridWidth = 2.0f,
            MinorGridWidth = 1.0f
        },
        Axis = new AxisStyle
        {
            LineColor = SKColors.Black,
            LineWidth = 3.0f,
            LabelColor = SKColors.Black,
            LabelFontSize = 12.0f,
            TitleColor = SKColors.Black,
            TitleFontSize = 14.0f,
            TickColor = SKColors.Black,
            TickLength = 8.0f,
            TickWidth = 2.0f
        },
        Series = new SeriesStyle
        {
            LineWidth = 3.0f,
            MarkerSize = 8.0f,
            AreaOpacity = 0.5f,
            ShowMarkers = true,
            EnableAntiAlias = true
        },
        Legend = new LegendStyle
        {
            BackgroundColor = SKColors.White,
            BorderColor = SKColors.Black,
            BorderWidth = 2.0f,
            TextColor = SKColors.Black,
            FontSize = 12.0f,
            Padding = 12.0f,
            CornerRadius = 0.0f
        },
        Tooltip = new TooltipStyle
        {
            BackgroundColor = SKColors.Black,
            TextColor = SKColors.White,
            FontSize = 12.0f,
            Padding = 10.0f,
            CornerRadius = 0.0f,
            BorderColor = SKColors.White,
            BorderWidth = 2.0f
        },
        Title = new TitleStyle
        {
            Color = SKColors.Black,
            FontSize = 18.0f,
            FontStyle = SKFontStyle.Bold,
            SubtitleColor = SKColors.Black,
            SubtitleFontSize = 14.0f
        },
        Fonts = new FontStyle
        {
            DefaultFontFamily = "Arial",
            TitleFontFamily = "Arial",
            LabelFontFamily = "Arial",
            MonospaceFontFamily = "Courier New"
        },
        ColorPalette = new ColorPalette(
            "High Contrast",
            ColorPaletteType.Categorical,
            SKColors.Black,
            new SKColor(0, 0, 200),      // Dark Blue
            new SKColor(200, 0, 0),      // Dark Red
            new SKColor(0, 150, 0),      // Dark Green
            new SKColor(200, 100, 0),    // Dark Orange
            new SKColor(150, 0, 150),    // Dark Purple
            new SKColor(100, 100, 100)   // Dark Gray
        )
    };

    /// <summary>
    /// Professional/Business theme (corporate colors).
    /// </summary>
    public static ChartTheme Professional => new()
    {
        Name = "Professional",
        Background = new BackgroundStyle
        {
            Color = new SKColor(248, 248, 248),
            BorderColor = new SKColor(180, 180, 180),
            BorderWidth = 1.0f,
            CornerRadius = 2.0f
        },
        Grid = new GridStyle
        {
            ShowMajorGrid = true,
            ShowMinorGrid = true,
            MajorGridColor = new SKColor(210, 210, 210),
            MinorGridColor = new SKColor(230, 230, 230),
            MajorGridWidth = 1.0f,
            MinorGridWidth = 0.5f
        },
        Axis = new AxisStyle
        {
            LineColor = new SKColor(80, 80, 80),
            LineWidth = 1.5f,
            LabelColor = new SKColor(60, 60, 60),
            LabelFontSize = 10.0f,
            TitleColor = new SKColor(40, 40, 40),
            TitleFontSize = 11.0f,
            TickColor = new SKColor(80, 80, 80),
            TickLength = 4.0f,
            TickWidth = 1.0f
        },
        Series = new SeriesStyle
        {
            LineWidth = 2.5f,
            MarkerSize = 5.0f,
            AreaOpacity = 0.25f,
            ShowMarkers = false,
            EnableAntiAlias = true
        },
        Legend = new LegendStyle
        {
            BackgroundColor = new SKColor(255, 255, 255, 250),
            BorderColor = new SKColor(180, 180, 180),
            BorderWidth = 1.0f,
            TextColor = new SKColor(60, 60, 60),
            FontSize = 10.0f,
            Padding = 12.0f,
            CornerRadius = 2.0f
        },
        Tooltip = new TooltipStyle
        {
            BackgroundColor = new SKColor(40, 40, 40, 245),
            TextColor = SKColors.White,
            FontSize = 10.0f,
            Padding = 10.0f,
            CornerRadius = 2.0f,
            BorderColor = new SKColor(80, 80, 80),
            BorderWidth = 1.0f
        },
        Title = new TitleStyle
        {
            Color = new SKColor(40, 40, 40),
            FontSize = 15.0f,
            FontStyle = SKFontStyle.Bold,
            SubtitleColor = new SKColor(100, 100, 100),
            SubtitleFontSize = 11.0f
        },
        Fonts = new FontStyle
        {
            DefaultFontFamily = "Segoe UI",
            TitleFontFamily = "Segoe UI",
            LabelFontFamily = "Segoe UI",
            MonospaceFontFamily = "Consolas"
        },
        ColorPalette = ColorPalettes.Professional
    };

    /// <summary>
    /// Gets all available theme presets.
    /// </summary>
    public static IEnumerable<ChartTheme> All => new[]
    {
        Light, Dark, HighContrast, Professional
    };

    /// <summary>
    /// Gets a theme preset by name (case-insensitive).
    /// </summary>
    public static ChartTheme? GetByName(string name)
    {
        return All.FirstOrDefault(t =>
            t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
