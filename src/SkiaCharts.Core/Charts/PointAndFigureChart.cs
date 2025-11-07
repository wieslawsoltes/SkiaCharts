using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Represents a Point & Figure column (either X's or O's).
/// </summary>
public readonly struct PnFColumn
{
    public PnFColumn(int columnIndex, double bottomBox, int boxCount, bool isX)
    {
        ColumnIndex = columnIndex;
        BottomBox = bottomBox;
        BoxCount = boxCount;
        IsX = isX;
    }

    public int ColumnIndex { get; }
    public double BottomBox { get; }
    public int BoxCount { get; }
    public bool IsX { get; } // true for X's (rising), false for O's (falling)
}

/// <summary>
/// Configuration for Point & Figure chart.
/// </summary>
public class PointAndFigureChartConfiguration
{
    /// <summary>
    /// Gets or sets the box size (price movement per box).
    /// </summary>
    public double BoxSize { get; set; } = 1.0;

    /// <summary>
    /// Gets or sets the reversal amount (number of boxes required to reverse direction).
    /// </summary>
    public int ReversalAmount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the box width in pixels.
    /// </summary>
    public float BoxWidth { get; set; } = 20f;

    /// <summary>
    /// Gets or sets the box height in pixels.
    /// </summary>
    public float BoxHeight { get; set; } = 20f;

    /// <summary>
    /// Gets or sets the column spacing in pixels.
    /// </summary>
    public float ColumnSpacing { get; set; } = 5f;

    /// <summary>
    /// Gets or sets the color for X's (rising columns).
    /// </summary>
    public SKColor XColor { get; set; } = SKColors.Green;

    /// <summary>
    /// Gets or sets the color for O's (falling columns).
    /// </summary>
    public SKColor OColor { get; set; } = SKColors.Red;

    /// <summary>
    /// Gets or sets the line width for X's and O's.
    /// </summary>
    public float LineWidth { get; set; } = 2f;

    /// <summary>
    /// Gets or sets whether to fill O's.
    /// </summary>
    public bool FillOs { get; set; } = false;
}

