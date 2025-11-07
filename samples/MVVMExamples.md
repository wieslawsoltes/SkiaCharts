# SkiaCharts Avalonia MVVM Examples

This document provides examples of using SkiaCharts with Avalonia's MVVM pattern.

## Table of Contents
1. [Basic ViewModel](#basic-viewmodel)
2. [ReactiveUI ViewModel](#reactiveui-viewmodel)
3. [Data Binding](#data-binding)
4. [Commands](#commands)
5. [Value Converters](#value-converters)
6. [Dynamic Data Updates](#dynamic-data-updates)

## Basic ViewModel

### Using ChartViewModelBase

```csharp
using SkiaCharts.Avalonia.ViewModels;
using SkiaCharts.Core.Data;

public class MyChartViewModel : ChartViewModelBase
{
    public MyChartViewModel()
    {
        // Set chart properties
        Title = "Sales Data 2025";
        ShowLegend = true;
        ShowGrid = true;
        EnableAnimation = true;
        ChartTheme = ThemePresets.Professional;
    }

    public void LoadData()
    {
        var lineChart = new LineChart();

        // Add series
        var points = new IDataPoint[]
        {
            new DataPoint(1, 100),
            new DataPoint(2, 150),
            new DataPoint(3, 120),
            new DataPoint(4, 180),
            new DataPoint(5, 200)
        };

        var series = new DataSeries<IDataPoint>(points, "Q1 Sales");
        lineChart.Series.Add(series);

        Chart = lineChart;
    }
}
```

### XAML Binding

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:charts="clr-namespace:SkiaCharts.Avalonia.Controls;assembly=SkiaCharts.Avalonia">

    <charts:SkiaChartView
        Chart="{Binding Chart}"
        ChartTheme="{Binding ChartTheme}"
        Width="800"
        Height="600" />

</Window>
```

## ReactiveUI ViewModel

### Using ReactiveChartViewModel

```csharp
using SkiaCharts.Avalonia.ViewModels;
using ReactiveUI;
using System.Reactive;

public class MyReactiveViewModel : ReactiveLineChartViewModel
{
    public MyReactiveViewModel()
    {
        Title = "Interactive Chart";
        ShowLegend = true;

        // Setup reactive property observation
        this.WhenAnyValue(x => x.Title)
            .Subscribe(title => UpdateChartTitle(title));

        // Load initial data
        LoadSampleData();
    }

    private void LoadSampleData()
    {
        var points = new IDataPoint[]
        {
            new DataPoint(1, 100),
            new DataPoint(2, 150),
            new DataPoint(3, 120)
        };

        AddSeries("Sample Data", points);
    }

    private void UpdateChartTitle(string title)
    {
        if (Chart != null)
        {
            Chart.Title = title;
        }
    }
}
```

### XAML with Commands

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:charts="clr-namespace:SkiaCharts.Avalonia.Controls;assembly=SkiaCharts.Avalonia">

    <StackPanel>
        <!-- Chart -->
        <charts:SkiaChartView
            Chart="{Binding Chart}"
            Title="{Binding Title}"
            ShowLegend="{Binding ShowLegend}"
            ShowGrid="{Binding ShowGrid}"
            EnableAnimations="{Binding EnableAnimations}"
            AnimationDuration="{Binding AnimationDuration}"
            Width="800"
            Height="600" />

        <!-- Controls -->
        <StackPanel Orientation="Horizontal" Margin="10">
            <Button Content="Refresh" Command="{Binding RefreshCommand}" />
            <Button Content="Clear Data" Command="{Binding ClearDataCommand}" />
            <Button Content="Load Data" Command="{Binding LoadDataCommand}" />
        </StackPanel>

        <!-- Settings -->
        <StackPanel Margin="10">
            <CheckBox Content="Show Legend" IsChecked="{Binding ShowLegend}" />
            <CheckBox Content="Show Grid" IsChecked="{Binding ShowGrid}" />
            <CheckBox Content="Enable Animations" IsChecked="{Binding EnableAnimations}" />
        </StackPanel>
    </StackPanel>

</Window>
```

## Data Binding

### Binding to Series Collection

```csharp
public class SeriesBindingViewModel : LineChartViewModel
{
    public SeriesBindingViewModel()
    {
        // Series is an ObservableCollection - changes are automatically reflected
        Series.CollectionChanged += (s, e) =>
        {
            // Update chart when series changes
            Refresh();
        };
    }

    public void AddRandomSeries()
    {
        var random = new Random();
        var points = Enumerable.Range(1, 10)
            .Select(i => new DataPoint(i, random.Next(50, 200)))
            .Cast<IDataPoint>()
            .ToArray();

        AddSeries($"Series {Series.Count + 1}", points);
    }
}
```

### Binding to Axes

```csharp
public class AxisBindingViewModel : ChartViewModelBase
{
    private string _xAxisLabel = "Time (months)";
    private string _yAxisLabel = "Sales ($)";

    public string XAxisLabel
    {
        get => _xAxisLabel;
        set
        {
            if (SetProperty(ref _xAxisLabel, value) && Chart != null)
            {
                // Update X axis label
                Chart.XAxis.Title = value;
                Refresh();
            }
        }
    }

    public string YAxisLabel
    {
        get => _yAxisLabel;
        set
        {
            if (SetProperty(ref _yAxisLabel, value) && Chart != null)
            {
                // Update Y axis label
                Chart.YAxis.Title = value;
                Refresh();
            }
        }
    }
}
```

### Binding to Styles

```csharp
public class StyleBindingViewModel : LineChartViewModel
{
    private double _lineWidth = 2.0;
    private double _markerSize = 6.0;
    private bool _showMarkers = true;

    public double LineWidth
    {
        get => _lineWidth;
        set => SetProperty(ref _lineWidth, value);
    }

    public double MarkerSize
    {
        get => _markerSize;
        set => SetProperty(ref _markerSize, value);
    }

    public bool ShowMarkers
    {
        get => _showMarkers;
        set => SetProperty(ref _showMarkers, value);
    }
}
```

## Commands

### Command Implementation

```csharp
public class CommandViewModel : ReactiveChartViewModel
{
    public CommandViewModel()
    {
        // Commands are already defined in ReactiveChartViewModel

        // Add custom commands
        ExportToPngCommand = ReactiveCommand.CreateFromTask(ExecuteExportToPngAsync);
        ExportToSvgCommand = ReactiveCommand.CreateFromTask(ExecuteExportToSvgAsync);
    }

    public ReactiveCommand<Unit, Unit> ExportToPngCommand { get; }
    public ReactiveCommand<Unit, Unit> ExportToSvgCommand { get; }

    private async Task ExecuteExportToPngAsync()
    {
        // Implement PNG export
        await Task.CompletedTask;
    }

    private async Task ExecuteExportToSvgAsync()
    {
        // Implement SVG export
        await Task.CompletedTask;
    }
}
```

### XAML Command Binding

```xml
<StackPanel>
    <Menu>
        <MenuItem Header="File">
            <MenuItem Header="Export to PNG" Command="{Binding ExportToPngCommand}" />
            <MenuItem Header="Export to SVG" Command="{Binding ExportToSvgCommand}" />
        </MenuItem>
        <MenuItem Header="Data">
            <MenuItem Header="Refresh" Command="{Binding RefreshCommand}" />
            <MenuItem Header="Clear" Command="{Binding ClearDataCommand}" />
            <MenuItem Header="Load" Command="{Binding LoadDataCommand}" />
        </MenuItem>
    </Menu>

    <charts:SkiaChartView Chart="{Binding Chart}" />
</StackPanel>
```

## Value Converters

### Using Built-in Converters

```xml
<Window xmlns:converters="clr-namespace:SkiaCharts.Avalonia.Converters;assembly=SkiaCharts.Avalonia">

    <Window.Resources>
        <converters:ThemeNameConverter x:Key="ThemeConverter" />
        <converters:BoolToVisibilityConverter x:Key="VisibilityConverter" />
        <converters:TimeSpanToMillisecondsConverter x:Key="TimeConverter" />
    </Window.Resources>

    <!-- Theme selection -->
    <ComboBox SelectedItem="{Binding ThemeName, Converter={StaticResource ThemeConverter}}">
        <ComboBoxItem Content="Light" />
        <ComboBoxItem Content="Dark" />
        <ComboBoxItem Content="Professional" />
        <ComboBoxItem Content="High Contrast" />
    </ComboBox>

    <!-- Conditional visibility -->
    <TextBlock
        Text="Loading..."
        IsVisible="{Binding IsLoading, Converter={StaticResource VisibilityConverter}}" />

    <!-- Animation duration slider -->
    <Slider
        Minimum="0"
        Maximum="2000"
        Value="{Binding AnimationDuration, Converter={StaticResource TimeConverter}}" />

</Window>
```

## Dynamic Data Updates

### Real-time Data Updates

```csharp
public class RealtimeChartViewModel : LineChartViewModel
{
    private readonly Timer _timer;
    private readonly Random _random = new();
    private int _dataPoint = 0;

    public RealtimeChartViewModel()
    {
        Title = "Real-time Data";

        // Initialize with empty series
        AddSeries("Live Data", Array.Empty<IDataPoint>());

        // Start updating data
        _timer = new Timer(UpdateData, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    private void UpdateData(object? state)
    {
        _dataPoint++;
        var newValue = 100 + _random.Next(-20, 20);
        var newPoint = new DataPoint(_dataPoint, newValue);

        // Get current series
        if (Series.Count > 0)
        {
            var currentPoints = Series[0].ToList();
            currentPoints.Add(newPoint);

            // Keep only last 20 points
            if (currentPoints.Count > 20)
            {
                currentPoints.RemoveAt(0);
            }

            // Update series
            Series[0] = new DataSeries<IDataPoint>(
                currentPoints.Cast<IDataPoint>(),
                "Live Data");

            Refresh();
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
```

### User-driven Updates

```csharp
public class InteractiveViewModel : LineChartViewModel
{
    public void AddDataPoint(double x, double y)
    {
        if (Series.Count == 0)
        {
            AddSeries("User Data", Array.Empty<IDataPoint>());
        }

        var currentPoints = Series[0].ToList();
        currentPoints.Add(new DataPoint(x, y));

        Series[0] = new DataSeries<IDataPoint>(
            currentPoints.Cast<IDataPoint>(),
            "User Data");

        Refresh();
    }

    public void RemoveLastPoint()
    {
        if (Series.Count > 0)
        {
            var currentPoints = Series[0].ToList();
            if (currentPoints.Count > 0)
            {
                currentPoints.RemoveAt(currentPoints.Count - 1);

                Series[0] = new DataSeries<IDataPoint>(
                    currentPoints.Cast<IDataPoint>(),
                    "User Data");

                Refresh();
            }
        }
    }
}
```

## Complete Example

### Full Application Example

```csharp
// ViewModel
public class MainViewModel : ReactiveLineChartViewModel
{
    public MainViewModel()
    {
        Title = "Sales Dashboard";
        ChartTheme = ThemePresets.Professional;
        ShowLegend = true;
        ShowGrid = true;
        EnableAnimations = true;
        AnimationDuration = TimeSpan.FromMilliseconds(500);

        LoadInitialData();

        // Watch for property changes
        this.WhenAnyValue(
            x => x.ShowLegend,
            x => x.ShowGrid,
            x => x.EnableAnimations)
            .Subscribe(_ => RefreshCommand.Execute().Subscribe());
    }

    private void LoadInitialData()
    {
        var q1Data = new IDataPoint[]
        {
            new DataPoint(1, 100),
            new DataPoint(2, 150),
            new DataPoint(3, 120),
            new DataPoint(4, 180)
        };

        var q2Data = new IDataPoint[]
        {
            new DataPoint(1, 110),
            new DataPoint(2, 160),
            new DataPoint(3, 130),
            new DataPoint(4, 190)
        };

        AddSeries("Q1 2025", q1Data);
        AddSeries("Q2 2025", q2Data);
    }
}

// View (XAML)
/*
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="using:MyApp.ViewModels"
        xmlns:charts="clr-namespace:SkiaCharts.Avalonia.Controls;assembly=SkiaCharts.Avalonia"
        x:Class="MyApp.Views.MainWindow"
        Title="Sales Dashboard">

    <Design.DataContext>
        <vm:MainViewModel />
    </Design.DataContext>

    <Grid RowDefinitions="Auto,*">
        <!-- Toolbar -->
        <StackPanel Grid.Row="0" Orientation="Horizontal" Margin="10">
            <TextBlock Text="Title:" VerticalAlignment="Center" Margin="0,0,5,0" />
            <TextBox Text="{Binding Title}" Width="200" Margin="0,0,10,0" />

            <CheckBox Content="Show Legend" IsChecked="{Binding ShowLegend}" Margin="0,0,10,0" />
            <CheckBox Content="Show Grid" IsChecked="{Binding ShowGrid}" Margin="0,0,10,0" />
            <CheckBox Content="Animations" IsChecked="{Binding EnableAnimations}" Margin="0,0,10,0" />

            <Button Content="Refresh" Command="{Binding RefreshCommand}" Margin="0,0,5,0" />
            <Button Content="Clear" Command="{Binding ClearDataCommand}" />
        </StackPanel>

        <!-- Chart -->
        <charts:SkiaChartView
            Grid.Row="1"
            Chart="{Binding Chart}"
            ChartTheme="{Binding ChartTheme}"
            Margin="10" />
    </Grid>

</Window>
*/
```

## Best Practices

1. **Use ReactiveUI for complex interactions**
   - Leverage `WhenAnyValue` for property dependencies
   - Use `ReactiveCommand` for asynchronous operations
   - Handle command exceptions properly

2. **Optimize data updates**
   - Batch multiple changes before calling `Refresh()`
   - Use `ObservableCollection` for automatic UI updates
   - Consider data virtualization for large datasets

3. **Property change notifications**
   - Always use `SetProperty` helper in ViewModels
   - Implement `INotifyPropertyChanged` for all bindable objects
   - Be careful with circular dependencies

4. **Memory management**
   - Unsubscribe from events when disposing ViewModels
   - Dispose of timers and other resources
   - Use weak event patterns for long-lived subscriptions

5. **Testing**
   - Test ViewModels independently of the View
   - Mock chart data for unit tests
   - Use design-time data for XAML previews
