using SkiaCharts.Avalonia.ViewModels;
using SkiaCharts.Core.Data;
using SkiaCharts.Core.Theming;

namespace SkiaCharts.Avalonia.DesignTime;

/// <summary>
/// Provides sample data for design-time preview.
/// </summary>
public static class DesignTimeDataProvider
{
    /// <summary>
    /// Creates a sample line chart view model.
    /// </summary>
    public static LineChartViewModel CreateSampleLineChart()
    {
        var viewModel = new LineChartViewModel
        {
            Title = "Sales Over Time",
            ChartTheme = ThemePresets.Light,
            ShowLegend = true,
            ShowGrid = true
        };

        // Add sample series
        viewModel.AddSeries("Product A", new IDataPoint[]
        {
            new DataPoint(1, 100),
            new DataPoint(2, 150),
            new DataPoint(3, 120),
            new DataPoint(4, 180),
            new DataPoint(5, 200),
            new DataPoint(6, 190)
        });

        viewModel.AddSeries("Product B", new IDataPoint[]
        {
            new DataPoint(1, 80),
            new DataPoint(2, 90),
            new DataPoint(3, 110),
            new DataPoint(4, 130),
            new DataPoint(5, 140),
            new DataPoint(6, 160)
        });

        return viewModel;
    }

    /// <summary>
    /// Creates a sample bar chart view model.
    /// </summary>
    public static BarChartViewModel CreateSampleBarChart()
    {
        var viewModel = new BarChartViewModel
        {
            Title = "Quarterly Revenue",
            ChartTheme = ThemePresets.Professional,
            ShowLegend = true,
            ShowGrid = true
        };

        viewModel.AddSeries("Q1", new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 120),
            new DataPoint(2, 90)
        });

        viewModel.AddSeries("Q2", new IDataPoint[]
        {
            new DataPoint(0, 150),
            new DataPoint(1, 130),
            new DataPoint(2, 140)
        });

        viewModel.AddSeries("Q3", new IDataPoint[]
        {
            new DataPoint(0, 180),
            new DataPoint(1, 160),
            new DataPoint(2, 170)
        });

        return viewModel;
    }

    /// <summary>
    /// Creates a sample pie chart view model.
    /// </summary>
    public static PieChartViewModel CreateSamplePieChart()
    {
        var viewModel = new PieChartViewModel
        {
            Title = "Market Share",
            ChartTheme = ThemePresets.Dark,
            ShowLegend = true
        };

        viewModel.AddSlice("Product A", 35);
        viewModel.AddSlice("Product B", 25);
        viewModel.AddSlice("Product C", 20);
        viewModel.AddSlice("Product D", 15);
        viewModel.AddSlice("Others", 5);

        return viewModel;
    }

    /// <summary>
    /// Creates sample data points with a trend.
    /// </summary>
    public static IEnumerable<DataPoint> CreateTrendData(int count, double startValue, double trend, double volatility = 0.1)
    {
        var random = new Random(42); // Fixed seed for consistency
        var points = new List<DataPoint>();

        for (int i = 0; i < count; i++)
        {
            var baseValue = startValue + (i * trend);
            var noise = (random.NextDouble() - 0.5) * volatility * baseValue;
            var value = Math.Max(0, baseValue + noise);
            points.Add(new DataPoint(i, value));
        }

        return points;
    }

    /// <summary>
    /// Creates sample seasonal data.
    /// </summary>
    public static IEnumerable<DataPoint> CreateSeasonalData(int count, double amplitude, double baseline)
    {
        var points = new List<DataPoint>();

        for (int i = 0; i < count; i++)
        {
            var seasonalFactor = Math.Sin(i * Math.PI / 6); // 12-month cycle
            var value = baseline + (amplitude * seasonalFactor);
            points.Add(new DataPoint(i, value));
        }

        return points;
    }

    /// <summary>
    /// Creates sample random walk data.
    /// </summary>
    public static IEnumerable<DataPoint> CreateRandomWalkData(int count, double startValue, double stepSize)
    {
        var random = new Random(42);
        var points = new List<DataPoint>();
        var currentValue = startValue;

        for (int i = 0; i < count; i++)
        {
            var step = (random.NextDouble() - 0.5) * stepSize;
            currentValue += step;
            points.Add(new DataPoint(i, Math.Max(0, currentValue)));
        }

        return points;
    }
}

/// <summary>
/// Design-time view model locator.
/// </summary>
public class DesignTimeViewModelLocator
{
    /// <summary>
    /// Gets a sample line chart view model.
    /// </summary>
    public static LineChartViewModel LineChart => DesignTimeDataProvider.CreateSampleLineChart();

    /// <summary>
    /// Gets a sample bar chart view model.
    /// </summary>
    public static BarChartViewModel BarChart => DesignTimeDataProvider.CreateSampleBarChart();

    /// <summary>
    /// Gets a sample pie chart view model.
    /// </summary>
    public static PieChartViewModel PieChart => DesignTimeDataProvider.CreateSamplePieChart();
}
