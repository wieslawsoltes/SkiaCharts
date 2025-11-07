using SkiaSharp;

namespace SkiaCharts.Core.Theming;

/// <summary>
/// Export-friendly themes optimized for printing and static exports.
/// </summary>
public static class PrintThemes
{
    /// <summary>
    /// Print theme with high contrast and thick lines (optimized for B&amp;W printing).
    /// </summary>
    public static ChartTheme Print => new()
    {
        Name = "Print",
        Background = new BackgroundStyle
        {
            Color = SKColors.White,
            BorderColor = SKColors.Black,
            BorderWidth = 2.0f,
            CornerRadius = 0.0f
        },
        Grid = new GridStyle
        {
            ShowMajorGrid = true,
            ShowMinorGrid = false,
            MajorGridColor = new SKColor(150, 150, 150),
            MinorGridColor = new SKColor(200, 200, 200),
            MajorGridWidth = 1.5f,
            MinorGridWidth = 1.0f,
            MajorGridDashPattern = null, // Solid lines for print
            MinorGridDashPattern = null
        },
        Axis = new AxisStyle
        {
            LineColor = SKColors.Black,
            LineWidth = 2.0f,
            LabelColor = SKColors.Black,
            LabelFontSize = 12.0f,
            TitleColor = SKColors.Black,
            TitleFontSize = 14.0f,
            TickColor = SKColors.Black,
            TickLength = 6.0f,
            TickWidth = 2.0f
        },
        Series = new SeriesStyle
        {
            LineWidth = 3.0f,
            MarkerSize = 7.0f,
            AreaOpacity = 0.2f, // Lower opacity for print
            ShowMarkers = true,
            EnableAntiAlias = true
        },
        Legend = new LegendStyle
        {
            BackgroundColor = SKColors.White,
            BorderColor = SKColors.Black,
            BorderWidth = 1.5f,
            TextColor = SKColors.Black,
            FontSize = 11.0f,
            Padding = 12.0f,
            CornerRadius = 0.0f
        },
        Tooltip = new TooltipStyle
        {
            BackgroundColor = SKColors.White,
            TextColor = SKColors.Black,
            FontSize = 11.0f,
            Padding = 8.0f,
            CornerRadius = 0.0f,
            BorderColor = SKColors.Black,
            BorderWidth = 1.5f
        },
        Title = new TitleStyle
        {
            Color = SKColors.Black,
            FontSize = 18.0f,
            FontStyle = SKFontStyle.Bold,
            SubtitleColor = new SKColor(60, 60, 60),
            SubtitleFontSize = 14.0f
        },
        Fonts = new FontStyle
        {
            DefaultFontFamily = "Times New Roman",
            TitleFontFamily = "Times New Roman",
            LabelFontFamily = "Times New Roman",
            MonospaceFontFamily = "Courier New"
        },
        ColorPalette = new ColorPalette(
            "Print",
            ColorPaletteType.Categorical,
            SKColors.Black,
            new SKColor(80, 80, 80),
            new SKColor(120, 120, 120),
            new SKColor(160, 160, 160),
            new SKColor(40, 40, 40),
            new SKColor(100, 100, 100)
        )
    };

