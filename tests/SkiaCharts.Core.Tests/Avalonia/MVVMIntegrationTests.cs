using Xunit;
using SkiaCharts.Avalonia.ViewModels;
using SkiaCharts.Core.Data;
using SkiaCharts.Core.Theming;
using System.ComponentModel;

namespace SkiaCharts.Core.Tests.Avalonia;

/// <summary>
/// Integration tests for MVVM support with ViewModels.
/// </summary>
public class MVVMIntegrationTests
{
    [Fact]
    public void ChartViewModelBase_ImplementsINotifyPropertyChanged()
    {
        // Arrange
        var viewModel = new LineChartViewModel();

        // Assert
        Assert.IsAssignableFrom<INotifyPropertyChanged>(viewModel);
    }

    [Fact]
    public void ChartViewModelBase_PropertyChangedEventFires()
    {
        // Arrange
        var viewModel = new LineChartViewModel();
        bool eventFired = false;
        string? propertyName = null;

        viewModel.PropertyChanged += (sender, args) =>
        {
            eventFired = true;
            propertyName = args.PropertyName;
        };

        // Act
        viewModel.Title = "New Title";

        // Assert
        Assert.True(eventFired);
        Assert.Equal(nameof(viewModel.Title), propertyName);
    }

    [Fact]
    public void LineChartViewModel_CanSetTitle()
    {
        // Arrange
        var viewModel = new LineChartViewModel();
        var title = "Test Chart";

        // Act
        viewModel.Title = title;

        // Assert
        Assert.Equal(title, viewModel.Title);
    }

    [Fact]
    public void LineChartViewModel_CanSetTheme()
    {
        // Arrange
        var viewModel = new LineChartViewModel();
        var theme = ThemePresets.Dark;

        // Act
        viewModel.ChartTheme = theme;

        // Assert
        Assert.Equal(theme, viewModel.ChartTheme);
    }

    [Fact]
    public void LineChartViewModel_CanToggleShowLegend()
    {
        // Arrange
        var viewModel = new LineChartViewModel();

        // Act
        viewModel.ShowLegend = false;

        // Assert
        Assert.False(viewModel.ShowLegend);
    }

    [Fact]
    public void LineChartViewModel_CanToggleShowGrid()
    {
        // Arrange
        var viewModel = new LineChartViewModel();

        // Act
        viewModel.ShowGrid = false;

        // Assert
        Assert.False(viewModel.ShowGrid);
    }

    [Fact]
    public void LineChartViewModel_CanToggleEnableAnimation()
    {
        // Arrange
        var viewModel = new LineChartViewModel();

        // Act
        viewModel.EnableAnimation = false;

        // Assert
        Assert.False(viewModel.EnableAnimation);
    }

    [Fact]
    public void LineChartViewModel_CanAddSeries()
    {
        // Arrange
        var viewModel = new LineChartViewModel();
        var points = new IDataPoint[]
        {
            new DataPoint(1, 100),
            new DataPoint(2, 150),
            new DataPoint(3, 120)
        };

        // Act
        viewModel.AddSeries("Test Series", points);

        // Assert
        Assert.Single(viewModel.Series);
        Assert.Equal("Test Series", viewModel.Series[0].Name);
    }

    [Fact]
    public void LineChartViewModel_CanClearSeries()
    {
        // Arrange
        var viewModel = new LineChartViewModel();
        var points = new IDataPoint[]
        {
            new DataPoint(1, 100)
        };
        viewModel.AddSeries("Test", points);

        // Act
        viewModel.ClearSeries();

        // Assert
        Assert.Empty(viewModel.Series);
    }

    [Fact]
    public void BarChartViewModel_CanAddSeries()
    {
        // Arrange
        var viewModel = new BarChartViewModel();
        var points = new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 150),
            new DataPoint(2, 120)
        };

        // Act
        viewModel.AddSeries("Test Series", points);

        // Assert
        Assert.Single(viewModel.Series);
    }

    [Fact]
    public void PieChartViewModel_CanAddSlice()
    {
        // Arrange
        var viewModel = new PieChartViewModel();

        // Act
        viewModel.AddSlice("Slice 1", 100);

        // Assert
        Assert.Single(viewModel.Slices);
        Assert.Equal("Slice 1", viewModel.Slices[0].Label);
        Assert.Equal(100, viewModel.Slices[0].Value);
    }

    [Fact]
    public void PieSlice_ImplementsINotifyPropertyChanged()
    {
        // Arrange
        var slice = new PieSlice("Test", 100);

        // Assert
        Assert.IsAssignableFrom<INotifyPropertyChanged>(slice);
    }

    [Fact]
    public void PieSlice_PropertyChangedEventFires()
    {
        // Arrange
        var slice = new PieSlice("Test", 100);
        bool eventFired = false;

        slice.PropertyChanged += (sender, args) =>
        {
            eventFired = true;
        };

        // Act
        slice.Value = 200;

        // Assert
        Assert.True(eventFired);
    }

    [Fact]
    public void PieSlice_CanSetIsSelected()
    {
        // Arrange
        var slice = new PieSlice("Test", 100);

        // Act
        slice.IsSelected = true;

        // Assert
        Assert.True(slice.IsSelected);
    }

    [Fact]
    public void ViewModel_SeriesObservableCollection_NotifiesChanges()
    {
        // Arrange
        var viewModel = new LineChartViewModel();
        bool collectionChanged = false;

        viewModel.Series.CollectionChanged += (sender, args) =>
        {
            collectionChanged = true;
        };

        // Act
        var points = new IDataPoint[] { new DataPoint(1, 100) };
        viewModel.AddSeries("Test", points);

        // Assert
        Assert.True(collectionChanged);
    }

    [Fact]
    public void ViewModel_MultiplePropertyChanges_AllNotified()
    {
        // Arrange
        var viewModel = new LineChartViewModel();
        var changedProperties = new List<string?>();

        viewModel.PropertyChanged += (sender, args) =>
        {
            changedProperties.Add(args.PropertyName);
        };

        // Act
        viewModel.Title = "New Title";
        viewModel.ShowLegend = false;
        viewModel.ShowGrid = false;

        // Assert
        Assert.Equal(3, changedProperties.Count);
        Assert.Contains(nameof(viewModel.Title), changedProperties);
        Assert.Contains(nameof(viewModel.ShowLegend), changedProperties);
        Assert.Contains(nameof(viewModel.ShowGrid), changedProperties);
    }

    [Fact]
    public void ViewModel_ChartProperty_CanBeSet()
    {
        // Arrange
        var viewModel = new LineChartViewModel();

        // Assert
        Assert.NotNull(viewModel.Chart);
    }

    [Fact]
    public void ViewModel_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var viewModel = new LineChartViewModel();

        // Assert
        Assert.Equal(string.Empty, viewModel.Title);
        Assert.True(viewModel.ShowLegend);
        Assert.True(viewModel.ShowGrid);
        Assert.True(viewModel.EnableAnimation);
        Assert.NotNull(viewModel.ChartTheme);
    }
}
