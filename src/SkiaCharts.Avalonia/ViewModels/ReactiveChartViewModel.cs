using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaCharts.Core.Theming;

namespace SkiaCharts.Avalonia.ViewModels;

/// <summary>
/// ReactiveUI-based chart view model with observable properties and commands.
/// </summary>
public class ReactiveChartViewModel : ReactiveObject
{
    private ChartBase? _chart;
    private ChartTheme _chartTheme = ThemePresets.Light;
    private string _title = string.Empty;
    private string _subtitle = string.Empty;
    private bool _showLegend = true;
    private bool _showGrid = true;
    private bool _showMinorGrid;
    private bool _enableAnimations = true;
    private TimeSpan _animationDuration = TimeSpan.FromMilliseconds(500);
    private string _xAxisLabel = string.Empty;
    private string _yAxisLabel = string.Empty;
    private double _lineWidth = 2.0;
    private double _markerSize = 6.0;
    private bool _showMarkers = true;
    private bool _isLoading;
    private string? _errorMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveChartViewModel"/> class.
    /// </summary>
    public ReactiveChartViewModel()
    {
        // Create commands
        RefreshCommand = ReactiveCommand.Create(ExecuteRefresh);
        ClearDataCommand = ReactiveCommand.Create(ExecuteClearData);
        ExportCommand = ReactiveCommand.CreateFromTask<string>(ExecuteExportAsync);
        LoadDataCommand = ReactiveCommand.CreateFromTask(ExecuteLoadDataAsync);

        // Subscribe to property changes
        this.WhenAnyValue(x => x.Title, x => x.Subtitle)
            .Subscribe(_ => OnTitleChanged());

        this.WhenAnyValue(x => x.ShowLegend, x => x.ShowGrid)
            .Subscribe(_ => OnDisplaySettingsChanged());

        // Handle errors from commands
        LoadDataCommand.ThrownExceptions.Subscribe(ex =>
        {
            ErrorMessage = ex.Message;
            IsLoading = false;
        });
    }

    #region Properties

    /// <summary>
    /// Gets or sets the chart instance.
    /// </summary>
    public ChartBase? Chart
    {
        get => _chart;
        set => this.RaiseAndSetIfChanged(ref _chart, value);
    }

    /// <summary>
    /// Gets or sets the chart theme.
    /// </summary>
    public ChartTheme ChartTheme
    {
        get => _chartTheme;
        set => this.RaiseAndSetIfChanged(ref _chartTheme, value);
    }

    /// <summary>
    /// Gets or sets the chart title.
    /// </summary>
    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    /// <summary>
    /// Gets or sets the chart subtitle.
    /// </summary>
    public string Subtitle
    {
        get => _subtitle;
        set => this.RaiseAndSetIfChanged(ref _subtitle, value);
    }

    /// <summary>
    /// Gets or sets whether the legend is visible.
    /// </summary>
    public bool ShowLegend
    {
        get => _showLegend;
        set => this.RaiseAndSetIfChanged(ref _showLegend, value);
    }

    /// <summary>
    /// Gets or sets whether the grid is visible.
    /// </summary>
    public bool ShowGrid
    {
        get => _showGrid;
        set => this.RaiseAndSetIfChanged(ref _showGrid, value);
    }

    /// <summary>
    /// Gets or sets whether minor grid lines are visible.
    /// </summary>
    public bool ShowMinorGrid
    {
        get => _showMinorGrid;
        set => this.RaiseAndSetIfChanged(ref _showMinorGrid, value);
    }

    /// <summary>
    /// Gets or sets whether animations are enabled.
    /// </summary>
    public bool EnableAnimations
    {
        get => _enableAnimations;
        set => this.RaiseAndSetIfChanged(ref _enableAnimations, value);
    }

    /// <summary>
    /// Gets or sets the animation duration.
    /// </summary>
    public TimeSpan AnimationDuration
    {
        get => _animationDuration;
        set => this.RaiseAndSetIfChanged(ref _animationDuration, value);
    }

    /// <summary>
    /// Gets or sets the X-axis label.
    /// </summary>
    public string XAxisLabel
    {
        get => _xAxisLabel;
        set => this.RaiseAndSetIfChanged(ref _xAxisLabel, value);
    }

    /// <summary>
    /// Gets or sets the Y-axis label.
    /// </summary>
    public string YAxisLabel
    {
        get => _yAxisLabel;
        set => this.RaiseAndSetIfChanged(ref _yAxisLabel, value);
    }