    /// <summary>
    /// Grayscale theme (for B&amp;W printing without color).
    /// </summary>
    public static ChartTheme Grayscale => new()
    {
        Name = "Grayscale",
        Background = new BackgroundStyle
        {
            Color = SKColors.White,
            BorderColor = new SKColor(100, 100, 100),
            BorderWidth = 1.5f,
            CornerRadius = 0.0f
        },
        Grid = new GridStyle
        {
            ShowMajorGrid = true,
            ShowMinorGrid = false,
            MajorGridColor = new SKColor(180, 180, 180),
            MinorGridColor = new SKColor(220, 220, 220),
            MajorGridWidth = 1.0f,
            MinorGridWidth = 0.5f
        },
        Axis = new AxisStyle
        {
            LineColor = new SKColor(80, 80, 80),
            LineWidth = 1.5f,
            LabelColor = new SKColor(60, 60, 60),
            LabelFontSize = 11.0f,
            TitleColor = SKColors.Black,
            TitleFontSize = 12.0f,
            TickColor = new SKColor(80, 80, 80),
            TickLength = 5.0f,
            TickWidth = 1.0f
        },
        Series = new SeriesStyle
        {
            LineWidth = 2.5f,
            MarkerSize = 6.0f,
            AreaOpacity = 0.15f,
            ShowMarkers = true,
            EnableAntiAlias = true
        },
        Legend = new LegendStyle
        {
            BackgroundColor = SKColors.White,
            BorderColor = new SKColor(120, 120, 120),
            BorderWidth = 1.0f,
            TextColor = SKColors.Black,
            FontSize = 10.0f,
            Padding = 10.0f,
            CornerRadius = 0.0f
        },
        Tooltip = new TooltipStyle
        {
            BackgroundColor = new SKColor(240, 240, 240),
            TextColor = SKColors.Black,
            FontSize = 10.0f,
            Padding = 8.0f,
            CornerRadius = 0.0f,
            BorderColor = new SKColor(120, 120, 120),
            BorderWidth = 1.0f
        },
        Title = new TitleStyle
        {
            Color = SKColors.Black,
            FontSize = 16.0f,
            FontStyle = SKFontStyle.Bold,
            SubtitleColor = new SKColor(80, 80, 80),
            SubtitleFontSize = 12.0f
        },
        Fonts = new FontStyle
        {
            DefaultFontFamily = "Arial",
            TitleFontFamily = "Arial",
            LabelFontFamily = "Arial",
            MonospaceFontFamily = "Courier New"
        },
        ColorPalette = new ColorPalette(
            "Grayscale",
            ColorPaletteType.Categorical,
            new SKColor(0, 0, 0),         // Black
            new SKColor(60, 60, 60),      // Dark Gray
            new SKColor(100, 100, 100),   // Medium Gray
            new SKColor(140, 140, 140),   // Light Gray
            new SKColor(180, 180, 180),   // Very Light Gray
            new SKColor(220, 220, 220)    // Almost White
        )
    };

    /// <summary>
    /// High-DPI print theme (optimized for 300+ DPI printing).
    /// </summary>
    public static ChartTheme HighDpi => new()
    {
        Name = "High DPI",
        Background = new BackgroundStyle
        {
            Color = SKColors.White,
            BorderColor = new SKColor(180, 180, 180),
            BorderWidth = 1.0f,
            CornerRadius = 0.0f
        },
        Grid = new GridStyle
        {
            ShowMajorGrid = true,
            ShowMinorGrid = true,
            MajorGridColor = new SKColor(200, 200, 200),
            MinorGridColor = new SKColor(230, 230, 230),
            MajorGridWidth = 0.75f, // Thinner for high DPI
            MinorGridWidth = 0.5f
        },
        Axis = new AxisStyle
        {
            LineColor = new SKColor(60, 60, 60),
            LineWidth = 1.0f,
            LabelColor = new SKColor(40, 40, 40),
            LabelFontSize = 9.0f, // Smaller for high DPI
            TitleColor = SKColors.Black,
            TitleFontSize = 10.0f,
            TickColor = new SKColor(60, 60, 60),
            TickLength = 4.0f,
            TickWidth = 0.75f
        },
        Series = new SeriesStyle
        {
            LineWidth = 2.0f,
            MarkerSize = 5.0f,
            AreaOpacity = 0.25f,
            ShowMarkers = true,
            EnableAntiAlias = true
        },
        Legend = new LegendStyle
        {
            BackgroundColor = SKColors.White,
            BorderColor = new SKColor(160, 160, 160),
            BorderWidth = 0.75f,
            TextColor = SKColors.Black,
            FontSize = 9.0f,
            Padding = 8.0f,
            CornerRadius = 0.0f
        },
        Tooltip = new TooltipStyle
        {
            BackgroundColor = new SKColor(250, 250, 250),
            TextColor = SKColors.Black,
            FontSize = 9.0f,
            Padding = 6.0f,
            CornerRadius = 0.0f,
            BorderColor = new SKColor(160, 160, 160),
            BorderWidth = 0.75f
        },
        Title = new TitleStyle
        {
            Color = SKColors.Black,
            FontSize = 14.0f,
            FontStyle = SKFontStyle.Bold,
            SubtitleColor = new SKColor(80, 80, 80),
            SubtitleFontSize = 10.0f
        },
        Fonts = new FontStyle
        {
            DefaultFontFamily = "Arial",
            TitleFontFamily = "Arial",
            LabelFontFamily = "Arial",
            MonospaceFontFamily = "Courier New"
        },
        ColorPalette = ColorPalettes.Professional
    };

