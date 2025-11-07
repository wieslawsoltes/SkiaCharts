# SkiaCharts

High-performance charting framework built on SkiaSharp with Avalonia integration.

## Project Vision

Enterprise-grade, modular charting framework with Excel/trading chart parity, real-time capabilities, and deep Avalonia integration.

## Current Status

**Milestone 1: Foundation & Core Architecture** ✅ COMPLETED
**Milestone 1.5: Animation Framework** ✅ COMPLETED
**Milestone 2: Essential Chart Types** ✅ COMPLETED

📊 **204/204 tests passing** (100%)

### Implemented Features

#### Data Abstractions
- ✅ `IDataPoint` - Base interface for all data points
- ✅ `DataPoint` - Basic 2D data point structure
- ✅ `OhlcDataPoint` - OHLC data point for financial charts
- ✅ `ScatterDataPoint` - Extended data point with size and color values
- ✅ `PieDataPoint` - Data point for pie/donut charts with optional labels
- ✅ `IDataSeries<T>` - Generic data series interface
- ✅ `DataSeries<T>` - Immutable data series with automatic bounds calculation
- ✅ `ObservableDataSeries<T>` - Observable series for real-time data
- ✅ `CircularBuffer<T>` - Fixed-size rolling buffer for streaming
- ✅ `DataRange` - Range utilities for min/max calculations
- ✅ `DataSeriesCollection` - Manage multiple series efficiently
- ✅ `DataTransform` - Pipeline for data preprocessing (NEW!)
  - ✅ Scale, Offset, Normalize, Log transformations
  - ✅ Moving Average smoothing
  - ✅ Clamp and composable pipeline

#### Rendering Pipeline
- ✅ `IRenderContext` - Abstraction over SkiaSharp canvas
- ✅ `RenderContext` - Default implementation
- ✅ `RenderQueue` - Layer-based rendering system
- ✅ `RenderLayer` - Defined rendering layers (Background, Grid, Data, Annotations, Overlay)
- ✅ `IRenderable` - Interface for renderable elements
- ✅ `ViewportManager` - Coordinate transformation (data ↔ screen space)
  - ✅ Zoom support
  - ✅ Pan support
  - ✅ FitToRange support
- ✅ `ClipRegion` - Efficient viewport culling support

#### Axis System
- ✅ `IAxis` - Base axis interface
- ✅ `LinearAxis` - Linear numeric axis with auto-scaling
  - ✅ "Nice number" tick generation algorithm
  - ✅ Automatic label formatting based on magnitude
  - ✅ Custom format string support
- ✅ `DateTimeAxis` - Time-based axis
  - ✅ Intelligent interval selection (seconds to decades)
  - ✅ Auto-formatting based on time span
  - ✅ Works with OADate internally
- ✅ `CategoryAxis` - Categorical/discrete axis
  - ✅ Dynamic category management
  - ✅ Label skip logic for overcrowding
  - ✅ Optimal range for bar centering
- ✅ `LogarithmicAxis` - Logarithmic scale axis (NEW!)
  - ✅ Base-n logarithmic scaling (default base 10)
  - ✅ Major/minor tick generation
  - ✅ Smart formatting (10^n notation)
- ✅ `AxisPosition` - Enum for axis positioning (Left, Right, Top, Bottom)
- ✅ `TickInfo` - Tick information structure

#### Core Chart Infrastructure
- ✅ `ChartElement` - Base class for renderable elements
- ✅ `ChartArea` - Plotting area with margins and padding
- ✅ `ChartBase` - Abstract base for all chart types
- ✅ `LineChart` - Basic line chart implementation
  - ✅ Multi-series support
  - ✅ Optional markers
  - ✅ Configurable line style
- ✅ `LineChartEnhanced` - Advanced line chart
  - ✅ 3 line modes: Linear, Stepped, Smooth (Catmull-Rom splines)
  - ✅ 7 marker shapes: Circle, Square, Diamond, Triangle, TriangleDown, Cross, Plus
  - ✅ Area fills with customizable color/alpha
  - ✅ Dash patterns for dashed/dotted lines
  - ✅ Per-series styling with independent configurations
  - ✅ Marker fill and stroke customization
  - ✅ Smooth curve tension control (0-1)
