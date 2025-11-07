using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaCharts.Core.Theming;

namespace SkiaCharts.Avalonia.ViewModels;

/// <summary>
/// Base class for chart view models with property change notification.
/// </summary>
public abstract class ChartViewModelBase : INotifyPropertyChanged
{
    private ChartBase? _chart;
    private ChartTheme _chartTheme = ThemePresets.Light;
    private string _title = string.Empty;
    private bool _showLegend = true;
    private bool _showGrid = true;
    private bool _enableAnimation = true;

    /// <summary>
    /// Gets or sets the chart instance.
    /// </summary>
    public ChartBase? Chart
    {
        get => _chart;
        set => SetProperty(ref _chart, value);
    }

    /// <summary>
    /// Gets or sets the chart theme.
    /// </summary>
    public ChartTheme ChartTheme
    {
        get => _chartTheme;
        set => SetProperty(ref _chartTheme, value);
    }

    /// <summary>
    /// Gets or sets the chart title.
    /// </summary>
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    /// <summary>
    /// Gets or sets whether the legend is visible.
    /// </summary>
    public bool ShowLegend
    {
        get => _showLegend;
        set => SetProperty(ref _showLegend, value);
    }

    /// <summary>
    /// Gets or sets whether the grid is visible.
    /// </summary>
    public bool ShowGrid
    {
        get => _showGrid;
        set => SetProperty(ref _showGrid, value);
    }

    /// <summary>
    /// Gets or sets whether animations are enabled.
    /// </summary>
    public bool EnableAnimation
    {
        get => _enableAnimation;
        set => SetProperty(ref _enableAnimation, value);
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Sets a property value and raises PropertyChanged if it changed.
    /// </summary>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Refreshes the chart.
    /// </summary>
    public virtual void Refresh()
    {
        OnPropertyChanged(nameof(Chart));
    }
}

/// <summary>
/// View model for line charts.
/// </summary>
public class LineChartViewModel : ChartViewModelBase
{
    private ObservableCollection<DataSeries<IDataPoint>> _series = new();

    /// <summary>
    /// Gets the collection of data series.
    /// </summary>
    public ObservableCollection<DataSeries<IDataPoint>> Series
    {
        get => _series;
        set => SetProperty(ref _series, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineChartViewModel"/> class.
    /// </summary>
    public LineChartViewModel()
    {
        Chart = new LineChart();
        Series.CollectionChanged += (_, _) => Refresh();
    }

    /// <summary>
    /// Adds a data series to the chart.
    /// </summary>
    public void AddSeries(string name, IEnumerable<IDataPoint> points)
    {
        var series = new DataSeries<IDataPoint>(points, name);
        Series.Add(series);
    }

    /// <summary>
    /// Clears all series.
    /// </summary>
    public void ClearSeries()
    {
        Series.Clear();
    }
}

/// <summary>
/// View model for bar charts.
/// </summary>
public class BarChartViewModel : ChartViewModelBase
{
    private ObservableCollection<DataSeries<IDataPoint>> _series = new();

    /// <summary>
    /// Gets the collection of data series.
    /// </summary>
    public ObservableCollection<DataSeries<IDataPoint>> Series
    {
        get => _series;
        set => SetProperty(ref _series, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BarChartViewModel"/> class.
    /// </summary>
    public BarChartViewModel()
    {
        Chart = new BarChart();
        Series.CollectionChanged += (_, _) => Refresh();
    }

    /// <summary>
    /// Adds a data series to the chart.
    /// </summary>
    public void AddSeries(string name, IEnumerable<IDataPoint> points)
    {
        var series = new DataSeries<IDataPoint>(points, name);
        Series.Add(series);
    }

    /// <summary>
    /// Clears all series.
    /// </summary>
    public void ClearSeries()
    {
        Series.Clear();
    }
}

/// <summary>
/// View model for pie charts.
/// </summary>
public class PieChartViewModel : ChartViewModelBase
{
    private ObservableCollection<PieSlice> _slices = new();

    /// <summary>
    /// Gets the collection of pie slices.
    /// </summary>
    public ObservableCollection<PieSlice> Slices
    {
        get => _slices;
        set => SetProperty(ref _slices, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PieChartViewModel"/> class.
    /// </summary>
    public PieChartViewModel()
    {
        Slices.CollectionChanged += (_, _) => Refresh();
    }

    /// <summary>
    /// Adds a pie slice.
    /// </summary>
    public void AddSlice(string label, double value)
    {
        Slices.Add(new PieSlice(label, value));
    }

    /// <summary>
    /// Clears all slices.
    /// </summary>
    public void ClearSlices()
    {
        Slices.Clear();
    }
}

/// <summary>
/// Represents a pie chart slice.
/// </summary>
public class PieSlice : INotifyPropertyChanged
{
    private string _label;
    private double _value;
    private bool _isSelected;

    /// <summary>
    /// Gets or sets the slice label.
    /// </summary>
    public string Label
    {
        get => _label;
        set
        {
            if (_label != value)
            {
                _label = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the slice value.
    /// </summary>
    public double Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets whether this slice is selected.
    /// </summary>
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PieSlice"/> class.
    /// </summary>
    public PieSlice(string label, double value)
    {
        _label = label;
        _value = value;
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