    /// <summary>
    /// Publication theme (optimized for academic papers and journals).
    /// </summary>
    public static ChartTheme Publication => new()
    {
        Name = "Publication",
        Background = new BackgroundStyle
        {
            Color = SKColors.White,
            BorderColor = SKColors.Black,
            BorderWidth = 1.0f,
            CornerRadius = 0.0f
        },
        Grid = new GridStyle
        {
            ShowMajorGrid = true,
            ShowMinorGrid = false,
            MajorGridColor = new SKColor(200, 200, 200),
            MinorGridColor = new SKColor(230, 230, 230),
            MajorGridWidth = 0.75f,
            MinorGridWidth = 0.5f
        },
        Axis = new AxisStyle
        {
            LineColor = SKColors.Black,
            LineWidth = 1.25f,
            LabelColor = SKColors.Black,
            LabelFontSize = 10.0f,
            TitleColor = SKColors.Black,
            TitleFontSize = 11.0f,
            TickColor = SKColors.Black,
            TickLength = 4.0f,
            TickWidth = 1.0f
        },
        Series = new SeriesStyle
        {
            LineWidth = 2.0f,
            MarkerSize = 6.0f,
            AreaOpacity = 0.2f,
            ShowMarkers = true,
            EnableAntiAlias = true
        },
        Legend = new LegendStyle
        {
            BackgroundColor = SKColors.Transparent,
            BorderColor = SKColors.Black,
            BorderWidth = 0.75f,
            TextColor = SKColors.Black,
            FontSize = 9.0f,
            Padding = 6.0f,
            CornerRadius = 0.0f
        },
        Tooltip = new TooltipStyle
        {
            BackgroundColor = SKColors.White,
            TextColor = SKColors.Black,
            FontSize = 9.0f,
            Padding = 6.0f,
            CornerRadius = 0.0f,
            BorderColor = SKColors.Black,
            BorderWidth = 1.0f
        },
        Title = new TitleStyle
        {
            Color = SKColors.Black,
            FontSize = 13.0f,
            FontStyle = SKFontStyle.Bold,
            SubtitleColor = SKColors.Black,
            SubtitleFontSize = 10.0f
        },
        Fonts = new FontStyle
        {
            DefaultFontFamily = "Times New Roman",
            TitleFontFamily = "Times New Roman",
            LabelFontFamily = "Times New Roman",
            MonospaceFontFamily = "Courier New"
        },
        ColorPalette = new ColorPalette(
            "Publication",
            ColorPaletteType.Categorical,
            SKColors.Black,
            new SKColor(100, 100, 100),
            new SKColor(150, 150, 150),
            new SKColor(200, 200, 200),
            new SKColor(50, 50, 50),
            new SKColor(180, 180, 180)
        )
    };