- ✅ `BarChart` - Bar/Column chart with full feature set
  - ✅ 2 orientations: Vertical (column) and Horizontal (bar)
  - ✅ 3 stack modes: None (grouped), Absolute, Percentage
  - ✅ Rounded corners with configurable radius
  - ✅ Gradient fills (linear, configurable angle)
  - ✅ Border/outline support with width and color
  - ✅ Per-series styling
  - ✅ Value labels with formatting
  - ✅ Configurable bar width and spacing
  - ✅ Minimum bar size for small values
- ✅ `AreaChart` - Area chart with transparency and gradients
  - ✅ 3 area modes: Linear, Stepped, Smooth (Catmull-Rom splines)
  - ✅ 2 stack modes: None (overlapping), Stacked
  - ✅ Gradient fills (vertical, horizontal, radial)
  - ✅ Transparency/alpha blending support
  - ✅ Optional boundary line with dash patterns
  - ✅ Customizable baseline
  - ✅ Per-series styling
  - ✅ Negative value support
- ✅ `ScatterChart` - Scatter chart with advanced visualization
  - ✅ 7 marker shapes: Circle, Square, Diamond, Triangle, TriangleDown, Cross, Plus
  - ✅ Variable marker sizing based on data values
  - ✅ Color mapping with interpolated color scales
  - ✅ Customizable color scales (default: blue→cyan→green→yellow→red)
  - ✅ Marker borders/outlines with width and color
  - ✅ Optional connecting lines between points
  - ✅ Per-series styling
  - ✅ Support for mixed data point types
  - ✅ Edge case handling (same sizes/colors, empty data)
- ✅ `PieChart` - Pie and donut charts with rich features (NEW!)
  - ✅ Unified chart for both pie and donut modes
  - ✅ Configurable inner radius ratio for donut thickness (0-1)
  - ✅ Exploded slices with configurable distance
  - ✅ Multiple exploded slices support
  - ✅ Rotation/start angle configuration (0-360 degrees)
  - ✅ Radial gradient fills for slices
  - ✅ Slice borders/outlines with width and color
  - ✅ 3 label positions: None, Inside, Outside with leader lines
  - ✅ 6 label content types: Percentage, Value, Both, Name, NameAndPercentage, NameAndValue
  - ✅ Automatic label skipping for small slices
  - ✅ Per-slice styling (color, gradient, explode distance, border)
  - ✅ Default color palette (8 colors)
  - ✅ Automatic percentage calculation
  - ✅ Handles negative/zero values (skips them)
  - ✅ Configurable radius ratio for chart sizing

#### Utilities
- ✅ `MathHelper` - Mathematical utilities for charting
  - ✅ Clamp
  - ✅ Lerp (linear interpolation)
  - ✅ Nice number calculation
  - ✅ Significant figures rounding

#### Animation Framework
- ✅ `Animation<T>` - Generic animation with any value type
- ✅ `AnimationController` - Manage multiple animations with FPS tracking
- ✅ `IEasingFunction` - 28 easing functions + custom Bezier
  - Linear, Quadratic, Cubic, Sine, Exponential, Elastic, Bounce, Back, Circular
  - Each with In, Out, InOut variants (except Linear)
  - Custom cubic Bezier curves via `CreateBezier(x1, y1, x2, y2)`
- ✅ `Interpolators` - Color, Point, Rect, Size interpolation
- ✅ Frame-rate independent animations (delta time)
- ✅ Animation callbacks (Start, Update, Complete)
- ✅ Loop and Auto-reverse support
- ✅ `AnimationGroup` - Parallel animation execution
- ✅ `AnimationSequence` - Sequential animation chaining
- ✅ `SpringAnimation` - Physics-based spring animations
- ✅ `AnimatableProperty<T>` - MVVM-ready property wrapper
- ✅ Fluent API and 7 animation presets
- ✅ Chart-specific animations (NEW!)
  - ✅ `FadeInAnimation` - Opacity fade-in
  - ✅ `GrowAnimation` - Scale growth
  - ✅ `SlideInAnimation` - Slide from direction
  - ✅ `WipeAnimation` - Progressive reveal

