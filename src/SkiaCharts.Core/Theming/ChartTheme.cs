using SkiaSharp;

namespace SkiaCharts.Core.Theming;

/// <summary>
/// Complete theme definition for charts.
/// </summary>
public class ChartTheme
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartTheme"/> class.
    /// </summary>
    public ChartTheme()
    {
        Name = "Default";
        Background = new BackgroundStyle();
        Grid = new GridStyle();
        Axis = new AxisStyle();
        Series = new SeriesStyle();
        Legend = new LegendStyle();
        Tooltip = new TooltipStyle();
        Title = new TitleStyle();
        Fonts = new FontStyle();
        ColorPalette = ColorPalettes.Default;
    }

    /// <summary>
    /// Gets or sets the theme name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the background style.
    /// </summary>
    public BackgroundStyle Background { get; set; }

    /// <summary>
    /// Gets or sets the grid style.
    /// </summary>
    public GridStyle Grid { get; set; }

    /// <summary>
    /// Gets or sets the axis style.
    /// </summary>
    public AxisStyle Axis { get; set; }

    /// <summary>
    /// Gets or sets the series style.
    /// </summary>
    public SeriesStyle Series { get; set; }

    /// <summary>
    /// Gets or sets the legend style.
    /// </summary>
    public LegendStyle Legend { get; set; }

    /// <summary>
    /// Gets or sets the tooltip style.
    /// </summary>
    public TooltipStyle Tooltip { get; set; }

    /// <summary>
    /// Gets or sets the title style.
    /// </summary>
    public TitleStyle Title { get; set; }

    /// <summary>
    /// Gets or sets the font style.
    /// </summary>
    public FontStyle Fonts { get; set; }

    /// <summary>
    /// Gets or sets the color palette.
    /// </summary>
    public ColorPalette ColorPalette { get; set; }

    /// <summary>
    /// Creates a deep copy of this theme.
    /// </summary>
    public ChartTheme Clone()
    {
        return new ChartTheme
        {
            Name = Name,
            Background = Background.Clone(),
            Grid = Grid.Clone(),
            Axis = Axis.Clone(),
            Series = Series.Clone(),
            Legend = Legend.Clone(),
            Tooltip = Tooltip.Clone(),
            Title = Title.Clone(),
            Fonts = Fonts.Clone(),
            ColorPalette = ColorPalette.Clone()
        };
    }

    /// <summary>
    /// Merges another theme into this theme (CSS-like cascading).
    /// Properties from the other theme override this theme's properties if set.
    /// </summary>
    public void Merge(ChartTheme other)
    {
        if (other == null)
            return;

        Background.Merge(other.Background);
        Grid.Merge(other.Grid);
        Axis.Merge(other.Axis);
        Series.Merge(other.Series);
        Legend.Merge(other.Legend);
        Tooltip.Merge(other.Tooltip);
        Title.Merge(other.Title);
        Fonts.Merge(other.Fonts);

        if (other.ColorPalette != null)
            ColorPalette = other.ColorPalette;
    }
}

/// <summary>
/// Background style settings.
/// </summary>
public class BackgroundStyle
{
    public SKColor Color { get; set; } = SKColors.White;
    public SKColor BorderColor { get; set; } = new SKColor(200, 200, 200);
    public float BorderWidth { get; set; } = 1.0f;
    public float CornerRadius { get; set; } = 0.0f;

    public BackgroundStyle Clone() => new()
    {
        Color = Color,
        BorderColor = BorderColor,
        BorderWidth = BorderWidth,
        CornerRadius = CornerRadius
    };

    public void Merge(BackgroundStyle other)
    {
        if (other == null) return;
        Color = other.Color;
        BorderColor = other.BorderColor;
        BorderWidth = other.BorderWidth;
        CornerRadius = other.CornerRadius;
    }
}

