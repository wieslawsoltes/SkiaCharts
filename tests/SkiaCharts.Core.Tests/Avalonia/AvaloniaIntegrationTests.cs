using Xunit;

namespace SkiaCharts.Core.Tests.Avalonia;

/// <summary>
/// Integration tests for Avalonia controls.
/// Note: These are placeholder tests since we cannot fully test Avalonia controls
/// without the Avalonia test framework.
/// </summary>
public class AvaloniaIntegrationTests
{
    [Fact]
    public void AvaloniaProject_Exists()
    {
        // This test verifies that the Avalonia integration project structure is in place
        var avaloniaProjectPath = Path.Combine(
            Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.Parent!.Parent!.FullName,
            "src",
            "SkiaCharts.Avalonia",
            "SkiaCharts.Avalonia.csproj");

        Assert.True(File.Exists(avaloniaProjectPath), "Avalonia project file should exist");
    }

    [Fact]
    public void SkiaChartView_ControlFile_Exists()
    {
        // Verify the main control file exists
        var controlPath = Path.Combine(
            Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.Parent!.Parent!.FullName,
            "src",
            "SkiaCharts.Avalonia",
            "Controls",
            "SkiaChartView.cs");

        Assert.True(File.Exists(controlPath), "SkiaChartView.cs should exist");
    }

    [Fact]
    public void SkiaChartView_PropertiesFile_Exists()
    {
        // Verify the properties file exists
        var propertiesPath = Path.Combine(
            Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.Parent!.Parent!.FullName,
            "src",
            "SkiaCharts.Avalonia",
            "Controls",
            "SkiaChartView.Properties.cs");

        Assert.True(File.Exists(propertiesPath), "SkiaChartView.Properties.cs should exist");
    }

    [Fact]
    public void ViewModels_Directory_Exists()
    {
        // Verify ViewModels directory exists
        var viewModelsPath = Path.Combine(
            Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.Parent!.Parent!.FullName,
            "src",
            "SkiaCharts.Avalonia",
            "ViewModels");

        Assert.True(Directory.Exists(viewModelsPath), "ViewModels directory should exist");
    }

    [Fact]
    public void DesignTime_Directory_Exists()
    {
        // Verify DesignTime directory exists
        var designTimePath = Path.Combine(
            Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.Parent!.Parent!.FullName,
            "src",
            "SkiaCharts.Avalonia",
            "DesignTime");

        Assert.True(Directory.Exists(designTimePath), "DesignTime directory should exist");
    }

    [Fact]
    public void ChartViewModelBase_File_Exists()
    {
        // Verify ViewModel base file exists
        var viewModelPath = Path.Combine(
            Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.Parent!.Parent!.FullName,
            "src",
            "SkiaCharts.Avalonia",
            "ViewModels",
            "ChartViewModelBase.cs");

        Assert.True(File.Exists(viewModelPath), "ChartViewModelBase.cs should exist");
    }

    [Fact]
    public void DesignTimeDataProvider_File_Exists()
    {
        // Verify design-time data provider exists
        var providerPath = Path.Combine(
            Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.Parent!.Parent!.FullName,
            "src",
            "SkiaCharts.Avalonia",
            "DesignTime",
            "DesignTimeDataProvider.cs");

        Assert.True(File.Exists(providerPath), "DesignTimeDataProvider.cs should exist");
    }
}
