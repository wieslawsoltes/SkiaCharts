using SkiaSharp;

namespace SkiaCharts.Core.Theming;

/// <summary>
/// Colorblind-safe and accessible color palettes.
/// </summary>
public static class AccessibilityPalettes
{
    /// <summary>
    /// Colorblind-safe palette for deuteranopia (red-green colorblindness).
    /// Uses blue-orange contrast that's distinguishable by all types of colorblindness.
    /// </summary>
    public static ColorPalette ColorblindSafe => new(
        "Colorblind Safe",
        ColorPaletteType.Categorical,
        new SKColor(0, 114, 178),    // Blue
        new SKColor(230, 159, 0),    // Orange
        new SKColor(86, 180, 233),   // Sky Blue
        new SKColor(240, 228, 66),   // Yellow
        new SKColor(0, 158, 115),    // Bluish Green
        new SKColor(213, 94, 0),     // Vermillion
        new SKColor(204, 121, 167)   // Reddish Purple
    );

    /// <summary>
    /// High contrast palette optimized for deuteranopia.
    /// Based on research by Martin Krzywinski.
    /// </summary>
    public static ColorPalette Deuteranopia => new(
        "Deuteranopia Safe",
        ColorPaletteType.Categorical,
        new SKColor(0, 92, 171),     // Strong Blue
        new SKColor(242, 95, 92),    // Strong Red
        new SKColor(255, 188, 0),    // Vivid Yellow
        new SKColor(36, 139, 142),   // Strong Cyan
        new SKColor(153, 79, 162),   // Strong Purple
        new SKColor(242, 133, 35)    // Vivid Orange
    );

    /// <summary>
    /// Protanopia-safe palette (red-blind).
    /// Avoids red-green confusion.
    /// </summary>
    public static ColorPalette Protanopia => new(
        "Protanopia Safe",
        ColorPaletteType.Categorical,
        new SKColor(0, 102, 204),    // Blue
        new SKColor(255, 204, 0),    // Gold
        new SKColor(51, 153, 255),   // Light Blue
        new SKColor(0, 51, 102),     // Dark Blue
        new SKColor(255, 153, 51),   // Orange
        new SKColor(153, 204, 255)   // Very Light Blue
    );

    /// <summary>
    /// Tritanopia-safe palette (blue-yellow colorblindness).
    /// Uses red-cyan contrast.
    /// </summary>
    public static ColorPalette Tritanopia => new(
        "Tritanopia Safe",
        ColorPaletteType.Categorical,
        new SKColor(204, 0, 0),      // Red
        new SKColor(0, 153, 153),    // Cyan
        new SKColor(255, 102, 102),  // Light Red
        new SKColor(102, 0, 0),      // Dark Red
        new SKColor(102, 204, 204),  // Light Cyan
        new SKColor(0, 102, 102)     // Dark Cyan
    );

    /// <summary>
    /// IBM accessible palette - designed for all colorblindness types.
    /// Based on IBM Design Language.
    /// </summary>
    public static ColorPalette IbmAccessible => new(
        "IBM Accessible",
        ColorPaletteType.Categorical,
        new SKColor(100, 143, 255),  // Blue 60
        new SKColor(254, 97, 0),     // Orange 60
        new SKColor(0, 220, 130),    // Green 50
        new SKColor(255, 131, 43),   // Orange 50
        new SKColor(170, 133, 255),  // Purple 50
        new SKColor(255, 199, 0),    // Yellow 30
        new SKColor(255, 56, 161)    // Magenta 50
    );

    /// <summary>
    /// Paul Tol's bright palette - scientifically designed for colorblindness.
    /// Maximum distinguishability for all vision types.
    /// </summary>
    public static ColorPalette PaulTolBright => new(
        "Paul Tol Bright",
        ColorPaletteType.Categorical,
        new SKColor(68, 119, 170),   // Blue
        new SKColor(238, 102, 119),  // Red
        new SKColor(34, 136, 51),    // Green
        new SKColor(204, 187, 68),   // Yellow
        new SKColor(102, 204, 238),  // Cyan
        new SKColor(187, 85, 102),   // Wine
        new SKColor(170, 51, 119)    // Purple
    );

    /// <summary>
    /// High contrast sequential palette for colorblind users.
    /// Uses luminance variation instead of hue.
    /// </summary>
    public static ColorPalette ColorblindSequential => new(
        "Colorblind Sequential",
        ColorPaletteType.Sequential,
        new SKColor(255, 247, 188),  // Very Light
        new SKColor(254, 227, 145),  // Light
        new SKColor(254, 196, 79),   // Medium Light
        new SKColor(254, 153, 41),   // Medium
        new SKColor(236, 112, 20),   // Medium Dark
        new SKColor(204, 76, 2),     // Dark
        new SKColor(153, 52, 4),     // Very Dark
        new SKColor(102, 37, 6)      // Darkest
    );

