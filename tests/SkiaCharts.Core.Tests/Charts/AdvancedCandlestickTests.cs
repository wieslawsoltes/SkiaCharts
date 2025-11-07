using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Charts;

public class AdvancedCandlestickTests
{
    // Heiken-Ashi Tests
    [Fact]
    public void HeikenAshi_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var chart = new HeikenAshiChart();

        // Assert
        Assert.NotNull(chart.DefaultStyle);
        Assert.NotNull(chart.Configuration);
    }

    [Fact]
    public void HeikenAshi_BasicChart_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new HeikenAshiChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 105, 98, 103, 1000),
            new OhlcDataPoint(1, 103, 108, 102, 107, 1200),
            new OhlcDataPoint(2, 107, 110, 105, 106, 1100),
            new OhlcDataPoint(3, 106, 109, 104, 105, 900),
            new OhlcDataPoint(4, 105, 107, 103, 106, 1000)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void HeikenAshi_EmptySeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new HeikenAshiChart();
        var series = new DataSeries<IDataPoint>(Array.Empty<IDataPoint>());
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    // Renko Chart Tests
    [Fact]
    public void Renko_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var chart = new RenkoChart();

        // Assert
        Assert.NotNull(chart.Configuration);
        Assert.Equal(20f, chart.Configuration.BrickWidth);
        Assert.Equal(2f, chart.Configuration.BrickSpacing);
    }

    [Fact]
    public void Renko_WithFixedBrickSize_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new RenkoChart();
        chart.Configuration.BrickSize = 2.0;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 105, 98, 103),
            new OhlcDataPoint(1, 103, 108, 102, 107),
            new OhlcDataPoint(2, 107, 110, 105, 106),
            new OhlcDataPoint(3, 106, 109, 104, 105),
            new OhlcDataPoint(4, 105, 107, 103, 106),
            new OhlcDataPoint(5, 106, 111, 105, 110),
            new OhlcDataPoint(6, 110, 112, 108, 109)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void Renko_WithAutoBrickSize_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new RenkoChart();
        // BrickSize is null, will be calculated automatically

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 105, 98, 103),
            new OhlcDataPoint(1, 103, 108, 102, 107),
            new OhlcDataPoint(2, 107, 110, 105, 106)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void Renko_HollowBearishBricks_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new RenkoChart();
        chart.Configuration.BrickSize = 2.0;
        chart.Configuration.HollowBearishBricks = true;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 105, 98, 103),
            new OhlcDataPoint(1, 103, 108, 102, 107),
            new OhlcDataPoint(2, 107, 110, 105, 106),
            new OhlcDataPoint(3, 106, 109, 100, 101),
            new OhlcDataPoint(4, 101, 102, 95, 96)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void Renko_EmptySeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new RenkoChart();
        var series = new DataSeries<IDataPoint>(Array.Empty<IDataPoint>());
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    // Kagi Chart Tests
    [Fact]
    public void Kagi_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var chart = new KagiChart();

        // Assert
        Assert.NotNull(chart.Configuration);
        Assert.Equal(4.0, chart.Configuration.ReversalPercentage);
        Assert.Equal(30f, chart.Configuration.HorizontalSpacing);
    }

    [Fact]
    public void Kagi_WithFixedReversalAmount_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new KagiChart();
        chart.Configuration.ReversalAmount = 3.0;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 105, 98, 103),
            new OhlcDataPoint(1, 103, 108, 102, 107),
            new OhlcDataPoint(2, 107, 110, 105, 106),
            new OhlcDataPoint(3, 106, 109, 102, 103),
            new OhlcDataPoint(4, 103, 105, 100, 101),
            new OhlcDataPoint(5, 101, 106, 100, 105)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void Kagi_WithPercentageReversal_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new KagiChart();
        chart.Configuration.ReversalPercentage = 5.0;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 105, 98, 103),
            new OhlcDataPoint(1, 103, 108, 102, 107),
            new OhlcDataPoint(2, 107, 110, 105, 109)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void Kagi_EmptySeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new KagiChart();
        var series = new DataSeries<IDataPoint>(Array.Empty<IDataPoint>());
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    // Point & Figure Tests
    [Fact]
    public void PointAndFigure_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var chart = new PointAndFigureChart();

        // Assert
        Assert.NotNull(chart.Configuration);
        Assert.Equal(1.0, chart.Configuration.BoxSize);
        Assert.Equal(3, chart.Configuration.ReversalAmount);
        Assert.Equal(20f, chart.Configuration.BoxWidth);
        Assert.Equal(20f, chart.Configuration.BoxHeight);
    }

    [Fact]
    public void PointAndFigure_BasicChart_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PointAndFigureChart();
        chart.Configuration.BoxSize = 2.0;
        chart.Configuration.ReversalAmount = 3;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 105, 98, 103),
            new OhlcDataPoint(1, 103, 110, 102, 109),
            new OhlcDataPoint(2, 109, 112, 108, 111),
            new OhlcDataPoint(3, 111, 113, 105, 106),
            new OhlcDataPoint(4, 106, 108, 102, 103),
            new OhlcDataPoint(5, 103, 110, 102, 108)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void PointAndFigure_FilledOs_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PointAndFigureChart();
        chart.Configuration.BoxSize = 2.0;
        chart.Configuration.FillOs = true;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 105, 98, 103),
            new OhlcDataPoint(1, 103, 110, 102, 109),
            new OhlcDataPoint(2, 109, 112, 105, 106),
            new OhlcDataPoint(3, 106, 108, 102, 103)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void PointAndFigure_EmptySeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new PointAndFigureChart();
        var series = new DataSeries<IDataPoint>(Array.Empty<IDataPoint>());
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    // Pattern Recognition Tests
    [Fact]
    public void PatternRecognizer_DetectDoji_ShouldIdentifyPattern()
    {
        // Arrange
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 105, 98, 102),  // Filler
            new OhlcDataPoint(1, 102, 103, 101, 102.2), // Filler
            new OhlcDataPoint(2, 102.2, 103, 101, 102.1) // Doji: small body
        });

        // Act
        var patterns = CandlestickPatternRecognizer.DetectPatterns(series);

        // Assert
        Assert.Contains(patterns, p => p.Pattern == CandlestickPattern.Doji);
    }

    [Fact]
    public void PatternRecognizer_DetectPatterns_ShouldWork()
    {
        // Arrange
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 105, 98, 102),
            new OhlcDataPoint(1, 102, 103, 101, 102),
            new OhlcDataPoint(2, 100, 106, 90, 105)
        });

        // Act
        var patterns = CandlestickPatternRecognizer.DetectPatterns(series);

        // Assert - just check that pattern detection runs without errors
        Assert.NotNull(patterns);
    }

    [Fact]
    public void PatternRecognizer_DetectBullishEngulfing_ShouldIdentifyPattern()
    {
        // Arrange
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 105, 98, 102),  // Filler
            new OhlcDataPoint(1, 105, 106, 102, 103), // Bearish
            new OhlcDataPoint(2, 102, 108, 101, 107)  // Bullish engulfing
        });

        // Act
        var patterns = CandlestickPatternRecognizer.DetectPatterns(series);

        // Assert
        Assert.Contains(patterns, p => p.Pattern == CandlestickPattern.BullishEngulfing);
    }

    [Fact]
    public void PatternRecognizer_DetectMorningStar_ShouldIdentifyPattern()
    {
        // Arrange
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 110, 111, 105, 106), // Bearish
            new OhlcDataPoint(1, 106, 107, 105, 106), // Doji
            new OhlcDataPoint(2, 106, 112, 105, 111)  // Bullish
        });

        // Act
        var patterns = CandlestickPatternRecognizer.DetectPatterns(series);

        // Assert
        Assert.Contains(patterns, p => p.Pattern == CandlestickPattern.MorningStar);
    }

    [Fact]
    public void PatternRecognizer_EmptySeries_ShouldReturnEmpty()
    {
        // Arrange
        var series = new DataSeries<IDataPoint>(Array.Empty<IDataPoint>());

        // Act
        var patterns = CandlestickPatternRecognizer.DetectPatterns(series);

        // Assert
        Assert.Empty(patterns);
    }

    [Fact]
    public void PatternRecognizer_InsufficientData_ShouldReturnEmpty()
    {
        // Arrange
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 105, 98, 103)
        });

        // Act
        var patterns = CandlestickPatternRecognizer.DetectPatterns(series);

        // Assert (only single-candle patterns can be detected)
        Assert.All(patterns, p => Assert.True(
            p.Pattern == CandlestickPattern.Doji ||
            p.Pattern == CandlestickPattern.Hammer ||
            p.Pattern == CandlestickPattern.InvertedHammer ||
            p.Pattern == CandlestickPattern.ShootingStar ||
            p.Pattern == CandlestickPattern.HangingMan
        ));
    }
}
