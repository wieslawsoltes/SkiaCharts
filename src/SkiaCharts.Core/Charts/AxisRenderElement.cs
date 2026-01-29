using SkiaCharts.Core.Axes;
using SkiaCharts.Core.Rendering;
using SkiaCharts.Core.Theming;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Renders chart axes, grid lines, and tick labels based on the current viewport.
/// </summary>
internal sealed class AxisRenderElement : ChartElement
{
    private readonly ChartArea _chartArea;
    private readonly ViewportManager _viewport;
    private readonly IAxis _axis;
    private readonly ChartTheme _theme;
    private readonly SKRect _chartBounds;

    public AxisRenderElement(ChartArea chartArea, ViewportManager viewport, IAxis axis, ChartTheme theme, SKRect chartBounds)
    {
        _chartArea = chartArea;
        _viewport = viewport;
        _axis = axis;
        _theme = theme;
        _chartBounds = chartBounds;
        Layer = RenderLayer.Grid;
    }

    public override void Render(IRenderContext context)
    {
        if (!_axis.IsVisible)
        {
            return;
        }

        var totalBounds = _chartBounds;
        var plotArea = _chartArea.CalculatePlotArea(totalBounds);

        if (plotArea.Width <= 0 || plotArea.Height <= 0)
        {
            return;
        }

        var ticks = _axis.GenerateTicks();
        if (ticks.Count == 0 && !_axis.ShowGridLines && !_axis.ShowLabels)
        {
            return;
        }

        var axisStyle = _theme.Axis;
        var gridStyle = _theme.Grid;

        using var axisPaint = new SKPaint
        {
            Color = axisStyle.LineColor,
            StrokeWidth = axisStyle.LineWidth,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        using var tickPaint = new SKPaint
        {
            Color = axisStyle.TickColor,
            StrokeWidth = axisStyle.TickWidth,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        using var labelPaint = new SKPaint
        {
            Color = axisStyle.LabelColor,
            TextSize = axisStyle.LabelFontSize,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName(_theme.Fonts.LabelFontFamily)
        };

        using var majorGridPaint = new SKPaint
        {
            Color = gridStyle.MajorGridColor,
            StrokeWidth = gridStyle.MajorGridWidth,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        using var minorGridPaint = new SKPaint
        {
            Color = gridStyle.MinorGridColor,
            StrokeWidth = gridStyle.MinorGridWidth,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        SKPathEffect? majorDash = null;
        if (gridStyle.MajorGridDashPattern != null && gridStyle.MajorGridDashPattern.Length > 0)
        {
            majorDash = SKPathEffect.CreateDash(gridStyle.MajorGridDashPattern, 0);
            majorGridPaint.PathEffect = majorDash;
        }

        SKPathEffect? minorDash = null;
        if (gridStyle.MinorGridDashPattern != null && gridStyle.MinorGridDashPattern.Length > 0)
        {
            minorDash = SKPathEffect.CreateDash(gridStyle.MinorGridDashPattern, 0);
            minorGridPaint.PathEffect = minorDash;
        }

        try
        {
            switch (_axis.Position)
            {
                case AxisPosition.Bottom:
                case AxisPosition.Top:
                    RenderHorizontalAxis(context, plotArea, ticks, axisPaint, tickPaint, labelPaint, majorGridPaint, minorGridPaint);
                    break;
                case AxisPosition.Left:
                case AxisPosition.Right:
                    RenderVerticalAxis(context, plotArea, ticks, axisPaint, tickPaint, labelPaint, majorGridPaint, minorGridPaint);
                    break;
            }
        }
        finally
        {
            majorDash?.Dispose();
            minorDash?.Dispose();
        }
    }

    private void RenderHorizontalAxis(
        IRenderContext context,
        SKRect plotArea,
        IReadOnlyList<TickInfo> ticks,
        SKPaint axisPaint,
        SKPaint tickPaint,
        SKPaint labelPaint,
        SKPaint majorGridPaint,
        SKPaint minorGridPaint)
    {
        var axisArea = _axis.Position == AxisPosition.Bottom
            ? _chartArea.CalculateBottomAxisArea(_chartBounds)
            : _chartArea.CalculateTopAxisArea(_chartBounds);

        var axisY = _axis.Position == AxisPosition.Bottom ? plotArea.Bottom : plotArea.Top;
        context.DrawLine(plotArea.Left, axisY, plotArea.Right, axisY, axisPaint);

        var labelPadding = 4f;
        var majorTickLength = _theme.Axis.TickLength;
        var minorTickLength = majorTickLength * 0.6f;

        foreach (var tick in ticks)
        {
            var x = _viewport.DataToScreenX(tick.Value);
            if (x < plotArea.Left - 1 || x > plotArea.Right + 1)
            {
                continue;
            }

            var isMajor = tick.IsMajor;
            var tickLength = isMajor ? majorTickLength : minorTickLength;
            tickPaint.StrokeWidth = isMajor ? _theme.Axis.TickWidth : _theme.Axis.TickWidth * 0.7f;

            var tickStartY = axisY;
            var tickEndY = _axis.Position == AxisPosition.Bottom ? axisY + tickLength : axisY - tickLength;
            context.DrawLine(x, tickStartY, x, tickEndY, tickPaint);

            if (_axis.ShowGridLines && isMajor && _theme.Grid.ShowMajorGrid)
            {
                context.DrawLine(x, plotArea.Top, x, plotArea.Bottom, majorGridPaint);
            }
            else if (_axis.ShowGridLines && !isMajor && _theme.Grid.ShowMinorGrid)
            {
                context.DrawLine(x, plotArea.Top, x, plotArea.Bottom, minorGridPaint);
            }

            if (_axis.ShowLabels && isMajor && !string.IsNullOrWhiteSpace(tick.Label))
            {
                var textBounds = context.MeasureText(tick.Label, labelPaint);
                var textX = x - textBounds.Width / 2f;
                var baselineY = _axis.Position == AxisPosition.Bottom
                    ? axisY + tickLength + labelPadding - textBounds.Top
                    : axisY - tickLength - labelPadding - textBounds.Bottom;

                if (_axis.Position == AxisPosition.Bottom && baselineY <= axisArea.Bottom + labelPadding)
                {
                    context.DrawText(tick.Label, textX, baselineY, labelPaint);
                }
                else if (_axis.Position == AxisPosition.Top && baselineY >= axisArea.Top - labelPadding)
                {
                    context.DrawText(tick.Label, textX, baselineY, labelPaint);
                }
            }
        }
    }

    private void RenderVerticalAxis(
        IRenderContext context,
        SKRect plotArea,
        IReadOnlyList<TickInfo> ticks,
        SKPaint axisPaint,
        SKPaint tickPaint,
        SKPaint labelPaint,
        SKPaint majorGridPaint,
        SKPaint minorGridPaint)
    {
        var axisArea = _axis.Position == AxisPosition.Left
            ? _chartArea.CalculateLeftAxisArea(_chartBounds)
            : _chartArea.CalculateRightAxisArea(_chartBounds);

        var axisX = _axis.Position == AxisPosition.Left ? plotArea.Left : plotArea.Right;
        context.DrawLine(axisX, plotArea.Top, axisX, plotArea.Bottom, axisPaint);

        var labelPadding = 6f;
        var majorTickLength = _theme.Axis.TickLength;
        var minorTickLength = majorTickLength * 0.6f;

        foreach (var tick in ticks)
        {
            var y = _viewport.DataToScreenY(tick.Value);
            if (y < plotArea.Top - 1 || y > plotArea.Bottom + 1)
            {
                continue;
            }

            var isMajor = tick.IsMajor;
            var tickLength = isMajor ? majorTickLength : minorTickLength;
            tickPaint.StrokeWidth = isMajor ? _theme.Axis.TickWidth : _theme.Axis.TickWidth * 0.7f;

            var tickStartX = axisX;
            var tickEndX = _axis.Position == AxisPosition.Left ? axisX - tickLength : axisX + tickLength;
            context.DrawLine(tickStartX, y, tickEndX, y, tickPaint);

            if (_axis.ShowGridLines && isMajor && _theme.Grid.ShowMajorGrid)
            {
                context.DrawLine(plotArea.Left, y, plotArea.Right, y, majorGridPaint);
            }
            else if (_axis.ShowGridLines && !isMajor && _theme.Grid.ShowMinorGrid)
            {
                context.DrawLine(plotArea.Left, y, plotArea.Right, y, minorGridPaint);
            }

            if (_axis.ShowLabels && isMajor && !string.IsNullOrWhiteSpace(tick.Label))
            {
                var textBounds = context.MeasureText(tick.Label, labelPaint);
                var baselineY = y - (textBounds.Top + textBounds.Bottom) / 2f;
                var textX = _axis.Position == AxisPosition.Left
                    ? axisArea.Right - labelPadding - textBounds.Width
                    : axisArea.Left + labelPadding;

                if (_axis.Position == AxisPosition.Left && textX >= axisArea.Left - labelPadding)
                {
                    context.DrawText(tick.Label, textX, baselineY, labelPaint);
                }
                else if (_axis.Position == AxisPosition.Right && textX + textBounds.Width <= axisArea.Right + labelPadding)
                {
                    context.DrawText(tick.Label, textX, baselineY, labelPaint);
                }
            }
        }
    }
}
