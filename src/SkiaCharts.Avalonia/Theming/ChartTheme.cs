using Avalonia;
using Avalonia.Controls;
using SkiaCharts.Avalonia.Controls;

namespace SkiaCharts.Avalonia.Theming;

/// <summary>
/// Provides attached properties for chart theming.
/// </summary>
public static class ChartTheme
{
    /// <summary>
    /// Defines the ThemeVariant attached property.
    /// </summary>
    public static readonly AttachedProperty<ChartThemeVariant> ThemeVariantProperty =
        AvaloniaProperty.RegisterAttached<Control, ChartThemeVariant>(
            "ThemeVariant",
            typeof(ChartTheme),
            ChartThemeVariant.Light);

    /// <summary>
    /// Defines the AutoSyncTheme attached property.
    /// </summary>
    public static readonly AttachedProperty<bool> AutoSyncThemeProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>(
            "AutoSyncTheme",
            typeof(ChartTheme),
            false);

    /// <summary>
    /// Gets the ThemeVariant attached property value.
    /// </summary>
    public static ChartThemeVariant GetThemeVariant(Control control)
    {
        return control.GetValue(ThemeVariantProperty);
    }

    /// <summary>
    /// Sets the ThemeVariant attached property value.
    /// </summary>
    public static void SetThemeVariant(Control control, ChartThemeVariant value)
    {
        control.SetValue(ThemeVariantProperty, value);
    }

    /// <summary>
    /// Gets the AutoSyncTheme attached property value.
    /// </summary>
    public static bool GetAutoSyncTheme(Control control)
    {
        return control.GetValue(AutoSyncThemeProperty);
    }

    /// <summary>
    /// Sets the AutoSyncTheme attached property value.
    /// </summary>
    public static void SetAutoSyncTheme(Control control, bool value)
    {
        control.SetValue(AutoSyncThemeProperty, value);
    }

    static ChartTheme()
    {
        ThemeVariantProperty.Changed.AddClassHandler<Control>(OnThemeVariantChanged);
        AutoSyncThemeProperty.Changed.AddClassHandler<Control>(OnAutoSyncThemeChanged);
    }

    private static void OnThemeVariantChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        var variant = (ChartThemeVariant)e.NewValue!;
        ChartThemeManager.ApplyTheme(control, variant);

        // If the control is a SkiaChartView, also update its ChartTheme property
        if (control is SkiaChartView chartView)
        {
            chartView.ChartTheme = ChartThemeManager.GetChartTheme(variant);
        }
    }

    private static void OnAutoSyncThemeChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        var autoSync = (bool)e.NewValue!;

        if (autoSync)
        {
            // Sync immediately
            ChartThemeManager.SyncWithAvaloniaTheme(control);

            // Subscribe to theme changes
            control.PropertyChanged += OnControlPropertyChanged;
        }
        else
        {
            // Unsubscribe from theme changes
            control.PropertyChanged -= OnControlPropertyChanged;
        }
    }

    private static void OnControlPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property.Name == nameof(Control.ActualThemeVariant) && sender is Control control)
        {
            if (GetAutoSyncTheme(control))
            {
                ChartThemeManager.SyncWithAvaloniaTheme(control);
            }
        }
    }
}
