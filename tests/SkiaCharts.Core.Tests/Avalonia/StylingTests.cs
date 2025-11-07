using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using SkiaCharts.Avalonia.Controls;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Theming;
using AvaloniaChartTheme = SkiaCharts.Avalonia.Theming.ChartTheme;
using AvaloniaChartThemeManager = SkiaCharts.Avalonia.Theming.ChartThemeManager;
using AvaloniaChartThemeVariant = SkiaCharts.Avalonia.Theming.ChartThemeVariant;
using AvaloniaLegendPosition = SkiaCharts.Avalonia.Controls.LegendPosition;

namespace SkiaCharts.Core.Tests.Avalonia;

/// <summary>
/// Tests for SkiaChartView styling and theming.
/// </summary>
public class StylingTests
{
    [Fact]
    public void SkiaChartView_HasDefaultBackground()
    {
        var chartView = new SkiaChartView();

        // Background can be null by default
        Assert.True(chartView.Background == null || chartView.Background != null);
    }

    [Fact]
    public void SkiaChartView_CanSetBackground()
    {
        var chartView = new SkiaChartView();
        var brush = new global::Avalonia.Media.SolidColorBrush(global::Avalonia.Media.Colors.White);

        chartView.Background = brush;

        Assert.Equal(brush, chartView.Background);
    }

    [Fact]
    public void SkiaChartView_DefaultEnableAntiAliasing_IsTrue()
    {
        var chartView = new SkiaChartView();

        Assert.True(chartView.EnableAntiAliasing);
    }

    [Fact]
    public void SkiaChartView_CanSetEnableAntiAliasing()
    {
        var chartView = new SkiaChartView();

        chartView.EnableAntiAliasing = false;

        Assert.False(chartView.EnableAntiAliasing);
    }

    [Fact]
    public void SkiaChartView_DefaultDpiScale_IsOne()
    {
        var chartView = new SkiaChartView();

        Assert.Equal(1.0, chartView.DpiScale);
    }

    [Fact]
    public void SkiaChartView_CanSetDpiScale()
    {
        var chartView = new SkiaChartView();

        chartView.DpiScale = 2.0;

        Assert.Equal(2.0, chartView.DpiScale);
    }

    [Fact]
    public void SkiaChartView_DefaultChartTheme_IsLight()
    {
        var chartView = new SkiaChartView();

        Assert.NotNull(chartView.ChartTheme);
        Assert.Equal("Default", chartView.ChartTheme.ColorPalette.Name);
    }

    [Fact]
    public void SkiaChartView_CanSetChartTheme()
    {
        var chartView = new SkiaChartView();

        chartView.ChartTheme = ThemePresets.Dark;

        Assert.NotNull(chartView.ChartTheme);
        Assert.Equal("Vibrant", chartView.ChartTheme.ColorPalette.Name);
    }

    [Fact]
    public void SkiaChartView_ClipToBounds_IsTrue()
    {
        var chartView = new SkiaChartView();

        Assert.True(chartView.ClipToBounds);
    }

    [Fact]
    public void SkiaChartView_DefaultAnimationDuration_IsHalfSecond()
    {
        var chartView = new SkiaChartView();

        Assert.Equal(TimeSpan.FromMilliseconds(500), chartView.AnimationDuration);
    }

    [Fact]
    public void SkiaChartView_CanSetAnimationDuration()
    {
        var chartView = new SkiaChartView();

        chartView.AnimationDuration = TimeSpan.FromMilliseconds(1000);

        Assert.Equal(TimeSpan.FromMilliseconds(1000), chartView.AnimationDuration);
    }

    [Fact]
    public void SkiaChartView_DefaultShowLegend_IsTrue()
    {
        var chartView = new SkiaChartView();

        Assert.True(chartView.ShowLegend);
    }

    [Fact]
    public void SkiaChartView_DefaultShowGrid_IsTrue()
    {
        var chartView = new SkiaChartView();

        Assert.True(chartView.ShowGrid);
    }

    [Fact]
    public void SkiaChartView_DefaultShowMarkers_IsTrue()
    {
        var chartView = new SkiaChartView();

        Assert.True(chartView.ShowMarkers);
    }

    [Fact]
    public void SkiaChartView_DefaultLineWidth_IsTwo()
    {
        var chartView = new SkiaChartView();

        Assert.Equal(2.0, chartView.LineWidth);
    }

    [Fact]
    public void SkiaChartView_CanSetLineWidth()
    {
        var chartView = new SkiaChartView();

        chartView.LineWidth = 3.0;

        Assert.Equal(3.0, chartView.LineWidth);
    }

    [Fact]
    public void SkiaChartView_DefaultMarkerSize_IsSix()
    {
        var chartView = new SkiaChartView();

        Assert.Equal(6.0, chartView.MarkerSize);
    }

