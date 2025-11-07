using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using SkiaCharts.Core.Theming;
using Avalonia.Controls.Primitives;

namespace SkiaCharts.Avalonia.Theming;

/// <summary>
/// Manages theme switching for SkiaChartView controls.
/// </summary>
public static class ChartThemeManager
{
    /// <summary>
    /// Gets or sets the default chart theme variant.
    /// </summary>
    public static ChartThemeVariant DefaultThemeVariant { get; set; } = ChartThemeVariant.Light;

    /// <summary>
    /// Applies a theme variant to the application.
    /// </summary>
    /// <param name="application">The application instance.</param>
    /// <param name="variant">The theme variant to apply.</param>
    public static void ApplyTheme(Application application, ChartThemeVariant variant)
    {
        if (application == null)
            throw new ArgumentNullException(nameof(application));

        DefaultThemeVariant = variant;

        // Update RequestedThemeVariant if using Avalonia 11+
        if (application.RequestedThemeVariant != null)
        {
            application.RequestedThemeVariant = variant switch
            {
                ChartThemeVariant.Dark => ThemeVariant.Dark,
                ChartThemeVariant.Light => ThemeVariant.Light,
                _ => ThemeVariant.Light
            };
        }
    }

    /// <summary>
    /// Applies a theme variant to a specific control.
    /// </summary>
    /// <param name="control">The control to theme.</param>
    /// <param name="variant">The theme variant to apply.</param>
    public static void ApplyTheme(Control control, ChartThemeVariant variant)
    {
        if (control == null)
            throw new ArgumentNullException(nameof(control));

        // Remove existing theme classes
        control.Classes.Remove("light");
        control.Classes.Remove("dark");
        control.Classes.Remove("professional");
        control.Classes.Remove("highcontrast");
        control.Classes.Remove("fluent-light");
        control.Classes.Remove("fluent-dark");

        // Add new theme class
        var className = variant switch
        {
            ChartThemeVariant.Light => "light",
            ChartThemeVariant.Dark => "dark",
            ChartThemeVariant.Professional => "professional",
            ChartThemeVariant.HighContrast => "highcontrast",
            ChartThemeVariant.FluentLight => "fluent-light",
            ChartThemeVariant.FluentDark => "fluent-dark",
            _ => "light"
        };

        control.Classes.Add(className);

        // Set RequestedThemeVariant using SetValue (available in Avalonia 11+)
        try
        {
            var themeVariant = variant switch
            {
                ChartThemeVariant.Dark or ChartThemeVariant.FluentDark or ChartThemeVariant.HighContrast => ThemeVariant.Dark,
                _ => ThemeVariant.Light
            };

            // Use reflection or direct property access if available
            var property = control.GetType().GetProperty("RequestedThemeVariant");
            if (property != null && property.CanWrite)
            {
                property.SetValue(control, themeVariant);
            }
        }
        catch
        {
            // RequestedThemeVariant not available, continue
        }
    }

    /// <summary>
    /// Gets the ChartTheme corresponding to a theme variant.
    /// </summary>
    /// <param name="variant">The theme variant.</param>
    /// <returns>The corresponding ChartTheme.</returns>
    public static Core.Theming.ChartTheme GetChartTheme(ChartThemeVariant variant)
    {
        return variant switch
        {
            ChartThemeVariant.Light or ChartThemeVariant.FluentLight => ThemePresets.Light,
            ChartThemeVariant.Dark or ChartThemeVariant.FluentDark => ThemePresets.Dark,
            ChartThemeVariant.Professional => ThemePresets.Professional,
            ChartThemeVariant.HighContrast => ThemePresets.HighContrast,
            _ => ThemePresets.Light
        };
    }

    /// <summary>
    /// Detects the system theme and returns the corresponding variant.
    /// </summary>
    /// <returns>The detected theme variant.</returns>
    public static ChartThemeVariant DetectSystemTheme()
    {
        if (Application.Current?.ActualThemeVariant == ThemeVariant.Dark)
        {
            return ChartThemeVariant.Dark;
        }

        return ChartThemeVariant.Light;
    }

    /// <summary>
    /// Toggles between light and dark theme variants.
    /// </summary>
    /// <param name="currentVariant">The current theme variant.</param>
    /// <returns>The toggled theme variant.</returns>
    public static ChartThemeVariant ToggleTheme(ChartThemeVariant currentVariant)
    {
        return currentVariant switch
        {
            ChartThemeVariant.Light => ChartThemeVariant.Dark,
            ChartThemeVariant.Dark => ChartThemeVariant.Light,
            ChartThemeVariant.FluentLight => ChartThemeVariant.FluentDark,
            ChartThemeVariant.FluentDark => ChartThemeVariant.FluentLight,
            _ => ChartThemeVariant.Light
        };
    }

    /// <summary>
    /// Syncs the chart theme with the Avalonia RequestedThemeVariant.
    /// </summary>
    /// <param name="control">The control to sync.</param>
    public static void SyncWithAvaloniaTheme(Control control)
    {
        if (control == null)
            throw new ArgumentNullException(nameof(control));

        var avaloniaTheme = control.ActualThemeVariant;
        var chartVariant = avaloniaTheme == ThemeVariant.Dark
            ? ChartThemeVariant.Dark
            : ChartThemeVariant.Light;

        ApplyTheme(control, chartVariant);
    }
}

/// <summary>
/// Chart theme variant enumeration.
/// </summary>
public enum ChartThemeVariant
{
    /// <summary>Light theme.</summary>
    Light,

    /// <summary>Dark theme.</summary>
    Dark,

    /// <summary>Professional theme.</summary>
    Professional,

    /// <summary>High contrast theme.</summary>
    HighContrast,

    /// <summary>Fluent light theme.</summary>
    FluentLight,

    /// <summary>Fluent dark theme.</summary>
    FluentDark
}
