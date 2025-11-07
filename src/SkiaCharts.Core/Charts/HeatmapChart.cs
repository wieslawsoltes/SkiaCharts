using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Heatmap chart for visualizing 2D data as a color-coded grid.
/// Supports color gradients, interpolation modes, and color legends.
/// </summary>
public class HeatmapChart : ChartBase
{
    private HeatmapData? _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="HeatmapChart"/> class.
    /// </summary>
    public HeatmapChart()
    {
        Style = new HeatmapSeriesStyle();
        Configuration = new HeatmapChartConfiguration();
    }

    /// <summary>
    /// Gets or sets the heatmap data.
    /// </summary>
    public HeatmapData? Data
    {
        get => _data;
        set => _data = value;
    }

    /// <summary>
    /// Gets or sets the heatmap style.
    /// </summary>
    public HeatmapSeriesStyle Style { get; set; }

    /// <summary>
    /// Gets or sets the chart configuration.
    /// </summary>
    public HeatmapChartConfiguration Configuration { get; set; }

    /// <inheritdoc/>
    protected override void BuildRenderQueue(RenderQueue queue, IRenderContext context)
    {
        base.BuildRenderQueue(queue, context);

        if (_data == null || _data.Rows == 0 || _data.Columns == 0)
        {
            return;
        }

        // Add heatmap renderer
        queue.Add(new HeatmapRenderer(_data, this, Style));

        // Add color legend if enabled
        if (Configuration.ShowColorLegend && Configuration.LegendPosition != LegendPosition.None)
        {
            queue.Add(new ColorLegendRenderer(_data, this, Style));
        }
    }

    private class HeatmapRenderer : ChartElement
    {
        private readonly HeatmapData _data;
        private readonly HeatmapChart _chart;
        private readonly HeatmapSeriesStyle _style;

        public HeatmapRenderer(HeatmapData data, HeatmapChart chart, HeatmapSeriesStyle style)
        {
            _data = data;
            _chart = chart;
            _style = style;
            Layer = RenderLayer.Data;
        }

        public override void Render(IRenderContext context)
        {
            var minValue = _chart.Configuration.MinValue ?? _data.MinValue;
            var maxValue = _chart.Configuration.MaxValue ?? _data.MaxValue;

            // Calculate cell dimensions
            var cellWidth = _chart.Viewport.ScreenRect.Width / _data.Columns;
            var cellHeight = _chart.Viewport.ScreenRect.Height / _data.Rows;

            // Render cells based on interpolation mode
            if (_style.Interpolation == HeatmapInterpolation.Nearest)
            {
                RenderNearestNeighbor(context, cellWidth, cellHeight, minValue, maxValue);
            }
            else
            {
                RenderInterpolated(context, cellWidth, cellHeight, minValue, maxValue);
            }

            // Render cell borders if enabled
            if (_style.ShowCellBorders)
            {
                RenderCellBorders(context, cellWidth, cellHeight);
            }

            // Render cell values if enabled
            if (_style.ShowCellValues && cellWidth >= _style.MinCellSizeForValues && cellHeight >= _style.MinCellSizeForValues)
            {
                RenderCellValues(context, cellWidth, cellHeight);
            }

            // Render contour lines if enabled
            if (_style.ShowContourLines)
            {
                RenderContourLines(context, cellWidth, cellHeight, minValue, maxValue);
            }
        }

        private void RenderNearestNeighbor(IRenderContext context, float cellWidth, float cellHeight, double minValue, double maxValue)
        {
            for (int row = 0; row < _data.Rows; row++)
            {
                for (int col = 0; col < _data.Columns; col++)
                {
                    var value = _data.GetValue(row, col);
                    if (double.IsNaN(value) || double.IsInfinity(value))
                    {
                        continue;
                    }

                    var color = ValueToColor(value, minValue, maxValue);
                    var x = _chart.Viewport.ScreenRect.Left + col * cellWidth;
                    var y = _chart.Viewport.ScreenRect.Top + row * cellHeight;

                    var cellRect = new SKRect(x, y, x + cellWidth, y + cellHeight);

                    using var paint = new SKPaint
                    {
                        Color = color,
                        Style = SKPaintStyle.Fill,
                        IsAntialias = false // Faster for rectangular cells
                    };

                    context.DrawRect(cellRect, paint);
                }
            }
        }