    /// <summary>
    /// Gets or sets the line width for line charts.
    /// </summary>
    public double LineWidth
    {
        get => _lineWidth;
        set => this.RaiseAndSetIfChanged(ref _lineWidth, value);
    }

    /// <summary>
    /// Gets or sets the marker size.
    /// </summary>
    public double MarkerSize
    {
        get => _markerSize;
        set => this.RaiseAndSetIfChanged(ref _markerSize, value);
    }

    /// <summary>
    /// Gets or sets whether markers are shown on data points.
    /// </summary>
    public bool ShowMarkers
    {
        get => _showMarkers;
        set => this.RaiseAndSetIfChanged(ref _showMarkers, value);
    }

    /// <summary>
    /// Gets or sets whether data is currently loading.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    /// <summary>
    /// Gets or sets the error message, if any.
    /// </summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Gets the command to refresh the chart.
    /// </summary>
    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

    /// <summary>
    /// Gets the command to clear all data.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ClearDataCommand { get; }

    /// <summary>
    /// Gets the command to export the chart.
    /// </summary>
    public ReactiveCommand<string, Unit> ExportCommand { get; }

    /// <summary>
    /// Gets the command to load data.
    /// </summary>
    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }

    #endregion

    #region Command Implementations

    private void ExecuteRefresh()
    {
        // Trigger re-render by notifying chart changed
        this.RaisePropertyChanged(nameof(Chart));
    }

    private void ExecuteClearData()
    {
        if (Chart != null)
        {
            Chart.Series.Clear();
            this.RaisePropertyChanged(nameof(Chart));
        }
    }

    private async Task ExecuteExportAsync(string filePath)
    {
        // Placeholder for export functionality
        await Task.Delay(100);
        // TODO: Implement actual export logic
    }

    private async Task ExecuteLoadDataAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            // Placeholder for data loading
            await Task.Delay(500);
            // TODO: Implement actual data loading logic
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion

    #region Event Handlers

    private void OnTitleChanged()
    {
        // React to title or subtitle changes
        if (Chart != null)
        {
            Chart.Title = string.IsNullOrEmpty(Subtitle)
                ? Title
                : $"{Title} - {Subtitle}";
        }
    }

    private void OnDisplaySettingsChanged()
    {
        // React to display settings changes
        // Chart will be re-rendered automatically through property notifications
    }

    #endregion
}

/// <summary>
/// ReactiveUI-based view model for line charts.
/// </summary>
public class ReactiveLineChartViewModel : ReactiveChartViewModel
{
    private ObservableCollection<DataSeries<IDataPoint>> _series = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveLineChartViewModel"/> class.
    /// </summary>
    public ReactiveLineChartViewModel()
    {
        Chart = new LineChart();
        Series.CollectionChanged += (_, _) => UpdateChart();
    }

    /// <summary>
    /// Gets the collection of data series.
    /// </summary>
    public ObservableCollection<DataSeries<IDataPoint>> Series
    {
        get => _series;
        set => this.RaiseAndSetIfChanged(ref _series, value);
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

    private void UpdateChart()
    {
        if (Chart is LineChart lineChart)
        {
            lineChart.Series.Clear();
            foreach (var series in Series)
            {
                lineChart.Series.Add(series);
            }
            this.RaisePropertyChanged(nameof(Chart));
        }
    }
}

/// <summary>
/// ReactiveUI-based view model for bar charts.
/// </summary>
public class ReactiveBarChartViewModel : ReactiveChartViewModel
{
    private ObservableCollection<DataSeries<IDataPoint>> _series = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveBarChartViewModel"/> class.
    /// </summary>
    public ReactiveBarChartViewModel()
    {
        Chart = new BarChart();
        Series.CollectionChanged += (_, _) => UpdateChart();
    }

    /// <summary>
    /// Gets the collection of data series.
    /// </summary>
    public ObservableCollection<DataSeries<IDataPoint>> Series
    {
        get => _series;
        set => this.RaiseAndSetIfChanged(ref _series, value);
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

    private void UpdateChart()
    {
        if (Chart is BarChart barChart)
        {
            barChart.Series.Clear();
            foreach (var series in Series)
            {
                barChart.Series.Add(series);
            }
            this.RaisePropertyChanged(nameof(Chart));
        }
    }
}
