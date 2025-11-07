using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaCharts.Core.Theming;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.StylingDemos;

public class ThemeShowcaseViewModel : ReactiveObject
{
    private ChartTheme _selectedTheme;

    public ThemeShowcaseViewModel()
    {
        // Get all available themes
        Themes = new ObservableCollection<ChartTheme>(ThemePresets.All);
        _selectedTheme = ThemePresets.Light;

        // Create a sample line chart
        CreateChart();
    }

    public ObservableCollection<ChartTheme> Themes { get; }

    public ChartTheme SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedTheme, value);
            UpdateChartTheme();
        }
    }

    public LineChart Chart { get; private set; } = null!;

    private void CreateChart()
    {
        Chart = new LineChart
        {
            LineColor = SKColors.DodgerBlue,
            LineWidth = 2f,
            ShowMarkers = true,
            MarkerSize = 6f
        };

        // Create sample data - three series
        var series1 = CreateSeries("Revenue", 0);
        var series2 = CreateSeries("Expenses", 1);
        var series3 = CreateSeries("Profit", 2);

        Chart.Series.Add(series1);
        Chart.Series.Add(series2);
        Chart.Series.Add(series3);
    }

    private DataSeries<DataPoint> CreateSeries(string name, int offset)
    {
        var points = new List<DataPoint>();
        for (int i = 0; i <= 12; i++)
        {
            double x = i;
            double y = 50 + offset * 10 + (i * 3) + (offset * 5);
            points.Add(new DataPoint(x, y));
        }
        return new DataSeries<DataPoint>(points, name);
    }

    private void UpdateChartTheme()
    {
        // Theme information is displayed in the selector
        // In a full implementation, the theme would be applied to the chart
        this.RaisePropertyChanged(nameof(SelectedTheme));
    }
}
