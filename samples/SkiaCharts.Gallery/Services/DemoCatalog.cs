using System.Collections.Generic;
using System.Linq;
using SkiaCharts.Gallery.Models;
using SkiaCharts.Gallery.ViewModels.BasicCharts;
using SkiaCharts.Gallery.Views.BasicCharts;
using SkiaCharts.Gallery.ViewModels.StylingDemos;
using SkiaCharts.Gallery.Views.StylingDemos;
using SkiaCharts.Gallery.ViewModels.MultiSeriesDemos;
using SkiaCharts.Gallery.Views.MultiSeriesDemos;
using SkiaCharts.Gallery.ViewModels.AxesDemos;
using SkiaCharts.Gallery.Views.AxesDemos;
using SkiaCharts.Gallery.ViewModels.FinancialDemos;
using SkiaCharts.Gallery.Views.FinancialDemos;
using SkiaCharts.Gallery.ViewModels.RealTimeDemos;
using SkiaCharts.Gallery.Views.RealTimeDemos;
using SkiaCharts.Gallery.ViewModels.InteractiveDemos;
using SkiaCharts.Gallery.Views.InteractiveDemos;
using SkiaCharts.Gallery.ViewModels.PerformanceDemos;
using SkiaCharts.Gallery.Views.PerformanceDemos;
using SkiaCharts.Gallery.ViewModels.CustomDemos;
using SkiaCharts.Gallery.Views.CustomDemos;

namespace SkiaCharts.Gallery.Services;

/// <summary>
/// Catalog of all available demos.
/// </summary>
public static class DemoCatalog
{
    private static List<DemoCategory>? _categories;

    /// <summary>
    /// Gets all demo categories.
    /// </summary>
    public static List<DemoCategory> Categories
    {
        get
        {
            if (_categories == null)
            {
                _categories = BuildCatalog();
            }
            return _categories;
        }
    }

    /// <summary>
    /// Gets all demos across all categories.
    /// </summary>
    public static IEnumerable<DemoPage> AllDemos =>
        Categories.SelectMany(c => c.Demos);

    /// <summary>
    /// Searches for demos matching the given query.
    /// </summary>
    public static IEnumerable<DemoPage> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return AllDemos;

        query = query.ToLowerInvariant();

