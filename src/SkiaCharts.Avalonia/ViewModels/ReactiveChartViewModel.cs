using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaCharts.Core.Exporting;
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
    private ExportSettings _exportSettings = ExportSettings.ForWeb();
    private int _exportWidth = 1200;
    private int _exportHeight = 800;
    private int _samplePointCount = 50;
    private int _sampleSeriesCount = 2;
    private int _sampleSeed = 42;

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

        ExportCommand.ThrownExceptions.Subscribe(ex =>
        {
            ErrorMessage = ex.Message;
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

    /// <summary>
    /// Gets or sets export settings for chart exports.
    /// </summary>
    public ExportSettings ExportSettings
    {
        get => _exportSettings;
        set => this.RaiseAndSetIfChanged(ref _exportSettings, value);
    }

    /// <summary>
    /// Gets or sets the export width in pixels (96 DPI).
    /// </summary>
    public int ExportWidth
    {
        get => _exportWidth;
        set => this.RaiseAndSetIfChanged(ref _exportWidth, value);
    }

    /// <summary>
    /// Gets or sets the export height in pixels (96 DPI).
    /// </summary>
    public int ExportHeight
    {
        get => _exportHeight;
        set => this.RaiseAndSetIfChanged(ref _exportHeight, value);
    }

    /// <summary>
    /// Gets or sets the number of points generated for sample data.
    /// </summary>
    public int SamplePointCount
    {
        get => _samplePointCount;
        set => this.RaiseAndSetIfChanged(ref _samplePointCount, value);
    }

    /// <summary>
    /// Gets or sets the number of series generated for sample data.
    /// </summary>
    public int SampleSeriesCount
    {
        get => _sampleSeriesCount;
        set => this.RaiseAndSetIfChanged(ref _sampleSeriesCount, value);
    }

    /// <summary>
    /// Gets or sets the random seed used for sample data generation.
    /// </summary>
    public int SampleSeed
    {
        get => _sampleSeed;
        set => this.RaiseAndSetIfChanged(ref _sampleSeed, value);
    }

    /// <summary>
    /// Gets or sets a custom data loader for the LoadData command.
    /// </summary>
    public Func<CancellationToken, Task<IReadOnlyList<IDataSeries<IDataPoint>>>>? DataLoader { get; set; }

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
        if (Chart == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Export file path is required.", nameof(filePath));
        }

        var width = ExportWidth > 0 ? ExportWidth : 1200;
        var height = ExportHeight > 0 ? ExportHeight : 800;
        var settings = ExportSettings;

        await Task.Run(() => ChartExporter.Export(Chart, filePath, width, height, settings));
    }

    private async Task ExecuteLoadDataAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var series = DataLoader != null
                ? await DataLoader(CancellationToken.None)
                : CreateSampleSeries();

            if (Chart != null)
            {
                Chart.Series.Clear();
                foreach (var dataSeries in series)
                {
                    Chart.Series.Add(dataSeries);
                }
            }

            OnSeriesLoaded(series);
            this.RaisePropertyChanged(nameof(Chart));
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
            Chart.Title = Title;
            Chart.Subtitle = Subtitle;
        }
    }

    private void OnDisplaySettingsChanged()
    {
        // React to display settings changes
        // Chart will be re-rendered automatically through property notifications
    }

    #endregion

    #region Data Loading Helpers

    /// <summary>
    /// Allows derived view models to synchronize their own collections when data is loaded.
    /// </summary>
    protected virtual void OnSeriesLoaded(IReadOnlyList<IDataSeries<IDataPoint>> series)
    {
        // No-op by default
    }

    private IReadOnlyList<IDataSeries<IDataPoint>> CreateSampleSeries()
    {
        if (Chart is PieChart)
        {
            return new[]
            {
                new DataSeries<IDataPoint>(new IDataPoint[]
                {
                    new PieDataPoint(35, "Product A"),
                    new PieDataPoint(25, "Product B"),
                    new PieDataPoint(20, "Product C"),
                    new PieDataPoint(15, "Product D"),
                    new PieDataPoint(5, "Other")
                }, "Series 1")
            };
        }

        var random = new Random(SampleSeed);
        var seriesList = new List<IDataSeries<IDataPoint>>();
        var seriesCount = Math.Max(1, SampleSeriesCount);
        var pointCount = Math.Max(2, SamplePointCount);

        for (int seriesIndex = 0; seriesIndex < seriesCount; seriesIndex++)
        {
            var points = new List<IDataPoint>(pointCount);
            var value = random.NextDouble() * 50 + 25 * (seriesIndex + 1);

            for (int i = 0; i < pointCount; i++)
            {
                value += (random.NextDouble() - 0.5) * 10;
                points.Add(new DataPoint(i, Math.Max(0, value)));
            }

            seriesList.Add(new DataSeries<IDataPoint>(points, $"Series {seriesIndex + 1}"));
        }

        return seriesList;
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

    /// <inheritdoc/>
    protected override void OnSeriesLoaded(IReadOnlyList<IDataSeries<IDataPoint>> series)
    {
        Series.Clear();
        foreach (var dataSeries in series)
        {
            if (dataSeries is DataSeries<IDataPoint> typedSeries)
            {
                Series.Add(typedSeries);
            }
            else
            {
                Series.Add(new DataSeries<IDataPoint>(dataSeries, dataSeries.Name));
            }
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

    /// <inheritdoc/>
    protected override void OnSeriesLoaded(IReadOnlyList<IDataSeries<IDataPoint>> series)
    {
        Series.Clear();
        foreach (var dataSeries in series)
        {
            if (dataSeries is DataSeries<IDataPoint> typedSeries)
            {
                Series.Add(typedSeries);
            }
            else
            {
                Series.Add(new DataSeries<IDataPoint>(dataSeries, dataSeries.Name));
            }
        }
    }
}