    /// <summary>
    /// Wong's palette - designed for Nature publications.
    /// 8 colors distinguishable by all colorblind types.
    /// </summary>
    public static ColorPalette Wong => new(
        "Wong (Nature)",
        ColorPaletteType.Categorical,
        new SKColor(0, 0, 0),        // Black
        new SKColor(230, 159, 0),    // Orange
        new SKColor(86, 180, 233),   // Sky Blue
        new SKColor(0, 158, 115),    // Bluish Green
        new SKColor(240, 228, 66),   // Yellow
        new SKColor(0, 114, 178),    // Blue
        new SKColor(213, 94, 0),     // Vermillion
        new SKColor(204, 121, 167)   // Reddish Purple
    );

    /// <summary>
    /// Okabe-Ito palette - gold standard for colorblind-safe colors.
    /// Recommended by multiple accessibility guidelines.
    /// </summary>
    public static ColorPalette OkabeIto => new(
        "Okabe-Ito",
        ColorPaletteType.Categorical,
        new SKColor(230, 159, 0),    // Orange
        new SKColor(86, 180, 233),   // Sky Blue
        new SKColor(0, 158, 115),    // Bluish Green
        new SKColor(240, 228, 66),   // Yellow
        new SKColor(0, 114, 178),    // Blue
        new SKColor(213, 94, 0),     // Vermillion
        new SKColor(204, 121, 167),  // Reddish Purple
        new SKColor(0, 0, 0)         // Black
    );

    /// <summary>
    /// High contrast grayscale - when color cannot be used at all.
    /// </summary>
    public static ColorPalette HighContrastGrayscale => new(
        "High Contrast Grayscale",
        ColorPaletteType.Categorical,
        new SKColor(0, 0, 0),        // Black
        new SKColor(255, 255, 255),  // White
        new SKColor(60, 60, 60),     // Very Dark Gray
        new SKColor(200, 200, 200),  // Light Gray
        new SKColor(100, 100, 100),  // Medium Gray
        new SKColor(150, 150, 150)   // Medium Light Gray
    );

    /// <summary>
    /// Gets all colorblind-safe palettes.
    /// </summary>
    public static IEnumerable<ColorPalette> All => new[]
    {
        ColorblindSafe,
        Deuteranopia,
        Protanopia,
        Tritanopia,
        IbmAccessible,
        PaulTolBright,
        ColorblindSequential,
        Wong,
        OkabeIto,
        HighContrastGrayscale
    };

    /// <summary>
    /// Gets all categorical colorblind palettes.
    /// </summary>
    public static IEnumerable<ColorPalette> Categorical => new[]
    {
        ColorblindSafe,
        Deuteranopia,
        Protanopia,
        Tritanopia,
        IbmAccessible,
        PaulTolBright,
        Wong,
        OkabeIto,
        HighContrastGrayscale
    };