/// <summary>
/// Point & Figure chart - Uses X's and O's to show price movements without time consideration.
/// X's represent rising prices, O's represent falling prices.
/// </summary>
public class PointAndFigureChart : ChartBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PointAndFigureChart"/> class.
    /// </summary>
    public PointAndFigureChart()
    {
        Configuration = new PointAndFigureChartConfiguration();
    }

    /// <summary>
    /// Gets or sets the chart configuration.
    /// </summary>
    public PointAndFigureChartConfiguration Configuration { get; set; }

    /// <inheritdoc/>
    protected override void BuildRenderQueue(RenderQueue queue, IRenderContext context)
    {
        base.BuildRenderQueue(queue, context);

        // Add Point & Figure renderers
        foreach (var series in Series)
        {
            if (series.Count > 0)
            {
                queue.Add(new PointAndFigureRenderer(series, this));
            }
        }
    }

    private class PointAndFigureRenderer : ChartElement
    {
        private readonly IDataSeries<IDataPoint> _series;
        private readonly PointAndFigureChart _chart;

        public PointAndFigureRenderer(IDataSeries<IDataPoint> series, PointAndFigureChart chart)
        {
            _series = series;
            _chart = chart;
            Layer = RenderLayer.Data;
        }

        public override void Render(IRenderContext context)
        {
            if (_series.Count == 0)
            {
                return;
            }

            // Convert to P&F columns
            var columns = ConvertToPointAndFigure(_series);

            // Render each column
            foreach (var column in columns)
            {
                RenderColumn(context, column);
            }
        }

        private List<PnFColumn> ConvertToPointAndFigure(IDataSeries<IDataPoint> series)
        {
            var columns = new List<PnFColumn>();

            if (series.Count == 0)
            {
                return columns;
            }

            var config = _chart.Configuration;
            var boxSize = config.BoxSize;
            var reversalAmount = config.ReversalAmount;

            // Get initial price
            var firstPoint = series[0];
            var ohlc = firstPoint as OhlcDataPoint? ?? new OhlcDataPoint(firstPoint.X, firstPoint.Y, firstPoint.Y, firstPoint.Y, firstPoint.Y);

            var currentPrice = ohlc.Close;
            var currentBoxLevel = (int)(currentPrice / boxSize);
            bool isXColumn = true; // Start with X's
            int columnIndex = 0;
            int boxCount = 1;
            var columnBottomBox = currentBoxLevel;

            for (int i = 1; i < series.Count; i++)
            {
                var point = series[i];
                var ohlcPoint = point as OhlcDataPoint? ?? new OhlcDataPoint(point.X, point.Y, point.Y, point.Y, point.Y);
                var close = ohlcPoint.Close;
                var newBoxLevel = (int)(close / boxSize);

                if (isXColumn)
                {
                    // In X column, look for rising prices
                    if (newBoxLevel > currentBoxLevel)
                    {
                        // Extend current X column
                        boxCount += (newBoxLevel - currentBoxLevel);
                        currentBoxLevel = newBoxLevel;
                    }
                    // Check for reversal (falling prices)
                    else if (currentBoxLevel - newBoxLevel >= reversalAmount)
                    {
                        // Save current X column
                        columns.Add(new PnFColumn(columnIndex++, columnBottomBox * boxSize, boxCount, true));

                        // Start new O column
                        isXColumn = false;
                        currentBoxLevel = newBoxLevel;
                        boxCount = (int)((columnBottomBox + boxCount - 1) - currentBoxLevel);
                        columnBottomBox = currentBoxLevel;
                    }
                }
                else
                {
                    // In O column, look for falling prices
                    if (newBoxLevel < currentBoxLevel)
                    {
                        // Extend current O column
                        boxCount += (currentBoxLevel - newBoxLevel);
                        currentBoxLevel = newBoxLevel;
                        columnBottomBox = newBoxLevel;
                    }
                    // Check for reversal (rising prices)
                    else if (newBoxLevel - currentBoxLevel >= reversalAmount)
                    {
                        // Save current O column
                        columns.Add(new PnFColumn(columnIndex++, columnBottomBox * boxSize, boxCount, false));

                        // Start new X column
                        isXColumn = true;
                        currentBoxLevel = newBoxLevel;
                        boxCount = newBoxLevel - (columnBottomBox - 1);
                        columnBottomBox = (columnBottomBox + 1);
                    }
                }
            }

            // Add final column
            if (boxCount > 0)
            {
                columns.Add(new PnFColumn(columnIndex, columnBottomBox * boxSize, boxCount, isXColumn));
            }

            return columns;
        }

        private void RenderColumn(IRenderContext context, PnFColumn column)
        {
            var config = _chart.Configuration;
            var xPos = column.ColumnIndex * (config.BoxWidth + config.ColumnSpacing);

            // Draw each box in the column
            for (int i = 0; i < column.BoxCount; i++)
            {
                var boxPrice = column.BottomBox + (i * config.BoxSize);
                var yPos = _chart.Viewport.DataToScreen(0, boxPrice).Y;

                if (column.IsX)
                {
                    DrawX(context, xPos, yPos, config);
                }
                else
                {
                    DrawO(context, xPos, yPos, config);
                }
            }
        }

        private void DrawX(IRenderContext context, float x, float y, PointAndFigureChartConfiguration config)
        {
            using var paint = new SKPaint
            {
                Color = config.XColor,
                StrokeWidth = config.LineWidth,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            var halfWidth = config.BoxWidth / 2;
            var halfHeight = config.BoxHeight / 2;

            // Draw X (two diagonal lines)
            context.DrawLine(x, y - halfHeight, x + config.BoxWidth, y + halfHeight, paint);
            context.DrawLine(x, y + halfHeight, x + config.BoxWidth, y - halfHeight, paint);
        }

        private void DrawO(IRenderContext context, float x, float y, PointAndFigureChartConfiguration config)
        {
            using var paint = new SKPaint
            {
                Color = config.OColor,
                StrokeWidth = config.LineWidth,
                Style = config.FillOs ? SKPaintStyle.Fill : SKPaintStyle.Stroke,
                IsAntialias = true
            };

            var centerX = x + config.BoxWidth / 2;
            var radius = Math.Min(config.BoxWidth, config.BoxHeight) / 2 - config.LineWidth;

            context.DrawCircle(centerX, y, radius, paint);
        }
    }
}