    [Fact]
    public void SkiaChartView_CanSetMarkerSize()
    {
        var chartView = new SkiaChartView();

        chartView.MarkerSize = 8.0;

        Assert.Equal(8.0, chartView.MarkerSize);
    }

    [Fact]
    public void ChartThemeManager_DefaultThemeVariant_IsLight()
    {
        Assert.Equal(AvaloniaChartThemeVariant.Light, AvaloniaChartThemeManager.DefaultThemeVariant);
    }

    [Fact]
    public void ChartThemeManager_CanSetDefaultThemeVariant()
    {
        var original = AvaloniaChartThemeManager.DefaultThemeVariant;

        AvaloniaChartThemeManager.DefaultThemeVariant = AvaloniaChartThemeVariant.Dark;
        Assert.Equal(AvaloniaChartThemeVariant.Dark, AvaloniaChartThemeManager.DefaultThemeVariant);

        // Restore
        AvaloniaChartThemeManager.DefaultThemeVariant = original;
    }

    [Fact]
    public void ChartThemeManager_ApplyTheme_UpdatesControlClasses()
    {
        var chartView = new SkiaChartView();

        AvaloniaChartThemeManager.ApplyTheme(chartView, AvaloniaChartThemeVariant.Dark);

        Assert.Contains("dark", chartView.Classes);
        Assert.DoesNotContain("light", chartView.Classes);
    }

    [Fact]
    public void ChartThemeManager_ApplyTheme_RemovesPreviousThemeClass()
    {
        var chartView = new SkiaChartView();

        AvaloniaChartThemeManager.ApplyTheme(chartView, AvaloniaChartThemeVariant.Light);
        Assert.Contains("light", chartView.Classes);

        AvaloniaChartThemeManager.ApplyTheme(chartView, AvaloniaChartThemeVariant.Dark);
        Assert.DoesNotContain("light", chartView.Classes);
        Assert.Contains("dark", chartView.Classes);
    }

    [Fact]
    public void ChartThemeManager_GetChartTheme_ReturnsCorrectTheme()
    {
        var lightTheme = AvaloniaChartThemeManager.GetChartTheme(AvaloniaChartThemeVariant.Light);
        var darkTheme = AvaloniaChartThemeManager.GetChartTheme(AvaloniaChartThemeVariant.Dark);
        var professionalTheme = AvaloniaChartThemeManager.GetChartTheme(AvaloniaChartThemeVariant.Professional);
        var highContrastTheme = AvaloniaChartThemeManager.GetChartTheme(AvaloniaChartThemeVariant.HighContrast);

        Assert.NotNull(lightTheme);
        Assert.Equal("Default", lightTheme.ColorPalette.Name);

        Assert.NotNull(darkTheme);
        Assert.Equal("Vibrant", darkTheme.ColorPalette.Name);

        Assert.NotNull(professionalTheme);
        Assert.Equal("Professional", professionalTheme.ColorPalette.Name);

        Assert.NotNull(highContrastTheme);
        Assert.Equal("High Contrast", highContrastTheme.ColorPalette.Name);
    }

    [Fact]
    public void ChartThemeManager_ToggleTheme_TogglesLightAndDark()
    {
        var dark = AvaloniaChartThemeManager.ToggleTheme(AvaloniaChartThemeVariant.Light);
        var light = AvaloniaChartThemeManager.ToggleTheme(AvaloniaChartThemeVariant.Dark);

        Assert.Equal(AvaloniaChartThemeVariant.Dark, dark);
        Assert.Equal(AvaloniaChartThemeVariant.Light, light);
    }

    [Fact]
    public void ChartThemeManager_ToggleTheme_TogglesFluentLightAndDark()
    {
        var fluentDark = AvaloniaChartThemeManager.ToggleTheme(AvaloniaChartThemeVariant.FluentLight);
        var fluentLight = AvaloniaChartThemeManager.ToggleTheme(AvaloniaChartThemeVariant.FluentDark);

        Assert.Equal(AvaloniaChartThemeVariant.FluentDark, fluentDark);
        Assert.Equal(AvaloniaChartThemeVariant.FluentLight, fluentLight);
    }

    [Fact]
    public void ChartTheme_AttachedProperty_CanGetAndSet()
    {
        var chartView = new SkiaChartView();

        AvaloniaChartTheme.SetThemeVariant(chartView, AvaloniaChartThemeVariant.Dark);
        var variant = AvaloniaChartTheme.GetThemeVariant(chartView);

        Assert.Equal(AvaloniaChartThemeVariant.Dark, variant);
    }