        private void RenderInterpolated(IRenderContext context, float cellWidth, float cellHeight, double minValue, double maxValue)
        {
            // Create a bitmap for smoother rendering with interpolation
            var bitmapWidth = (int)Math.Max(1, _chart.Viewport.ScreenRect.Width);
            var bitmapHeight = (int)Math.Max(1, _chart.Viewport.ScreenRect.Height);

            using var bitmap = new SKBitmap(bitmapWidth, bitmapHeight);

            for (int py = 0; py < bitmapHeight; py++)
            {
                for (int px = 0; px < bitmapWidth; px++)
                {
                    // Map pixel to grid coordinates
                    var gridX = (px / (float)bitmapWidth) * _data.Columns;
                    var gridY = (py / (float)bitmapHeight) * _data.Rows;

                    // Interpolate value
                    var value = BilinearInterpolate(gridX, gridY);
                    var color = ValueToColor(value, minValue, maxValue);

                    bitmap.SetPixel(px, py, color);
                }
            }

            using var paint = new SKPaint
            {
                IsAntialias = true
            };

            context.Canvas.DrawBitmap(bitmap, _chart.Viewport.ScreenRect, paint);
        }

        private double BilinearInterpolate(float x, float y)
        {
            // Get the four surrounding grid points
            var x0 = (int)Math.Floor(x);
            var x1 = Math.Min(x0 + 1, _data.Columns - 1);
            var y0 = (int)Math.Floor(y);
            var y1 = Math.Min(y0 + 1, _data.Rows - 1);

            // Clamp to grid bounds
            x0 = Math.Max(0, Math.Min(x0, _data.Columns - 1));
            y0 = Math.Max(0, Math.Min(y0, _data.Rows - 1));

            // Get values at corners
            var v00 = _data.GetValue(y0, x0);
            var v10 = _data.GetValue(y0, x1);
            var v01 = _data.GetValue(y1, x0);
            var v11 = _data.GetValue(y1, x1);

            // Handle NaN values
            if (double.IsNaN(v00)) v00 = 0;
            if (double.IsNaN(v10)) v10 = v00;
            if (double.IsNaN(v01)) v01 = v00;
            if (double.IsNaN(v11)) v11 = v00;

            // Calculate interpolation weights
            var wx = x - x0;
            var wy = y - y0;

            // Bilinear interpolation
            var v0 = v00 * (1 - wx) + v10 * wx;
            var v1 = v01 * (1 - wx) + v11 * wx;
            return v0 * (1 - wy) + v1 * wy;
        }

