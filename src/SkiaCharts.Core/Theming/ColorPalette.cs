using SkiaSharp;

namespace SkiaCharts.Core.Theming;

/// <summary>
/// Represents a color palette for charts.
/// </summary>
public class ColorPalette
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColorPalette"/> class.
    /// </summary>
    public ColorPalette(string name, ColorPaletteType type, params SKColor[] colors)
    {
        Name = name;
        Type = type;
        Colors = colors.ToList();
    }

    /// <summary>
    /// Gets the palette name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the palette type.
    /// </summary>
    public ColorPaletteType Type { get; }

    /// <summary>
    /// Gets the colors in the palette.
    /// </summary>
    public List<SKColor> Colors { get; }

    /// <summary>
    /// Gets a color by index (wraps around if index exceeds palette size).
    /// </summary>
    public SKColor GetColor(int index)
    {
        if (Colors.Count == 0)
            return SKColors.Gray;

        return Colors[index % Colors.Count];
    }

    /// <summary>
    /// Gets a color by interpolating between palette colors (0.0 to 1.0).
    /// Used for sequential and diverging palettes.
    /// </summary>
    public SKColor GetInterpolatedColor(double value)
    {
        if (Colors.Count == 0)
            return SKColors.Gray;

        if (Colors.Count == 1)
            return Colors[0];

        value = Math.Clamp(value, 0.0, 1.0);

        var scaledValue = value * (Colors.Count - 1);
        var index = (int)scaledValue;
        var fraction = scaledValue - index;

        if (index >= Colors.Count - 1)
            return Colors[^1];

        var color1 = Colors[index];
        var color2 = Colors[index + 1];

        return InterpolateColor(color1, color2, (float)fraction);
    }

    private SKColor InterpolateColor(SKColor c1, SKColor c2, float t)
    {
        return new SKColor(
            (byte)(c1.Red + (c2.Red - c1.Red) * t),
            (byte)(c1.Green + (c2.Green - c1.Green) * t),
            (byte)(c1.Blue + (c2.Blue - c1.Blue) * t),
            (byte)(c1.Alpha + (c2.Alpha - c1.Alpha) * t)
        );
    }

    /// <summary>
    /// Creates a deep copy of this palette.
    /// </summary>
    public ColorPalette Clone()
    {
        return new ColorPalette(Name, Type, Colors.ToArray());
    }
}

/// <summary>
/// Color palette type enumeration.
/// </summary>
public enum ColorPaletteType
{
    /// <summary>Categorical palette for distinct categories.</summary>
    Categorical,
    /// <summary>Sequential palette for continuous data.</summary>
    Sequential,
    /// <summary>Diverging palette for data with a meaningful midpoint.</summary>
    Diverging
}

/// <summary>
/// Predefined color palettes.
/// </summary>
public static class ColorPalettes
{
    // Categorical Palettes

    /// <summary>
    /// Default categorical palette (8 distinct colors).
    /// </summary>
    public static ColorPalette Default => new(
        "Default",
        ColorPaletteType.Categorical,
        new SKColor(31, 119, 180),   // Blue
        new SKColor(255, 127, 14),   // Orange
        new SKColor(44, 160, 44),    // Green
        new SKColor(214, 39, 40),    // Red
        new SKColor(148, 103, 189),  // Purple
        new SKColor(140, 86, 75),    // Brown
        new SKColor(227, 119, 194),  // Pink
        new SKColor(127, 127, 127)   // Gray
    );

    /// <summary>
    /// Vibrant categorical palette (10 distinct colors).
    /// </summary>
    public static ColorPalette Vibrant => new(
        "Vibrant",
        ColorPaletteType.Categorical,
        new SKColor(230, 25, 75),    // Red
        new SKColor(60, 180, 75),    // Green
        new SKColor(255, 225, 25),   // Yellow
        new SKColor(0, 130, 200),    // Blue
        new SKColor(245, 130, 48),   // Orange
        new SKColor(145, 30, 180),   // Purple
        new SKColor(70, 240, 240),   // Cyan
        new SKColor(240, 50, 230),   // Magenta
        new SKColor(210, 245, 60),   // Lime
        new SKColor(250, 190, 212)   // Pink
    );

