using System.Globalization;
using Avalonia.Data.Converters;
using SkiaCharts.Core.Theming;
using SkiaSharp;

namespace SkiaCharts.Avalonia.Converters;

/// <summary>
/// Converts between Avalonia colors and SkiaSharp colors.
/// </summary>
public class ColorToSKColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is global::Avalonia.Media.Color avaloniaColor)
        {
            return new SKColor(avaloniaColor.R, avaloniaColor.G, avaloniaColor.B, avaloniaColor.A);
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is SKColor skColor)
        {
            return global::Avalonia.Media.Color.FromArgb(skColor.Alpha, skColor.Red, skColor.Green, skColor.Blue);
        }
        return null;
    }
}

/// <summary>
/// Converts boolean to visibility for UI elements.
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            bool invert = parameter is string str && str == "Invert";
            bool result = invert ? !boolValue : boolValue;
            return result;
        }
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts theme names to ChartTheme objects.
/// </summary>
public class ThemeNameConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string themeName)
        {
            return themeName.ToLowerInvariant() switch
            {
                "light" => ThemePresets.Light,
                "dark" => ThemePresets.Dark,
                "professional" => ThemePresets.Professional,
                "highcontrast" => ThemePresets.HighContrast,
                _ => ThemePresets.Light
            };
        }
        return ThemePresets.Light;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ChartTheme theme)
        {
            // Simple comparison based on some theme characteristics
            if (ReferenceEquals(theme, ThemePresets.Dark))
                return "dark";
            if (ReferenceEquals(theme, ThemePresets.Professional))
                return "professional";
            if (ReferenceEquals(theme, ThemePresets.HighContrast))
                return "highcontrast";
            return "light";
        }
        return "light";
    }
}

/// <summary>
/// Converts numbers to formatted strings with units.
/// </summary>
public class NumberFormatConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double doubleValue)
        {
            string format = parameter as string ?? "F2";
            return doubleValue.ToString(format, culture);
        }
        if (value is int intValue)
        {
            return intValue.ToString(culture);
        }
        return value?.ToString() ?? string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str && double.TryParse(str, NumberStyles.Any, culture, out double result))
        {
            return result;
        }
        return null;
    }
}

/// <summary>
/// Converts TimeSpan to milliseconds for binding.
/// </summary>
public class TimeSpanToMillisecondsConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan)
        {
            return timeSpan.TotalMilliseconds;
        }
        return 0.0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double milliseconds)
        {
            return TimeSpan.FromMilliseconds(milliseconds);
        }
        return TimeSpan.Zero;
    }
}
