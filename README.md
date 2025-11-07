# SkiaCharts

[![CI](https://github.com/wieslawsoltes/SkiaCharts/actions/workflows/ci.yml/badge.svg)](https://github.com/wieslawsoltes/SkiaCharts/actions/workflows/ci.yml)

High-performance charting framework built on SkiaSharp with a first-class Avalonia UI experience.

## Packages

| Package | Description | NuGet |
| --- | --- | --- |
| `SkiaCharts.Core` | Platform-agnostic rendering engine, chart primitives, and data abstractions. | [![SkiaCharts.Core](https://img.shields.io/nuget/vpre/SkiaCharts.Core?logo=nuget&label=Core)](https://www.nuget.org/packages/SkiaCharts.Core/) |
| `SkiaCharts.Avalonia` | Avalonia control set, theming, and bindings for hosting charts in desktop apps. | [![SkiaCharts.Avalonia](https://img.shields.io/nuget/vpre/SkiaCharts.Avalonia?logo=nuget&label=Avalonia)](https://www.nuget.org/packages/SkiaCharts.Avalonia/) |
| `SkiaCharts.Trading` | Financial/trading extensions, indicators, and specialized chart types. | [![SkiaCharts.Trading](https://img.shields.io/nuget/vpre/SkiaCharts.Trading?logo=nuget&label=Trading)](https://www.nuget.org/packages/SkiaCharts.Trading/) |

## Quick Start

### Install via NuGet

```bash
dotnet add package SkiaCharts.Core
dotnet add package SkiaCharts.Avalonia
# Optional trading extensions
dotnet add package SkiaCharts.Trading
```

### Basic rendering (SkiaSharp)

```csharp
using SkiaCharts.Core.Axes;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

var series = new DataSeries<DataPoint>(
    Enumerable.Range(0, 500)
        .Select(i => new DataPoint(i, MathF.Sin(i * 0.05f))),
    name: "Sample");

var chart = new LineChart
{
    Title = "Sine wave",
    LineWidth = 3f,
    LineColor = SKColors.DeepSkyBlue,
    XAxis = new LinearAxis { Position = AxisPosition.Bottom },
    YAxis = new LinearAxis { Position = AxisPosition.Left }
};

chart.Series.Add(series);

using var bitmap = new SKBitmap(1200, 480);
using var canvas = new SKCanvas(bitmap);
chart.Render(canvas, bitmap.Width, bitmap.Height);

// Persist the bitmap using standard SkiaSharp helpers.
```

### Avalonia host

```xml
<UserControl
    xmlns="https://github.com/avaloniaui"
    xmlns:charts="clr-namespace:SkiaCharts.Avalonia.Controls;assembly=SkiaCharts.Avalonia">
  <charts:SkiaChartView Chart="{Binding Chart}"
                        EnableZoom="True"
                        EnablePan="True"
                        ShowLegend="True"
                        Margin="16" />
</UserControl>
```

```csharp
public class ChartViewModel
{
    public ChartBase Chart { get; }

    public ChartViewModel()
    {
        var points = Enumerable.Range(0, 100)
            .Select(i => new DataPoint(i, i))
            .ToList();

        var series = new DataSeries<DataPoint>(points, "Line");
        var chart = new LineChart { Title = "Avalonia sample" };
        chart.Series.Add(series);

        Chart = chart;
    }
}
```

### Build & test locally

```bash
dotnet restore
dotnet build
dotnet test
```

## Continuous Integration & Releases

The GitHub Actions workflow (`.github/workflows/ci.yml`) restores, builds, tests, and packs the solution on every push and pull request. Tagging a commit with the `v*` pattern automatically:

1. Publishes the generated NuGet packages as workflow artifacts.
2. Creates a GitHub Release with generated notes and attached `.nupkg` files.
3. Pushes packages to NuGet using the repository `NUGET_API_KEY` secret.

## Feature Roadmap

| Area | Supported Today | Planned |
| --- | --- | --- |
| Data & Series | `IDataPoint`, `DataSeries`, `ObservableDataSeries`, `CircularBuffer`, `DataRange`, and the transform pipeline (scale/offset/normalize/log/moving average). | Streaming data adapters, rolling aggregations, reusable indicator catalog. |
| Chart Types | Line, area, bar/column, scatter, pie/donut, and configurable trading primitives with per-series styling and gradients. | Candlestick/Heikin-Ashi, heatmaps, dashboards, and composable multi-chart layouts. |
| Axes & Scaling | Linear, logarithmic, date/time, and category axes with auto-fit, tick formatting, and grid styling. | Dual-axis sync, polar/radar axes, and customizable axis renderers. |
| Rendering & Layout | Layered render queue, clip regions, viewport manager with fit/zoom/pan, chart area layout, theming support. | Adaptive layout presets, accessibility overlays, export helpers. |
| Interaction & Animation | Zoom/pan gestures, animation framework with easing presets, hooks for tooltips/markers via Avalonia controls. | Crosshair/selection gestures, annotation authoring, richer animation presets. |
| Avalonia Integration | `SkiaChartView`, theme presets, MVVM bindings, and the gallery sample. | Responsive dashboard components, designer tooling, Fluent-style templates. |

## Detailed Feature Matrix

| Area | Capability | Status | Notes |
| --- | --- | --- | --- |
| Data | `IDataPoint`, `DataPoint` | ✅ | Immutable XY primitives with documented semantics for every chart. |
| Data | `OhlcDataPoint` | ✅ | High/low/open/close payload used by trading visuals and indicators. |
| Data | `ScatterDataPoint` | ✅ | Adds color/size channels to drive bubble charts and heat-style plots. |
| Data | `PieDataPoint` | ✅ | Provides slice labels, explode distance, and per-slice color overrides. |
| Data | `DataSeries<T>` | ✅ | Immutable series with lazy min/max caching and indexer access. |
| Data | `DataSeriesCollection` | ✅ | Aggregates multiple series and exports combined X/Y ranges. |
| Data | `ObservableDataSeries<T>` | ✅ | Notifies listeners of insert/remove operations for live dashboards. |
| Data | `CircularBuffer<T>` | ✅ | Fixed-size rolling buffer optimized for streaming financial feeds. |
| Data | `DataTransform` pipeline | ✅ | Chainable scale/offset/normalize/log/moving-average transforms. |
| Rendering | `IRenderContext` / `RenderContext` | ✅ | Abstraction over SkiaSharp canvases, handles clears and draw ops. |
| Rendering | `RenderQueue` & `RenderLayer` | ✅ | Layered renderer ordering background, grid, data, annotations, overlay. |
| Rendering | `ViewportManager` | ✅ | Bidirectional data↔screen transforms with zoom/pan/fit support. |
| Rendering | `ClipRegion` | ✅ | Efficient culling that skips drawing work outside the viewport. |
| Rendering | `ChartArea` layout | ✅ | Computes margins and padding to derive the plot rectangle. |
| Axes | `LinearAxis` | ✅ | Auto-scaling, nice-number ticks, and customizable format strings. |
| Axes | `DateTimeAxis` | ✅ | Picks optimal intervals (seconds→decades) and formats timestamps. |
| Axes | `CategoryAxis` | ✅ | Category management with label skipping and centered bars. |
| Axes | `LogarithmicAxis` | ✅ | Base-n scaling with major/minor tick generation and 10^n labels. |
| Axes | Axis primitives | ✅ | `AxisPosition`, `TickInfo`, and shared contracts for custom axes. |
| Charts | `LineChart` | ✅ | Lightweight renderer with markers, per-series color and width. |
| Charts | `LineChartEnhanced` | ✅ | Linear/stepped/smooth curves, dash patterns, area fills, 7 marker shapes. |
| Charts | `BarChart` | ✅ | Vertical/horizontal layouts, stacking modes, gradients, borders, labels. |
| Charts | `AreaChart` | ✅ | Overlapping or stacked areas with gradients, transparency, baselines. |
| Charts | `ScatterChart` | ✅ | 7 marker shapes, variable sizing, color mapping, optional connectors. |
| Charts | `PieChart` | ✅ | Unified pie/donut engine with exploded slices, label positions, gradients. |
| Animation | Core animation engine | ✅ | 28 easing functions, interpolators, sequences/groups, FPS awareness. |
| Animation | Chart animation presets | ✅ | Fade, grow, slide, and wipe helpers tuned for chart scenarios. |
| Interaction | Viewport gestures | ✅ | Zoom, pan, and fit-to-range via `ViewportManager` APIs. |
| Interaction | Hit testing API | 🟡 | `ChartBase.HitTest` stub ready; interactive annotations queued next. |
| Avalonia | `SkiaChartView` control | ✅ | GPU-backed control with caching, bindings, and property system integration. |
| Avalonia | Theme system (`ChartTheme`) | ✅ | Light/dark presets and binding hooks for app-wide theming. |
| Avalonia | Gallery sample | ✅ | Desktop showcase demonstrating MVVM bindings and multiple chart types. |
| Tooling | `SkiaCharts.Core.Tests` suite | ✅ | 200+ unit tests covering data structures, charts, and rendering logic. |
| Tooling | Documentation set | ✅ | README plus `docs/PLAN.md` communicate roadmap and architecture. |

## Project Structure

```
SkiaCharts/
├── src/
│   ├── SkiaCharts.Core/              # Platform-agnostic core library
│   │   ├── Data/                     # Data abstractions
│   │   ├── Rendering/                # Rendering pipeline
│   │   ├── Axes/                     # Axis system
│   │   ├── Charts/                   # Chart implementations
│   │   ├── Utilities/                # Helper utilities
│   │   └── (Transforms, Styles, Interaction - TBD)
│   ├── SkiaCharts.Avalonia/          # Avalonia integration (TBD)
│   └── SkiaCharts.Trading/           # Trading charts & indicators (TBD)
├── samples/
│   └── SkiaCharts.Gallery/           # Demo application
├── tests/
│   └── SkiaCharts.Core.Tests/        # Unit tests
└── docs/
    └── PLAN.md                        # Detailed implementation plan
```

## Building

```bash
dotnet build
```

## Running Tests

```bash
dotnet test
```

## Requirements

- .NET 9.0+
- SkiaSharp 3.116.1+

## License

MIT