    /// <summary>
    /// Gets a colorblind-safe palette by name (case-insensitive).
    /// </summary>
    public static ColorPalette? GetByName(string name)
    {
        return All.FirstOrDefault(p =>
            p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the recommended palette for a specific colorblindness type.
    /// </summary>
    public static ColorPalette GetForColorblindType(ColorblindType type)
    {
        return type switch
        {
            ColorblindType.Deuteranopia => Deuteranopia,
            ColorblindType.Protanopia => Protanopia,
            ColorblindType.Tritanopia => Tritanopia,
            ColorblindType.None => ColorblindSafe,
            _ => ColorblindSafe
        };
    }
}

/// <summary>
/// Types of colorblindness.
/// </summary>
public enum ColorblindType
{
    /// <summary>No colorblindness.</summary>
    None,

    /// <summary>Red-green colorblindness (most common, ~5% of males).</summary>
    Deuteranopia,

    /// <summary>Red-blind (less common, ~1% of males).</summary>
    Protanopia,

    /// <summary>Blue-yellow colorblindness (very rare, &lt;0.01%).</summary>
    Tritanopia,

    /// <summary>Complete colorblindness (extremely rare).</summary>
    Achromatopsia
}

/// <summary>
/// Utilities for simulating and testing colorblind vision.
/// </summary>
public static class ColorblindSimulator
{
    /// <summary>
    /// Simulates how a color appears to someone with deuteranopia.
    /// </summary>
    public static SKColor SimulateDeuteranopia(SKColor color)
    {
        // Convert RGB to LMS color space
        var l = 17.8824f * color.Red + 43.5161f * color.Green + 4.11935f * color.Blue;
        var m = 3.45565f * color.Red + 27.1554f * color.Green + 3.86714f * color.Blue;
        var s = 0.0299566f * color.Red + 0.184309f * color.Green + 1.46709f * color.Blue;

        // Apply deuteranopia transformation
        var lNew = l;
        var mNew = 0.494207f * l + 1.24827f * s;
        var sNew = s;

        // Convert back to RGB
        var r = (byte)Math.Clamp(0.0809444479f * lNew - 0.130504409f * mNew + 0.116721066f * sNew, 0, 255);
        var g = (byte)Math.Clamp(-0.0102485335f * lNew + 0.0540193266f * mNew - 0.113614708f * sNew, 0, 255);
        var b = (byte)Math.Clamp(-0.000365296938f * lNew - 0.00412161469f * mNew + 0.693511405f * sNew, 0, 255);

        return new SKColor(r, g, b, color.Alpha);
    }

    /// <summary>
    /// Simulates how a color appears to someone with protanopia.
    /// </summary>
    public static SKColor SimulateProtanopia(SKColor color)
    {
        var l = 17.8824f * color.Red + 43.5161f * color.Green + 4.11935f * color.Blue;
        var m = 3.45565f * color.Red + 27.1554f * color.Green + 3.86714f * color.Blue;
        var s = 0.0299566f * color.Red + 0.184309f * color.Green + 1.46709f * color.Blue;

        // Apply protanopia transformation
        var lNew = 0.0f * l + 2.02344f * m - 2.52581f * s;
        var mNew = m;
        var sNew = s;

        var r = (byte)Math.Clamp(0.0809444479f * lNew - 0.130504409f * mNew + 0.116721066f * sNew, 0, 255);
        var g = (byte)Math.Clamp(-0.0102485335f * lNew + 0.0540193266f * mNew - 0.113614708f * sNew, 0, 255);
        var b = (byte)Math.Clamp(-0.000365296938f * lNew - 0.00412161469f * mNew + 0.693511405f * sNew, 0, 255);

        return new SKColor(r, g, b, color.Alpha);
    }

    /// <summary>
    /// Simulates how a color appears to someone with tritanopia.
    /// </summary>
    public static SKColor SimulateTritanopia(SKColor color)
    {
        var l = 17.8824f * color.Red + 43.5161f * color.Green + 4.11935f * color.Blue;
        var m = 3.45565f * color.Red + 27.1554f * color.Green + 3.86714f * color.Blue;
        var s = 0.0299566f * color.Red + 0.184309f * color.Green + 1.46709f * color.Blue;

        // Apply tritanopia transformation
        var lNew = l;
        var mNew = m;
        var sNew = -0.395913f * l + 0.801109f * m;

        var r = (byte)Math.Clamp(0.0809444479f * lNew - 0.130504409f * mNew + 0.116721066f * sNew, 0, 255);
        var g = (byte)Math.Clamp(-0.0102485335f * lNew + 0.0540193266f * mNew - 0.113614708f * sNew, 0, 255);
        var b = (byte)Math.Clamp(-0.000365296938f * lNew - 0.00412161469f * mNew + 0.693511405f * sNew, 0, 255);

        return new SKColor(r, g, b, color.Alpha);
    }

    /// <summary>
    /// Simulates colorblind vision for a given type.
    /// </summary>
    public static SKColor Simulate(SKColor color, ColorblindType type)
    {
        return type switch
        {
            ColorblindType.Deuteranopia => SimulateDeuteranopia(color),
            ColorblindType.Protanopia => SimulateProtanopia(color),
            ColorblindType.Tritanopia => SimulateTritanopia(color),
            ColorblindType.Achromatopsia => ToGrayscale(color),
            _ => color
        };
    }

    /// <summary>
    /// Converts a color to grayscale.
    /// </summary>
    public static SKColor ToGrayscale(SKColor color)
    {
        var gray = (byte)(0.299 * color.Red + 0.587 * color.Green + 0.114 * color.Blue);
        return new SKColor(gray, gray, gray, color.Alpha);
    }

    /// <summary>
    /// Tests if two colors are distinguishable for colorblind users.
    /// </summary>
    public static bool AreDistinguishable(SKColor c1, SKColor c2, ColorblindType type, double threshold = 30.0)
    {
        var sim1 = Simulate(c1, type);
        var sim2 = Simulate(c2, type);

        var deltaR = Math.Abs(sim1.Red - sim2.Red);
        var deltaG = Math.Abs(sim1.Green - sim2.Green);
        var deltaB = Math.Abs(sim1.Blue - sim2.Blue);

        var distance = Math.Sqrt(deltaR * deltaR + deltaG * deltaG + deltaB * deltaB);
        return distance >= threshold;
    }
}