        return AllDemos.Where(demo =>
            demo.Title.ToLowerInvariant().Contains(query) ||
            demo.Description.ToLowerInvariant().Contains(query) ||
            demo.Tags.Any(tag => tag.ToLowerInvariant().Contains(query)));
    }

    /// <summary>
    /// Gets a demo by its ID.
    /// </summary>
    public static DemoPage? GetDemo(string id)
    {
        return AllDemos.FirstOrDefault(d => d.Id == id);
    }

    /// <summary>
    /// Gets a category by its ID.
    /// </summary>
    public static DemoCategory? GetCategory(string id)
    {
        return Categories.FirstOrDefault(c => c.Id == id);
    }

    private static List<DemoCategory> BuildCatalog()
    {
        var categories = new List<DemoCategory>();

        // Basic Charts Category
        categories.Add(new DemoCategory
        {
            Id = "basic-charts",
            Name = "Basic Charts",
            Description = "Simple, single-series charts demonstrating fundamental chart types",
            Icon = "📈",
            Demos = new List<DemoPage>
            {
                new DemoPage
                {
                    Id = "simple-line",
                    Title = "Simple Line Chart",
                    Description = "A basic line chart with a single data series",
                    CategoryId = "basic-charts",
                    Tags = new[] { "line", "basic", "simple" },
                    Difficulty = 1,
                    ViewModelType = typeof(SimpleLineViewModel),
                    ViewType = typeof(SimpleLineView)
                },
                new DemoPage
                {
                    Id = "multi-line",
                    Title = "Multi-Line Chart",
                    Description = "Line chart with multiple data series",
                    CategoryId = "basic-charts",
                    Tags = new[] { "line", "multiple", "series" },
                    Difficulty = 1,
                    ViewModelType = typeof(MultiLineViewModel),
                    ViewType = typeof(MultiLineView)
                },
                new DemoPage
                {
                    Id = "column-chart",
                    Title = "Column Chart",
                    Description = "Vertical bar chart showing categorical data",
                    CategoryId = "basic-charts",
                    Tags = new[] { "column", "bar", "categorical" },
                    Difficulty = 1,
                    ViewModelType = typeof(ColumnChartViewModel),
                    ViewType = typeof(ColumnChartView)
                },
                new DemoPage
                {
                    Id = "bar-chart",
                    Title = "Bar Chart",
                    Description = "Horizontal bar chart for comparison",
                    CategoryId = "basic-charts",
                    Tags = new[] { "bar", "horizontal", "comparison" },
                    Difficulty = 1,
                    ViewModelType = typeof(BarChartViewModel),
                    ViewType = typeof(BarChartView)
                },
                new DemoPage
                {
                    Id = "pie-chart",
                    Title = "Pie Chart",
                    Description = "Circular chart showing proportions",
                    CategoryId = "basic-charts",
                    Tags = new[] { "pie", "circular", "proportion" },
                    Difficulty = 1,
                    ViewModelType = typeof(PieChartViewModel),
                    ViewType = typeof(PieChartView)
                },
                new DemoPage
                {
                    Id = "area-chart",
                    Title = "Area Chart",
                    Description = "Chart with filled area under the curve",
                    CategoryId = "basic-charts",
                    Tags = new[] { "area", "filled", "curve" },
                    Difficulty = 1,
                    ViewModelType = typeof(AreaChartViewModel),
                    ViewType = typeof(AreaChartView)
                },
                new DemoPage
                {
                    Id = "scatter-chart",
                    Title = "Scatter Chart",
                    Description = "Individual data points without connecting lines",
                    CategoryId = "basic-charts",
                    Tags = new[] { "scatter", "points", "distribution" },
                    Difficulty = 1,
                    ViewModelType = typeof(ScatterChartViewModel),
                    ViewType = typeof(ScatterChartView)
                },
                new DemoPage
                {
                    Id = "donut-chart",
                    Title = "Donut Chart",
                    Description = "Pie chart with hollow center",
                    CategoryId = "basic-charts",
                    Tags = new[] { "donut", "pie", "circular" },
                    Difficulty = 1,
                    ViewModelType = typeof(DonutChartViewModel),
                    ViewType = typeof(DonutChartView)
                },
                new DemoPage
                {
                    Id = "stacked-column",
                    Title = "Stacked Column Chart",
                    Description = "Vertical bars stacked on top of each other",
                    CategoryId = "basic-charts",
                    Tags = new[] { "stacked", "column", "multi-series" },
                    Difficulty = 2,
                    ViewModelType = typeof(StackedColumnViewModel),
                    ViewType = typeof(StackedColumnView)
                },
                new DemoPage
                {
                    Id = "grouped-bar",
                    Title = "Grouped Bar Chart",
                    Description = "Horizontal bars grouped side by side",
                    CategoryId = "basic-charts",
                    Tags = new[] { "grouped", "bar", "comparison" },
                    Difficulty = 2,
                    ViewModelType = typeof(GroupedBarViewModel),
                    ViewType = typeof(GroupedBarView)
                }
            }
        });

        // Styling Category
        categories.Add(new DemoCategory
        {
            Id = "styling",
            Name = "Styling & Theming",
            Description = "Customizing chart appearance with colors, fonts, and themes",
            Icon = "🎨",
            Demos = new List<DemoPage>
            {
                new DemoPage
                {
                    Id = "theme-showcase",
                    Title = "Theme Showcase",
                    Description = "Compare all built-in themes side by side",
                    CategoryId = "styling",
                    Tags = new[] { "theme", "color", "style" },
                    Difficulty = 1,
                    ViewModelType = typeof(ThemeShowcaseViewModel),
                    ViewType = typeof(ThemeShowcaseView)
                },
                new DemoPage
                {
                    Id = "color-palette",
                    Title = "Color Palettes",
                    Description = "Different color schemes and palettes",
                    CategoryId = "styling",
                    Tags = new[] { "color", "palette", "scheme" },
                    Difficulty = 2,
                    ViewModelType = typeof(ColorPaletteViewModel),
                    ViewType = typeof(ColorPaletteView)
                },
                new DemoPage
                {
                    Id = "gradient-fills",
                    Title = "Gradient Fills",
                    Description = "Charts with gradient color fills",
                    CategoryId = "styling",
                    Tags = new[] { "gradient", "color", "fill" },
                    Difficulty = 2,
                    ViewModelType = typeof(GradientFillsViewModel),
                    ViewType = typeof(GradientFillsView)
                },
                new DemoPage
                {
                    Id = "transparency",
                    Title = "Transparency",
                    Description = "Overlapping charts with transparency",
                    CategoryId = "styling",
                    Tags = new[] { "transparency", "alpha", "opacity" },
                    Difficulty = 2,
                    ViewModelType = typeof(TransparencyViewModel),
                    ViewType = typeof(TransparencyView)
                }
            }
        });

        // Multi-Series Category
        categories.Add(new DemoCategory
        {
            Id = "multi-series",
            Name = "Multi-Series Charts",
            Description = "Charts with multiple data series and complex layouts",
            Icon = "📊",
            Demos = new List<DemoPage>
            {
                new DemoPage
                {
                    Id = "many-series",
                    Title = "100+ Series Line Chart",
                    Description = "Performance test with 100 data series",
                    CategoryId = "multi-series",
                    Tags = new[] { "line", "performance", "many-series" },
                    Difficulty = 3,
                    ViewModelType = typeof(ManySeriesViewModel),
                    ViewType = typeof(ManySeriesView)
                },
                new DemoPage
                {
                    Id = "mixed-types",
                    Title = "Mixed Chart Types",
                    Description = "Combining different chart types in one view",
                    CategoryId = "multi-series",
                    Tags = new[] { "mixed", "combined", "line", "bar", "area" },
                    Difficulty = 2,
                    ViewModelType = typeof(MixedTypesViewModel),
                    ViewType = typeof(MixedTypesView)
                },
                new DemoPage
                {
                    Id = "stacked-area",
                    Title = "Stacked Area Chart",
                    Description = "Multiple area series stacked together",
                    CategoryId = "multi-series",
                    Tags = new[] { "area", "stacked", "cumulative" },
                    Difficulty = 2,
                    ViewModelType = typeof(StackedAreaViewModel),
                    ViewType = typeof(StackedAreaView)
                }
            }
        });

        // Axes Category
        categories.Add(new DemoCategory
        {
            Id = "axes",
            Name = "Axes & Scales",
            Description = "Different axis types and scale configurations",
            Icon = "📐",
            Demos = new List<DemoPage>
            {
                new DemoPage
                {
                    Id = "linear-axis",
                    Title = "Linear Axis",
                    Description = "Standard linear axis with evenly spaced intervals",
                    CategoryId = "axes",
                    Tags = new[] { "axis", "linear", "scale" },
                    Difficulty = 1,
                    ViewModelType = typeof(LinearAxisViewModel),
                    ViewType = typeof(LinearAxisView)
                },
                new DemoPage
                {
                    Id = "logarithmic-axis",
                    Title = "Logarithmic Axis",
                    Description = "Logarithmic scale for exponential data",
                    CategoryId = "axes",
                    Tags = new[] { "axis", "logarithmic", "log", "scale" },
                    Difficulty = 2,
                    ViewModelType = typeof(LogarithmicAxisViewModel),
                    ViewType = typeof(LogarithmicAxisView)
                },
                new DemoPage
                {
                    Id = "datetime-axis",
                    Title = "DateTime Axis",
                    Description = "Time-based axis for temporal data",
                    CategoryId = "axes",
                    Tags = new[] { "axis", "datetime", "time", "temporal" },
                    Difficulty = 2,
                    ViewModelType = typeof(DateTimeAxisViewModel),
                    ViewType = typeof(DateTimeAxisView)
                },
                new DemoPage
                {
                    Id = "category-axis",
                    Title = "Category Axis",
                    Description = "Discrete category axis for labeled data",
                    CategoryId = "axes",
                    Tags = new[] { "axis", "category", "discrete", "labels" },
                    Difficulty = 1,
                    ViewModelType = typeof(CategoryAxisViewModel),
                    ViewType = typeof(CategoryAxisView)
                }
            }
        });

        // Financial Category
        categories.Add(new DemoCategory
        {
            Id = "financial",
            Name = "Financial Charts",
            Description = "Specialized charts for financial data and trading",
            Icon = "💰",
            Demos = new List<DemoPage>
            {
                new DemoPage
                {
                    Id = "candlestick",
                    Title = "Candlestick Chart",
                    Description = "Traditional Japanese candlestick chart for OHLC data",
                    CategoryId = "financial",
                    Tags = new[] { "candlestick", "ohlc", "financial", "trading", "stock" },
                    Difficulty = 2,
                    ViewModelType = typeof(CandlestickViewModel),
                    ViewType = typeof(CandlestickView)
                },
                new DemoPage
                {
                    Id = "ohlc",
                    Title = "OHLC Chart",
                    Description = "OHLC bar chart with horizontal tick marks",
                    CategoryId = "financial",
                    Tags = new[] { "ohlc", "bar", "financial", "trading", "price" },
                    Difficulty = 2,
                    ViewModelType = typeof(OhlcViewModel),
                    ViewType = typeof(OhlcView)
                },
                new DemoPage
                {
                    Id = "volume",
                    Title = "Volume Chart",
                    Description = "Trading volume visualization",
                    CategoryId = "financial",
                    Tags = new[] { "volume", "trading", "financial", "bar" },
                    Difficulty = 1,
                    ViewModelType = typeof(VolumeViewModel),
                    ViewType = typeof(VolumeView)
                }
            }
        });

        // Real-Time Category
        categories.Add(new DemoCategory
        {
            Id = "realtime",
            Name = "Real-Time Updates",
            Description = "Live streaming data and continuous chart updates",
            Icon = "⚡",
            Demos = new List<DemoPage>
            {
                new DemoPage
                {
                    Id = "streaming-line",
                    Title = "Streaming Line Chart",
                    Description = "Real-time line chart with continuous updates",
                    CategoryId = "realtime",
                    Tags = new[] { "realtime", "streaming", "live", "line", "updates" },
                    Difficulty = 3,
                    ViewModelType = typeof(StreamingLineViewModel),
                    ViewType = typeof(StreamingLineView)
                },
                new DemoPage
                {
                    Id = "live-ticker",
                    Title = "Live Data Ticker",
                    Description = "Price ticker with real-time updates and status",
                    CategoryId = "realtime",
                    Tags = new[] { "ticker", "live", "realtime", "price", "financial" },
                    Difficulty = 3,
                    ViewModelType = typeof(LiveTickerViewModel),
                    ViewType = typeof(LiveTickerView)
                }
            }
        });

        // Performance Category
        categories.Add(new DemoCategory
        {
            Id = "performance",
            Name = "Performance",
            Description = "High-performance rendering with large datasets",
            Icon = "⚡",
            Demos = new List<DemoPage>
            {
                new DemoPage
                {
                    Id = "million-points",
                    Title = "1 Million Points",
                    Description = "Performance test with 1,000,000 data points",
                    CategoryId = "performance",
                    Tags = new[] { "performance", "million", "large", "stress-test" },
                    Difficulty = 3,
                    ViewModelType = typeof(MillionPointsViewModel),
                    ViewType = typeof(MillionPointsView)
                },
                new DemoPage
                {
                    Id = "scatter-100k",
                    Title = "100K Scatter Points",
                    Description = "Performance test with 100,000 scatter points",
                    CategoryId = "performance",
                    Tags = new[] { "performance", "scatter", "large", "100k" },
                    Difficulty = 3,
                    ViewModelType = typeof(ScatterPerformanceViewModel),
                    ViewType = typeof(ScatterPerformanceView)
                },
                new DemoPage
                {
                    Id = "streaming-performance",
                    Title = "Streaming Performance",
                    Description = "High-speed streaming at 60 FPS with performance metrics",
                    CategoryId = "performance",
                    Tags = new[] { "performance", "streaming", "fps", "realtime" },
                    Difficulty = 3,
                    ViewModelType = typeof(StreamingPerformanceViewModel),
                    ViewType = typeof(StreamingPerformanceView)
                },
                new DemoPage
                {
                    Id = "multi-series-performance",
                    Title = "Multi-Series Performance",
                    Description = "50 series × 10,000 points = 500,000 total points",
                    CategoryId = "performance",
                    Tags = new[] { "performance", "multi-series", "large", "500k" },
                    Difficulty = 3,
                    ViewModelType = typeof(MultiSeriesPerformanceViewModel),
                    ViewType = typeof(MultiSeriesPerformanceView)
                }
            }
        });

        // Custom Category
        categories.Add(new DemoCategory
        {
            Id = "custom",
            Name = "Customization & Extensibility",
            Description = "Custom themes, styling, and extensibility examples",
            Icon = "🎨",
            Demos = new List<DemoPage>
            {
                new DemoPage
                {
                    Id = "custom-theme",
                    Title = "Custom Theme",
                    Description = "Create custom chart themes with your own colors and styling",
                    CategoryId = "custom",
                    Tags = new[] { "custom", "theme", "styling", "colors" },
                    Difficulty = 2,
                    ViewModelType = typeof(CustomThemeViewModel),
                    ViewType = typeof(CustomThemeView)
                },
                new DemoPage
                {
                    Id = "custom-styling",
                    Title = "Advanced Customization",
                    Description = "Advanced customization with borders, corner radius, and spacing",
                    CategoryId = "custom",
                    Tags = new[] { "custom", "styling", "borders", "advanced" },
                    Difficulty = 2,
                    ViewModelType = typeof(CustomStylingViewModel),
                    ViewType = typeof(CustomStylingView)
                },
                new DemoPage
                {
                    Id = "custom-colors",
                    Title = "Custom Color Palette",
                    Description = "Define custom color palettes for multi-series charts",
                    CategoryId = "custom",
                    Tags = new[] { "custom", "colors", "palette", "multi-series" },
                    Difficulty = 2,
                    ViewModelType = typeof(CustomColorsViewModel),
                    ViewType = typeof(CustomColorsView)
                },
                new DemoPage
                {
                    Id = "custom-markers",
                    Title = "Custom Markers",
                    Description = "Customize marker shapes, sizes, colors, and borders",
                    CategoryId = "custom",
                    Tags = new[] { "custom", "markers", "shapes", "styling" },
                    Difficulty = 2,
                    ViewModelType = typeof(CustomMarkersViewModel),
                    ViewType = typeof(CustomMarkersView)
                }
            }
        });

        // Advanced Category
        categories.Add(new DemoCategory
        {
            Id = "advanced",
            Name = "Advanced Features",
            Description = "Complex charts and advanced functionality",
            Icon = "🔬",
            Demos = new List<DemoPage>
            {
                new DemoPage
                {
                    Id = "large-dataset",
                    Title = "Large Dataset",
                    Description = "Performance test with thousands of data points",
                    CategoryId = "advanced",
                    Tags = new[] { "performance", "large", "optimization" },
                    Difficulty = 3
                }
            }
        });

        // Interactive Category
        categories.Add(new DemoCategory
        {
            Id = "interactive",
            Name = "Interactive Charts",
            Description = "Charts with user interaction and responsiveness",
            Icon = "👆",
            Demos = new List<DemoPage>
            {
                new DemoPage
                {
                    Id = "point-selection",
                    Title = "Point Selection",
                    Description = "Interactive chart demonstrating point selection functionality",
                    CategoryId = "interactive",
                    Tags = new[] { "selection", "click", "interaction", "point" },
                    Difficulty = 2,
                    ViewModelType = typeof(PointSelectionViewModel),
                    ViewType = typeof(PointSelectionView)
                },
                new DemoPage
                {
                    Id = "legend-interaction",
                    Title = "Legend Interaction",
                    Description = "Toggle series visibility using interactive legend controls",
                    CategoryId = "interactive",
                    Tags = new[] { "legend", "toggle", "visibility", "series" },
                    Difficulty = 2,
                    ViewModelType = typeof(LegendInteractionViewModel),
                    ViewType = typeof(LegendInteractionView)
                },
                new DemoPage
                {
                    Id = "tooltips",
                    Title = "Tooltips",
                    Description = "Interactive tooltips on hover",
                    CategoryId = "interactive",
                    Tags = new[] { "tooltip", "hover", "interaction" },
                    Difficulty = 2,
                    ViewModelType = typeof(TooltipsViewModel),
                    ViewType = typeof(TooltipsView)
                },
                new DemoPage
                {
                    Id = "zoom-pan",
                    Title = "Zoom & Pan",
                    Description = "Navigate large datasets with zoom and pan",
                    CategoryId = "interactive",
                    Tags = new[] { "zoom", "pan", "navigation" },
                    Difficulty = 3,
                    ViewModelType = typeof(ZoomPanViewModel),
                    ViewType = typeof(ZoomPanView)
                }
            }
        });

        return categories;
    }
}