/// <summary>
/// Grid style settings.
/// </summary>
public class GridStyle
{
    public bool ShowMajorGrid { get; set; } = true;
    public bool ShowMinorGrid { get; set; } = false;
    public SKColor MajorGridColor { get; set; } = new SKColor(220, 220, 220);
    public SKColor MinorGridColor { get; set; } = new SKColor(240, 240, 240);
    public float MajorGridWidth { get; set; } = 1.0f;
    public float MinorGridWidth { get; set; } = 0.5f;
    public float[]? MajorGridDashPattern { get; set; } = null;
    public float[]? MinorGridDashPattern { get; set; } = new[] { 2f, 4f };

    public GridStyle Clone() => new()
    {
        ShowMajorGrid = ShowMajorGrid,
        ShowMinorGrid = ShowMinorGrid,
        MajorGridColor = MajorGridColor,
        MinorGridColor = MinorGridColor,
        MajorGridWidth = MajorGridWidth,
        MinorGridWidth = MinorGridWidth,
        MajorGridDashPattern = MajorGridDashPattern,
        MinorGridDashPattern = MinorGridDashPattern
    };

    public void Merge(GridStyle other)
    {
        if (other == null) return;
        ShowMajorGrid = other.ShowMajorGrid;
        ShowMinorGrid = other.ShowMinorGrid;
        MajorGridColor = other.MajorGridColor;
        MinorGridColor = other.MinorGridColor;
        MajorGridWidth = other.MajorGridWidth;
        MinorGridWidth = other.MinorGridWidth;
        MajorGridDashPattern = other.MajorGridDashPattern;
        MinorGridDashPattern = other.MinorGridDashPattern;
    }
}

/// <summary>
/// Axis style settings.
/// </summary>
public class AxisStyle
{
    public SKColor LineColor { get; set; } = SKColors.Black;
    public float LineWidth { get; set; } = 1.5f;
    public SKColor LabelColor { get; set; } = SKColors.Black;
    public float LabelFontSize { get; set; } = 11.0f;
    public SKColor TitleColor { get; set; } = SKColors.Black;
    public float TitleFontSize { get; set; } = 12.0f;
    public SKColor TickColor { get; set; } = SKColors.Black;
    public float TickLength { get; set; } = 5.0f;
    public float TickWidth { get; set; } = 1.0f;

    public AxisStyle Clone() => new()
    {
        LineColor = LineColor,
        LineWidth = LineWidth,
        LabelColor = LabelColor,
        LabelFontSize = LabelFontSize,
        TitleColor = TitleColor,
        TitleFontSize = TitleFontSize,
        TickColor = TickColor,
        TickLength = TickLength,
        TickWidth = TickWidth
    };

    public void Merge(AxisStyle other)
    {
        if (other == null) return;
        LineColor = other.LineColor;
        LineWidth = other.LineWidth;
        LabelColor = other.LabelColor;
        LabelFontSize = other.LabelFontSize;
        TitleColor = other.TitleColor;
        TitleFontSize = other.TitleFontSize;
        TickColor = other.TickColor;
        TickLength = other.TickLength;
        TickWidth = other.TickWidth;
    }
}

/// <summary>
/// Series style settings.
/// </summary>
public class SeriesStyle
{
    public float LineWidth { get; set; } = 2.0f;
    public float MarkerSize { get; set; } = 6.0f;
    public float AreaOpacity { get; set; } = 0.3f;
    public bool ShowMarkers { get; set; } = true;
    public bool EnableAntiAlias { get; set; } = true;

    public SeriesStyle Clone() => new()
    {
        LineWidth = LineWidth,
        MarkerSize = MarkerSize,
        AreaOpacity = AreaOpacity,
        ShowMarkers = ShowMarkers,
        EnableAntiAlias = EnableAntiAlias
    };

    public void Merge(SeriesStyle other)
    {
        if (other == null) return;
        LineWidth = other.LineWidth;
        MarkerSize = other.MarkerSize;
        AreaOpacity = other.AreaOpacity;
        ShowMarkers = other.ShowMarkers;
        EnableAntiAlias = other.EnableAntiAlias;
    }
}