        private void RenderCellBorders(IRenderContext context, float cellWidth, float cellHeight)
        {
            using var paint = new SKPaint
            {
                Color = _style.CellBorderColor,
                StrokeWidth = _style.CellBorderWidth,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            // Draw vertical lines
            for (int col = 0; col <= _data.Columns; col++)
            {
                var x = _chart.Viewport.ScreenRect.Left + col * cellWidth;
                context.DrawLine(x, _chart.Viewport.ScreenRect.Top,
                               x, _chart.Viewport.ScreenRect.Bottom, paint);
            }

            // Draw horizontal lines
            for (int row = 0; row <= _data.Rows; row++)
            {
                var y = _chart.Viewport.ScreenRect.Top + row * cellHeight;
                context.DrawLine(_chart.Viewport.ScreenRect.Left, y,
                               _chart.Viewport.ScreenRect.Right, y, paint);
            }
        }

        private void RenderCellValues(IRenderContext context, float cellWidth, float cellHeight)
        {
            using var paint = new SKPaint
            {
                Color = _style.CellValueColor,
                TextSize = _style.CellValueFontSize,
                TextAlign = SKTextAlign.Center,
                IsAntialias = true
            };

            for (int row = 0; row < _data.Rows; row++)
            {
                for (int col = 0; col < _data.Columns; col++)
                {
                    var value = _data.GetValue(row, col);
                    if (double.IsNaN(value) || double.IsInfinity(value))
                    {
                        continue;
                    }

                    var text = value.ToString(_style.CellValueFormat);
                    var x = _chart.Viewport.ScreenRect.Left + col * cellWidth + cellWidth / 2;
                    var y = _chart.Viewport.ScreenRect.Top + row * cellHeight + cellHeight / 2 + _style.CellValueFontSize / 3;

                    context.DrawText(text, x, y, paint);
                }
            }
        }

        private void RenderContourLines(IRenderContext context, float cellWidth, float cellHeight, double minValue, double maxValue)
        {
            // Simple contour line implementation
            using var paint = new SKPaint
            {
                Color = _style.ContourLineColor,
                StrokeWidth = _style.ContourLineWidth,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            var range = maxValue - minValue;
            if (range <= 0) return;

            // Draw contour lines at specific levels
            for (int level = 1; level < _style.ContourLevels; level++)
            {
                var threshold = minValue + (range * level / _style.ContourLevels);

                // Simplified contour detection - check cell boundaries
                for (int row = 0; row < _data.Rows - 1; row++)
                {
                    for (int col = 0; col < _data.Columns - 1; col++)
                    {
                        var v00 = _data.GetValue(row, col);
                        var v10 = _data.GetValue(row, col + 1);
                        var v01 = _data.GetValue(row + 1, col);

                        // Check if contour crosses horizontally
                        if ((v00 < threshold && v10 >= threshold) || (v00 >= threshold && v10 < threshold))
                        {
                            var x = _chart.Viewport.ScreenRect.Left + (col + 0.5f) * cellWidth;
                            var y = _chart.Viewport.ScreenRect.Top + row * cellHeight;
                            context.DrawLine(x, y, x, y + cellHeight, paint);
                        }

                        // Check if contour crosses vertically
                        if ((v00 < threshold && v01 >= threshold) || (v00 >= threshold && v01 < threshold))
                        {
                            var x = _chart.Viewport.ScreenRect.Left + col * cellWidth;
                            var y = _chart.Viewport.ScreenRect.Top + (row + 0.5f) * cellHeight;
                            context.DrawLine(x, y, x + cellWidth, y, paint);
                        }
                    }
                }
            }
        }

        private SKColor ValueToColor(double value, double minValue, double maxValue)
        {
            // Normalize value to 0-1 range
            var range = maxValue - minValue;
            if (range <= 0) return SKColors.Gray;

            var normalizedValue = (value - minValue) / range;
            normalizedValue = Math.Max(0, Math.Min(1, normalizedValue));

            // Use custom color scale or default
            var colorScale = _style.ColorScale ?? new[]
            {
                new SKColor(0, 0, 255),      // Blue (low)
                new SKColor(0, 255, 255),    // Cyan
                new SKColor(0, 255, 0),      // Green
                new SKColor(255, 255, 0),    // Yellow
                new SKColor(255, 0, 0)       // Red (high)
            };

            return InterpolateColor(normalizedValue, colorScale);
        }

        private SKColor InterpolateColor(double t, SKColor[] colors)
        {
            if (colors.Length == 0) return SKColors.Black;
            if (colors.Length == 1) return colors[0];

            var scaledT = t * (colors.Length - 1);
            var index = (int)scaledT;
            var localT = scaledT - index;

            if (index >= colors.Length - 1)
            {
                return colors[colors.Length - 1];
            }

            var color1 = colors[index];
            var color2 = colors[index + 1];

            var r = (byte)(color1.Red + (color2.Red - color1.Red) * localT);
            var g = (byte)(color1.Green + (color2.Green - color1.Green) * localT);
            var b = (byte)(color1.Blue + (color2.Blue - color1.Blue) * localT);

            return new SKColor(r, g, b);
        }
    }

    private class ColorLegendRenderer : ChartElement
    {
        private readonly HeatmapData _data;
        private readonly HeatmapChart _chart;
        private readonly HeatmapSeriesStyle _style;

        public ColorLegendRenderer(HeatmapData data, HeatmapChart chart, HeatmapSeriesStyle style)
        {
            _data = data;
            _chart = chart;
            _style = style;
            Layer = RenderLayer.Overlay;
        }

        public override void Render(IRenderContext context)
        {
            if (_chart.Configuration.LegendPosition == LegendPosition.None)
            {
                return;
            }

            var minValue = _chart.Configuration.MinValue ?? _data.MinValue;
            var maxValue = _chart.Configuration.MaxValue ?? _data.MaxValue;

            var legendRect = CalculateLegendRect();

            // Draw color gradient
            var steps = 100;
            var stepHeight = legendRect.Height / steps;

            for (int i = 0; i < steps; i++)
            {
                var t = i / (double)steps;
                var value = minValue + t * (maxValue - minValue);
                var color = ValueToColor(value, minValue, maxValue);

                var y = legendRect.Bottom - (i + 1) * stepHeight;
                var rect = new SKRect(legendRect.Left, y, legendRect.Right, y + stepHeight);

                using var paint = new SKPaint
                {
                    Color = color,
                    Style = SKPaintStyle.Fill
                };

                context.DrawRect(rect, paint);
            }

            // Draw border
            using var borderPaint = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1f
            };
            context.DrawRect(legendRect, borderPaint);

            // Draw labels
            using var textPaint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 10f,
                IsAntialias = true
            };

            var maxText = maxValue.ToString("F1");
            var minText = minValue.ToString("F1");

            context.DrawText(maxText, legendRect.Right + 5, legendRect.Top + 10, textPaint);
            context.DrawText(minText, legendRect.Right + 5, legendRect.Bottom, textPaint);
        }

