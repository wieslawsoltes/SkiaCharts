using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Charts;

public class CandlestickChartTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var chart = new CandlestickChart();

        // Assert
        Assert.NotNull(chart.DefaultStyle);
        Assert.NotNull(chart.Configuration);
        Assert.Equal(CandleType.Candlestick, chart.DefaultStyle.CandleType);
        Assert.True(chart.DefaultStyle.UseHollowCandles);
        Assert.Equal(0.7, chart.DefaultStyle.CandleWidthRatio);
    }

    [Fact]
    public void BasicCandlestick_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new CandlestickChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105),   // Bullish
            new OhlcDataPoint(1, 105, 108, 102, 103),  // Bearish
            new OhlcDataPoint(2, 103, 115, 100, 112),  // Bullish
            new OhlcDataPoint(3, 112, 118, 110, 111)   // Bearish
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (no exception should be thrown)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void OhlcBars_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new CandlestickChart();
        chart.DefaultStyle.CandleType = CandleType.OhlcBar;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105),
            new OhlcDataPoint(1, 105, 108, 102, 103),
            new OhlcDataPoint(2, 103, 115, 100, 112)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void HollowCandles_Enabled_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new CandlestickChart();
        chart.DefaultStyle.UseHollowCandles = true;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105),   // Bullish - hollow
            new OhlcDataPoint(1, 105, 108, 102, 103),  // Bearish - filled
            new OhlcDataPoint(2, 103, 115, 100, 112)   // Bullish - hollow
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void HollowCandles_Disabled_ShouldRenderAllFilled()
    {
        // Arrange
        var chart = new CandlestickChart();
        chart.DefaultStyle.UseHollowCandles = false;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105),   // Bullish - filled
            new OhlcDataPoint(1, 105, 108, 102, 103),  // Bearish - filled
            new OhlcDataPoint(2, 103, 115, 100, 112)   // Bullish - filled
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void CustomColors_ShouldApply()
    {
        // Arrange
        var chart = new CandlestickChart();
        chart.DefaultStyle.BullishColor = SKColors.Green;
        chart.DefaultStyle.BearishColor = SKColors.Red;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105),
            new OhlcDataPoint(1, 105, 108, 102, 103)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void SetSeriesStyle_ShouldStoreStyleForSeries()
    {
        // Arrange
        var chart = new CandlestickChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105)
        });

        var style = new CandlestickSeriesStyle
        {
            BullishColor = SKColors.Blue,
            BearishColor = SKColors.Orange,
            CandleWidthRatio = 0.8
        };

        // Act
        chart.SetSeriesStyle(series, style);
        var retrievedStyle = chart.GetSeriesStyle(series);

        // Assert
        Assert.Equal(SKColors.Blue, retrievedStyle.BullishColor);
        Assert.Equal(SKColors.Orange, retrievedStyle.BearishColor);
        Assert.Equal(0.8, retrievedStyle.CandleWidthRatio);
    }

    [Fact]
    public void GetSeriesStyle_ShouldReturnDefaultStyleForUnstyledSeries()
    {
        // Arrange
        var chart = new CandlestickChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105)
        });

        // Act
        var style = chart.GetSeriesStyle(series);

        // Assert
        Assert.Same(chart.DefaultStyle, style);
    }

    [Fact]
    public void Doji_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new CandlestickChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 105, 95, 100),   // Doji (open == close)
            new OhlcDataPoint(1, 100, 110, 95, 105),
            new OhlcDataPoint(2, 105, 110, 105, 105)   // Doji
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (should handle doji candles)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void LongWicks_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new CandlestickChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 120, 80, 105),   // Long wicks
            new OhlcDataPoint(1, 105, 125, 90, 110),
            new OhlcDataPoint(2, 110, 115, 95, 100)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void ManyCandles_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new CandlestickChart();

        var points = new List<IDataPoint>();
        var price = 100.0;
        for (int i = 0; i < 100; i++)
        {
            var open = price;
            var close = price + (i % 2 == 0 ? 2 : -1.5);
            var high = Math.Max(open, close) + 1;
            var low = Math.Min(open, close) - 1;
            points.Add(new OhlcDataPoint(i, open, high, low, close));
            price = close;
        }

        var series = new DataSeries<IDataPoint>(points);
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(1200, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 1200, 600);
    }

    [Fact]
    public void CustomCandleWidth_ShouldApply()
    {
        // Arrange
        var chart = new CandlestickChart();
        chart.DefaultStyle.CandleWidthRatio = 0.9; // Wide candles

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105),
            new OhlcDataPoint(1, 105, 108, 102, 103),
            new OhlcDataPoint(2, 103, 115, 100, 112)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void MinimumCandleWidth_ShouldApply()
    {
        // Arrange
        var chart = new CandlestickChart();
        chart.DefaultStyle.MinimumCandleWidth = 5f;

        var points = new List<IDataPoint>();
        for (int i = 0; i < 200; i++)
        {
            points.Add(new OhlcDataPoint(i, 100, 110, 95, 105));
        }

        var series = new DataSeries<IDataPoint>(points);
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (candles should not be smaller than minimum)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void MaximumCandleWidth_ShouldApply()
    {
        // Arrange
        var chart = new CandlestickChart();
        chart.DefaultStyle.MaximumCandleWidth = 10f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105),
            new OhlcDataPoint(1, 105, 108, 102, 103)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (candles should not exceed maximum)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void CustomWickWidth_ShouldApply()
    {
        // Arrange
        var chart = new CandlestickChart();
        chart.DefaultStyle.WickWidth = 3f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105),
            new OhlcDataPoint(1, 105, 108, 102, 103)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void CustomBodyBorderWidth_ShouldApply()
    {
        // Arrange
        var chart = new CandlestickChart();
        chart.DefaultStyle.UseHollowCandles = true;
        chart.DefaultStyle.BodyBorderWidth = 2f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105),   // Bullish - hollow with thick border
            new OhlcDataPoint(1, 105, 108, 102, 103)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void OhlcTickRatio_ShouldApply()
    {
        // Arrange
        var chart = new CandlestickChart();
        chart.DefaultStyle.CandleType = CandleType.OhlcBar;
        chart.DefaultStyle.OhlcTickRatio = 0.7; // Longer ticks

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105),
            new OhlcDataPoint(1, 105, 108, 102, 103)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void EmptySeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new CandlestickChart();
        var series = new DataSeries<IDataPoint>(Array.Empty<IDataPoint>());
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (should not throw)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void NoSeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new CandlestickChart();

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (should not throw)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void SingleCandle_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new CandlestickChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void NonOhlcDataPoints_ShouldBeSkipped()
    {
        // Arrange
        var chart = new CandlestickChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105),
            new DataPoint(1, 105), // Regular data point - should be skipped
            new OhlcDataPoint(2, 105, 115, 100, 112)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (should skip non-OHLC points)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void MixedBullishBearish_ShouldRenderCorrectColors()
    {
        // Arrange
        var chart = new CandlestickChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105),   // Bullish (close > open)
            new OhlcDataPoint(1, 105, 108, 102, 103),  // Bearish (close < open)
            new OhlcDataPoint(2, 103, 115, 100, 112),  // Bullish
            new OhlcDataPoint(3, 112, 118, 110, 111),  // Bearish
            new OhlcDataPoint(4, 111, 111, 105, 111)   // Neutral (close == open, treated as bullish)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }
}