/// <summary>
/// Legend style settings.
/// </summary>
public class LegendStyle
{
    public SKColor BackgroundColor { get; set; } = new SKColor(255, 255, 255, 230);
    public SKColor BorderColor { get; set; } = new SKColor(200, 200, 200);
    public float BorderWidth { get; set; } = 1.0f;
    public SKColor TextColor { get; set; } = SKColors.Black;
    public float FontSize { get; set; } = 11.0f;
    public float Padding { get; set; } = 10.0f;
    public float CornerRadius { get; set; } = 4.0f;

    public LegendStyle Clone() => new()
    {
        BackgroundColor = BackgroundColor,
        BorderColor = BorderColor,
        BorderWidth = BorderWidth,
        TextColor = TextColor,
        FontSize = FontSize,
        Padding = Padding,
        CornerRadius = CornerRadius
    };

    public void Merge(LegendStyle other)
    {
        if (other == null) return;
        BackgroundColor = other.BackgroundColor;
        BorderColor = other.BorderColor;
        BorderWidth = other.BorderWidth;
        TextColor = other.TextColor;
        FontSize = other.FontSize;
        Padding = other.Padding;
        CornerRadius = other.CornerRadius;
    }
}

/// <summary>
/// Tooltip style settings.
/// </summary>
public class TooltipStyle
{
    public SKColor BackgroundColor { get; set; } = new SKColor(50, 50, 50, 240);
    public SKColor TextColor { get; set; } = SKColors.White;
    public float FontSize { get; set; } = 11.0f;
    public float Padding { get; set; } = 8.0f;
    public float CornerRadius { get; set; } = 4.0f;
    public SKColor BorderColor { get; set; } = new SKColor(100, 100, 100);
    public float BorderWidth { get; set; } = 1.0f;

    public TooltipStyle Clone() => new()
    {
        BackgroundColor = BackgroundColor,
        TextColor = TextColor,
        FontSize = FontSize,
        Padding = Padding,
        CornerRadius = CornerRadius,
        BorderColor = BorderColor,
        BorderWidth = BorderWidth
    };

    public void Merge(TooltipStyle other)
    {
        if (other == null) return;
        BackgroundColor = other.BackgroundColor;
        TextColor = other.TextColor;
        FontSize = other.FontSize;
        Padding = other.Padding;
        CornerRadius = other.CornerRadius;
        BorderColor = other.BorderColor;
        BorderWidth = other.BorderWidth;
    }
}

/// <summary>
/// Title style settings.
/// </summary>
public class TitleStyle
{
    public SKColor Color { get; set; } = SKColors.Black;
    public float FontSize { get; set; } = 16.0f;
    public SKFontStyle FontStyle { get; set; } = SKFontStyle.Bold;
    public SKColor SubtitleColor { get; set; } = new SKColor(100, 100, 100);
    public float SubtitleFontSize { get; set; } = 12.0f;

    public TitleStyle Clone() => new()
    {
        Color = Color,
        FontSize = FontSize,
        FontStyle = FontStyle,
        SubtitleColor = SubtitleColor,
        SubtitleFontSize = SubtitleFontSize
    };

    public void Merge(TitleStyle other)
    {
        if (other == null) return;
        Color = other.Color;
        FontSize = other.FontSize;
        FontStyle = other.FontStyle;
        SubtitleColor = other.SubtitleColor;
        SubtitleFontSize = other.SubtitleFontSize;
    }
}

/// <summary>
/// Font style settings.
/// </summary>
public class FontStyle
{
    public string DefaultFontFamily { get; set; } = "Arial";
    public string TitleFontFamily { get; set; } = "Arial";
    public string LabelFontFamily { get; set; } = "Arial";
    public string MonospaceFontFamily { get; set; } = "Courier New";

    public FontStyle Clone() => new()
    {
        DefaultFontFamily = DefaultFontFamily,
        TitleFontFamily = TitleFontFamily,
        LabelFontFamily = LabelFontFamily,
        MonospaceFontFamily = MonospaceFontFamily
    };

    public void Merge(FontStyle other)
    {
        if (other == null) return;
        DefaultFontFamily = other.DefaultFontFamily;
        TitleFontFamily = other.TitleFontFamily;
        LabelFontFamily = other.LabelFontFamily;
        MonospaceFontFamily = other.MonospaceFontFamily;
    }
}