        private SKRect CalculateLegendRect()
        {
            var config = _chart.Configuration;
            var legendWidth = config.LegendWidth;
            var legendHeight = _chart.Viewport.ScreenRect.Height * config.LegendHeightRatio;

            var left = _chart.Viewport.ScreenRect.Right + config.LegendSpacing;
            var top = _chart.Viewport.ScreenRect.Top +
                     (_chart.Viewport.ScreenRect.Height - legendHeight) / 2;

            return new SKRect(left, top, left + legendWidth, top + legendHeight);
        }

        private SKColor ValueToColor(double value, double minValue, double maxValue)
        {
            var range = maxValue - minValue;
            if (range <= 0) return SKColors.Gray;

            var normalizedValue = (value - minValue) / range;
            normalizedValue = Math.Max(0, Math.Min(1, normalizedValue));

            var colorScale = _style.ColorScale ?? new[]
            {
                new SKColor(0, 0, 255),
                new SKColor(0, 255, 255),
                new SKColor(0, 255, 0),
                new SKColor(255, 255, 0),
                new SKColor(255, 0, 0)
            };

            return InterpolateColor(normalizedValue, colorScale);
        }

        private SKColor InterpolateColor(double t, SKColor[] colors)
        {
            if (colors.Length == 0) return SKColors.Black;
            if (colors.Length == 1) return colors[0];

            var scaledT = t * (colors.Length - 1);
            var index = (int)scaledT;
            var localT = scaledT - index;

            if (index >= colors.Length - 1)
            {
                return colors[colors.Length - 1];
            }

            var color1 = colors[index];
            var color2 = colors[index + 1];

            var r = (byte)(color1.Red + (color2.Red - color1.Red) * localT);
            var g = (byte)(color1.Green + (color2.Green - color1.Green) * localT);
            var b = (byte)(color1.Blue + (color2.Blue - color1.Blue) * localT);

            return new SKColor(r, g, b);
        }
    }
}