    /// <summary>
    /// Pastel categorical palette (8 soft colors).
    /// </summary>
    public static ColorPalette Pastel => new(
        "Pastel",
        ColorPaletteType.Categorical,
        new SKColor(179, 226, 205),  // Mint
        new SKColor(253, 205, 172),  // Peach
        new SKColor(203, 213, 232),  // Lavender
        new SKColor(244, 202, 228),  // Pink
        new SKColor(230, 245, 201),  // Light Green
        new SKColor(255, 242, 174),  // Light Yellow
        new SKColor(241, 226, 204),  // Beige
        new SKColor(207, 226, 243)   // Light Blue
    );

    /// <summary>
    /// Professional/Business categorical palette (6 colors).
    /// </summary>
    public static ColorPalette Professional => new(
        "Professional",
        ColorPaletteType.Categorical,
        new SKColor(0, 92, 175),     // Corporate Blue
        new SKColor(112, 173, 71),   // Forest Green
        new SKColor(192, 0, 0),      // Deep Red
        new SKColor(255, 192, 0),    // Gold
        new SKColor(91, 155, 213),   // Sky Blue
        new SKColor(165, 165, 165)   // Neutral Gray
    );

    // Sequential Palettes

    /// <summary>
    /// Blue sequential palette (light to dark).
    /// </summary>
    public static ColorPalette BluesSequential => new(
        "Blues",
        ColorPaletteType.Sequential,
        new SKColor(247, 251, 255),  // Very Light Blue
        new SKColor(222, 235, 247),  // Light Blue
        new SKColor(198, 219, 239),  // Medium Light Blue
        new SKColor(158, 202, 225),  // Medium Blue
        new SKColor(107, 174, 214),  // Medium Dark Blue
        new SKColor(66, 146, 198),   // Dark Blue
        new SKColor(33, 113, 181),   // Darker Blue
        new SKColor(8, 69, 148)      // Darkest Blue
    );

    /// <summary>
    /// Green sequential palette (light to dark).
    /// </summary>
    public static ColorPalette GreensSequential => new(
        "Greens",
        ColorPaletteType.Sequential,
        new SKColor(247, 252, 245),  // Very Light Green
        new SKColor(229, 245, 224),  // Light Green
        new SKColor(199, 233, 192),  // Medium Light Green
        new SKColor(161, 217, 155),  // Medium Green
        new SKColor(116, 196, 118),  // Medium Dark Green
        new SKColor(65, 171, 93),    // Dark Green
        new SKColor(35, 139, 69),    // Darker Green
        new SKColor(0, 90, 50)       // Darkest Green
    );

    /// <summary>
    /// Reds sequential palette (light to dark).
    /// </summary>
    public static ColorPalette RedsSequential => new(
        "Reds",
        ColorPaletteType.Sequential,
        new SKColor(255, 245, 240),  // Very Light Red
        new SKColor(254, 224, 210),  // Light Red
        new SKColor(252, 187, 161),  // Medium Light Red
        new SKColor(252, 146, 114),  // Medium Red
        new SKColor(251, 106, 74),   // Medium Dark Red
        new SKColor(239, 59, 44),    // Dark Red
        new SKColor(203, 24, 29),    // Darker Red
        new SKColor(153, 0, 13)      // Darkest Red
    );

    /// <summary>
    /// Heat sequential palette (yellow to red).
    /// </summary>
    public static ColorPalette Heat => new(
        "Heat",
        ColorPaletteType.Sequential,
        new SKColor(255, 255, 204),  // Light Yellow
        new SKColor(255, 237, 160),  // Yellow
        new SKColor(254, 217, 118),  // Gold
        new SKColor(254, 178, 76),   // Orange
        new SKColor(253, 141, 60),   // Deep Orange
        new SKColor(252, 78, 42),    // Red Orange
        new SKColor(227, 26, 28),    // Red
        new SKColor(189, 0, 38)      // Dark Red
    );

    // Diverging Palettes