#### Testing
- ✅ 204 unit tests passing (100%)
- ✅ Data series tests (9 tests)
- ✅ Viewport transformation tests (4 tests)
- ✅ ClipRegion tests (13 tests)
- ✅ Animation tests (16 tests)
  - Core animation tests
  - Bezier easing tests
- ✅ Axis system tests (23 tests)
  - LinearAxis tests (7)
  - DateTimeAxis tests (6)
  - CategoryAxis tests (10)
- ✅ LineChartEnhanced tests (26 tests)
  - Line mode tests (Linear, Stepped, Smooth)
  - Marker shape tests (all 7 shapes)
  - Area fill tests
  - Dash pattern tests
  - Multi-series tests
  - Edge case tests
- ✅ BarChart tests (24 tests)
  - Orientation tests (Vertical, Horizontal)
  - Stack mode tests (None, Absolute, Percentage)
  - Rounded corners tests
  - Gradient fill tests
  - Border/outline tests
  - Value label tests
  - Multi-series tests (grouped and stacked)
  - Edge case tests
- ✅ AreaChart tests (26 tests)
  - Area mode tests (Linear, Stepped, Smooth)
  - Stack mode tests (None, Stacked)
  - Gradient tests (Vertical, Horizontal, Radial)
  - Transparency/alpha tests
  - Boundary line tests
  - Baseline customization tests
  - Multi-series tests
  - Edge case tests
- ✅ ScatterChart tests (22 tests)
  - Marker shape tests (all 7 shapes)
  - Variable marker sizing tests
  - Color mapping tests (default and custom scales)
  - Combined features tests (size + color)
  - Connecting lines tests
  - Border/outline tests with all shapes
  - Multi-series tests with different styles
  - Mixed data point type tests
  - Edge case tests (empty data, single point, same values, large datasets)
- ✅ PieChart tests (31 tests - NEW!)
  - Basic pie and donut rendering tests
  - Exploded slice tests (single and multiple)
  - Start angle/rotation tests (0°, 90°, 180°, 270°)
  - Gradient fill tests (single and multiple slices)
  - Slice border tests
  - Label position tests (None, Inside, Outside)
  - Label content tests (all 6 content types)
  - Donut with labels tests
  - Small slice label skipping tests
  - Value handling tests (negative, zero, all zero)
  - Donut thickness tests (thin and thick rings)
  - Exploded donut tests
  - Combined features tests (exploded + gradient + labels)
  - Custom radius ratio tests
  - Mixed data point type tests (PieDataPoint and DataPoint)
  - Edge case tests (empty series, single slice, many slices)
- ✅ Integration tests (10 tests)
  - Complete workflow tests
  - Component interaction tests

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

All tests passing: ✅ 204/204

## Next Steps

See [docs/PLAN.md](docs/PLAN.md) for the complete roadmap.

**Milestone 2: Essential Chart Types** ✅ COMPLETE
- ✅ Line Charts (M2.1)
  - Linear, Stepped, and Smooth curves
  - 7 marker shapes with customization
  - Area fills and dash patterns
  - Per-series styling
- ✅ Bar/Column Charts (M2.2)
  - Vertical and horizontal orientations
  - Grouped, stacked (absolute), and stacked (percentage) modes
  - Rounded corners, gradients, and borders
  - Value labels and styling
- ✅ Area Charts (M2.3)
  - Linear, Stepped, and Smooth modes
  - Overlapping and stacked modes
  - Three gradient directions (vertical, horizontal, radial)
  - Transparency and baseline support
- ✅ Scatter Charts (M2.4)
  - 7 marker shapes with variable sizing
  - Color mapping with interpolated scales
  - Connecting lines and border support
  - Per-series styling
- ✅ Pie/Donut Charts (M2.5)
  - Unified pie/donut implementation
  - Exploded slices and rotation
  - Labels with leader lines
  - Radial gradients and borders

**Next Major Milestone: M3 - Advanced Chart Types**
See [docs/PLAN.md](docs/PLAN.md) for detailed planning.

## Requirements

- .NET 9.0+
- SkiaSharp 3.116.1+

## License

MIT