    /// <summary>
    /// Pattern-fill friendly theme (uses different patterns instead of colors for accessibility).
    /// </summary>
    public static ChartTheme PatternFill => new()
    {
        Name = "Pattern Fill",
        Background = new BackgroundStyle
        {
            Color = SKColors.White,
            BorderColor = SKColors.Black,
            BorderWidth = 1.5f,
            CornerRadius = 0.0f
        },
        Grid = new GridStyle
        {
            ShowMajorGrid = true,
            ShowMinorGrid = false,
            MajorGridColor = new SKColor(180, 180, 180),
            MinorGridColor = new SKColor(220, 220, 220),
            MajorGridWidth = 1.0f,
            MinorGridWidth = 0.5f
        },
        Axis = new AxisStyle
        {
            LineColor = SKColors.Black,
            LineWidth = 2.0f,
            LabelColor = SKColors.Black,
            LabelFontSize = 11.0f,
            TitleColor = SKColors.Black,
            TitleFontSize = 12.0f,
            TickColor = SKColors.Black,
            TickLength = 5.0f,
            TickWidth = 1.5f
        },
        Series = new SeriesStyle
        {
            LineWidth = 2.5f,
            MarkerSize = 7.0f,
            AreaOpacity = 0.3f,
            ShowMarkers = true,
            EnableAntiAlias = true
        },
        Legend = new LegendStyle
        {
            BackgroundColor = SKColors.White,
            BorderColor = SKColors.Black,
            BorderWidth = 1.5f,
            TextColor = SKColors.Black,
            FontSize = 11.0f,
            Padding = 10.0f,
            CornerRadius = 0.0f
        },
        Tooltip = new TooltipStyle
        {
            BackgroundColor = SKColors.White,
            TextColor = SKColors.Black,
            FontSize = 11.0f,
            Padding = 8.0f,
            CornerRadius = 0.0f,
            BorderColor = SKColors.Black,
            BorderWidth = 1.5f
        },
        Title = new TitleStyle
        {
            Color = SKColors.Black,
            FontSize = 16.0f,
            FontStyle = SKFontStyle.Bold,
            SubtitleColor = new SKColor(80, 80, 80),
            SubtitleFontSize = 12.0f
        },
        Fonts = new FontStyle
        {
            DefaultFontFamily = "Arial",
            TitleFontFamily = "Arial",
            LabelFontFamily = "Arial",
            MonospaceFontFamily = "Courier New"
        },
        ColorPalette = new ColorPalette(
            "Pattern Fill",
            ColorPaletteType.Categorical,
            SKColors.Black,
            new SKColor(60, 60, 60),
            new SKColor(100, 100, 100),
            new SKColor(140, 140, 140),
            new SKColor(40, 40, 40),
            new SKColor(120, 120, 120)
        )
    };

    /// <summary>
    /// Gets all print-friendly theme presets.
    /// </summary>
    public static IEnumerable<ChartTheme> All => new[]
    {
        Print, Grayscale, HighDpi, Publication, PatternFill
    };

    /// <summary>
    /// Gets a print theme by name (case-insensitive).
    /// </summary>
    public static ChartTheme? GetByName(string name)
    {
        return All.FirstOrDefault(t =>
            t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}

/// <summary>
/// Export settings for different output formats.
/// </summary>
public class ExportSettings
{
    /// <summary>
    /// Gets or sets the DPI for export (default: 96 for screen, 300 for print).
    /// </summary>
    public int Dpi { get; set; } = 96;

    /// <summary>
    /// Gets or sets the output format.
    /// </summary>
    public ExportFormat Format { get; set; } = ExportFormat.Png;

    /// <summary>
    /// Gets or sets the quality (0-100) for JPEG export.
    /// </summary>
    public int JpegQuality { get; set; } = 90;

    /// <summary>
    /// Gets or sets whether to use transparent background (PNG only).
    /// </summary>
    public bool TransparentBackground { get; set; } = false;

    /// <summary>
    /// Gets or sets the theme to use for export.
    /// </summary>
    public ChartTheme? ExportTheme { get; set; }

    /// <summary>
    /// Creates export settings optimized for printing.
    /// </summary>
    public static ExportSettings ForPrint() => new()
    {
        Dpi = 300,
        Format = ExportFormat.Pdf,
        ExportTheme = PrintThemes.Print
    };

    /// <summary>
    /// Creates export settings optimized for web.
    /// </summary>
    public static ExportSettings ForWeb() => new()
    {
        Dpi = 96,
        Format = ExportFormat.Png,
        TransparentBackground = false,
        ExportTheme = ThemePresets.Light
    };

    /// <summary>
    /// Creates export settings optimized for publications.
    /// </summary>
    public static ExportSettings ForPublication() => new()
    {
        Dpi = 300,
        Format = ExportFormat.Pdf,
        ExportTheme = PrintThemes.Publication
    };
}

/// <summary>
/// Export format enumeration.
/// </summary>
public enum ExportFormat
{
    /// <summary>PNG image format.</summary>
    Png,
    /// <summary>JPEG image format.</summary>
    Jpeg,
    /// <summary>PDF document format.</summary>
    Pdf,
    /// <summary>SVG vector format.</summary>
    Svg,
    /// <summary>WebP image format.</summary>
    WebP
}