    /// <summary>
    /// Red-Blue diverging palette (for data with meaningful midpoint).
    /// </summary>
    public static ColorPalette RedBlue => new(
        "Red-Blue",
        ColorPaletteType.Diverging,
        new SKColor(178, 24, 43),    // Dark Red
        new SKColor(214, 96, 77),    // Medium Red
        new SKColor(244, 165, 130),  // Light Red
        new SKColor(253, 219, 199),  // Very Light Red
        new SKColor(247, 247, 247),  // Neutral Gray
        new SKColor(209, 229, 240),  // Very Light Blue
        new SKColor(146, 197, 222),  // Light Blue
        new SKColor(67, 147, 195),   // Medium Blue
        new SKColor(33, 102, 172)    // Dark Blue
    );

    /// <summary>
    /// Purple-Green diverging palette.
    /// </summary>
    public static ColorPalette PurpleGreen => new(
        "Purple-Green",
        ColorPaletteType.Diverging,
        new SKColor(118, 42, 131),   // Dark Purple
        new SKColor(153, 112, 171),  // Medium Purple
        new SKColor(194, 165, 207),  // Light Purple
        new SKColor(231, 212, 232),  // Very Light Purple
        new SKColor(247, 247, 247),  // Neutral Gray
        new SKColor(217, 240, 211),  // Very Light Green
        new SKColor(166, 219, 160),  // Light Green
        new SKColor(90, 174, 97),    // Medium Green
        new SKColor(27, 120, 55)     // Dark Green
    );

    /// <summary>
    /// Brown-Teal diverging palette.
    /// </summary>
    public static ColorPalette BrownTeal => new(
        "Brown-Teal",
        ColorPaletteType.Diverging,
        new SKColor(140, 81, 10),    // Dark Brown
        new SKColor(191, 129, 45),   // Medium Brown
        new SKColor(223, 194, 125),  // Light Brown
        new SKColor(246, 232, 195),  // Very Light Brown
        new SKColor(245, 245, 245),  // Neutral Gray
        new SKColor(199, 234, 229),  // Very Light Teal
        new SKColor(128, 205, 193),  // Light Teal
        new SKColor(53, 151, 143),   // Medium Teal
        new SKColor(1, 102, 94)      // Dark Teal
    );

    /// <summary>
    /// Spectral diverging palette (rainbow-like).
    /// </summary>
    public static ColorPalette Spectral => new(
        "Spectral",
        ColorPaletteType.Diverging,
        new SKColor(158, 1, 66),     // Dark Red
        new SKColor(213, 62, 79),    // Red
        new SKColor(244, 109, 67),   // Orange Red
        new SKColor(253, 174, 97),   // Orange
        new SKColor(254, 224, 139),  // Yellow
        new SKColor(255, 255, 191),  // Light Yellow
        new SKColor(230, 245, 152),  // Yellow Green
        new SKColor(171, 221, 164),  // Light Green
        new SKColor(102, 194, 165),  // Green
        new SKColor(50, 136, 189),   // Blue
        new SKColor(94, 79, 162)     // Dark Blue
    );

    /// <summary>
    /// Gets all available palettes.
    /// </summary>
    public static IEnumerable<ColorPalette> All => new[]
    {
        Default, Vibrant, Pastel, Professional,
        BluesSequential, GreensSequential, RedsSequential, Heat,
        RedBlue, PurpleGreen, BrownTeal, Spectral
    };

    /// <summary>
    /// Gets all categorical palettes.
    /// </summary>
    public static IEnumerable<ColorPalette> Categorical => new[]
    {
        Default, Vibrant, Pastel, Professional
    };

    /// <summary>
    /// Gets all sequential palettes.
    /// </summary>
    public static IEnumerable<ColorPalette> Sequential => new[]
    {
        BluesSequential, GreensSequential, RedsSequential, Heat
    };

    /// <summary>
    /// Gets all diverging palettes.
    /// </summary>
    public static IEnumerable<ColorPalette> Diverging => new[]
    {
        RedBlue, PurpleGreen, BrownTeal, Spectral
    };
}