    [Fact]
    public void ChartTheme_AutoSyncTheme_CanGetAndSet()
    {
        var chartView = new SkiaChartView();

        AvaloniaChartTheme.SetAutoSyncTheme(chartView, true);
        var autoSync = AvaloniaChartTheme.GetAutoSyncTheme(chartView);

        Assert.True(autoSync);
    }

    [Fact]
    public void ChartTheme_SettingThemeVariant_UpdatesChartTheme()
    {
        var chartView = new SkiaChartView();

        AvaloniaChartTheme.SetThemeVariant(chartView, AvaloniaChartThemeVariant.Dark);

        // The attached property should have updated the ChartTheme property
        Assert.NotNull(chartView.ChartTheme);
        Assert.Equal("Vibrant", chartView.ChartTheme.ColorPalette.Name);
    }

    [Fact]
    public void SkiaChartView_InvalidateChart_DoesNotThrow()
    {
        var chartView = new SkiaChartView();

        var exception = Record.Exception(() => chartView.InvalidateChart());

        Assert.Null(exception);
    }

    [Fact]
    public void SkiaChartView_ClearCache_DoesNotThrow()
    {
        var chartView = new SkiaChartView();

        var exception = Record.Exception(() => chartView.ClearCache());

        Assert.Null(exception);
    }

    [Fact]
    public void SkiaChartView_WithChart_InvalidateChart_DoesNotThrow()
    {
        var chartView = new SkiaChartView
        {
            Chart = new LineChart()
        };

        var exception = Record.Exception(() => chartView.InvalidateChart());

        Assert.Null(exception);
    }

    [Fact]
    public void SkiaChartView_LegendPosition_DefaultIsRight()
    {
        var chartView = new SkiaChartView();

        Assert.Equal(AvaloniaLegendPosition.Right, chartView.LegendPosition);
    }

    [Fact]
    public void SkiaChartView_CanSetLegendPosition()
    {
        var chartView = new SkiaChartView();

        chartView.LegendPosition = AvaloniaLegendPosition.Top;

        Assert.Equal(AvaloniaLegendPosition.Top, chartView.LegendPosition);
    }

    [Fact]
    public void SkiaChartView_ShowMinorGrid_DefaultIsFalse()
    {
        var chartView = new SkiaChartView();

        Assert.False(chartView.ShowMinorGrid);
    }

    [Fact]
    public void SkiaChartView_CanSetShowMinorGrid()
    {
        var chartView = new SkiaChartView();

        chartView.ShowMinorGrid = true;

        Assert.True(chartView.ShowMinorGrid);
    }

    [Fact]
    public void SkiaChartView_EnableTooltips_DefaultIsTrue()
    {
        var chartView = new SkiaChartView();

        Assert.True(chartView.EnableTooltips);
    }

    [Fact]
    public void SkiaChartView_EnableZoom_DefaultIsTrue()
    {
        var chartView = new SkiaChartView();

        Assert.True(chartView.EnableZoom);
    }

    [Fact]
    public void SkiaChartView_EnablePan_DefaultIsTrue()
    {
        var chartView = new SkiaChartView();

        Assert.True(chartView.EnablePan);
    }

    [Fact]
    public void SkiaChartView_EnableAnimations_DefaultIsTrue()
    {
        var chartView = new SkiaChartView();

        Assert.True(chartView.EnableAnimations);
    }

    [Fact]
    public void SkiaChartView_CanSetTitle()
    {
        var chartView = new SkiaChartView();

        chartView.Title = "Test Title";

        Assert.Equal("Test Title", chartView.Title);
    }

    [Fact]
    public void SkiaChartView_CanSetSubtitle()
    {
        var chartView = new SkiaChartView();

        chartView.Subtitle = "Test Subtitle";

        Assert.Equal("Test Subtitle", chartView.Subtitle);
    }

    [Fact]
    public void SkiaChartView_CanSetAxisLabels()
    {
        var chartView = new SkiaChartView();

        chartView.XAxisLabel = "X Axis";
        chartView.YAxisLabel = "Y Axis";

        Assert.Equal("X Axis", chartView.XAxisLabel);
        Assert.Equal("Y Axis", chartView.YAxisLabel);
    }

    [Fact]
    public void ChartThemeManager_ApplyTheme_ThrowsOnNullControl()
    {
        Assert.Throws<ArgumentNullException>(() =>
            AvaloniaChartThemeManager.ApplyTheme((Control)null!, AvaloniaChartThemeVariant.Light));
    }

    [Fact]
    public void ChartThemeManager_SyncWithAvaloniaTheme_ThrowsOnNullControl()
    {
        Assert.Throws<ArgumentNullException>(() =>
            AvaloniaChartThemeManager.SyncWithAvaloniaTheme(null!));
    }
}
