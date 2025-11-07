using SkiaCharts.Core.Data;
using SkiaCharts.Core.Indicators;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Indicators;

public class OverlayIndicatorTests
{
    // SMA Tests
    [Fact]
    public void SMA_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var sma = new SmaIndicator();

        // Assert
        Assert.Equal(20, sma.Period);
        Assert.Equal("SMA(20)", sma.Name);
    }

    [Fact]
    public void SMA_Calculate_ShouldReturnCorrectValues()
    {
        // Arrange
        var sma = new SmaIndicator(3);
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 30),
            new DataPoint(3, 40),
            new DataPoint(4, 50)
        });

        // Act
        var result = sma.Calculate(series);

        // Assert
        Assert.Equal(5, result.Count);
        Assert.True(double.IsNaN(result[0].Y)); // Not enough data
        Assert.True(double.IsNaN(result[1].Y)); // Not enough data
        Assert.Equal(20, result[2].Y);          // (10+20+30)/3 = 20
        Assert.Equal(30, result[3].Y);          // (20+30+40)/3 = 30
        Assert.Equal(40, result[4].Y);          // (30+40+50)/3 = 40
    }

    [Fact]
    public void SMA_InsufficientData_ShouldReturnEmpty()
    {
        // Arrange
        var sma = new SmaIndicator(10);
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20)
        });

        // Act
        var result = sma.Calculate(series);

        // Assert
        Assert.Empty(result);
    }

    // EMA Tests
    [Fact]
    public void EMA_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var ema = new EmaIndicator();

        // Assert
        Assert.Equal(20, ema.Period);
        Assert.Equal("EMA(20)", ema.Name);
    }

    [Fact]
    public void EMA_Calculate_ShouldReturnCorrectValues()
    {
        // Arrange
        var ema = new EmaIndicator(3);
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 30),
            new DataPoint(3, 40),
            new DataPoint(4, 50)
        });

        // Act
        var result = ema.Calculate(series);

        // Assert
        Assert.Equal(5, result.Count);
        Assert.True(double.IsNaN(result[0].Y));
        Assert.True(double.IsNaN(result[1].Y));
        Assert.Equal(20, result[2].Y); // Initial SMA: (10+20+30)/3 = 20

        // EMA uses multiplier = 2/(3+1) = 0.5
        // EMA[3] = (40 - 20) * 0.5 + 20 = 30
        Assert.Equal(30, result[3].Y);

        // EMA[4] = (50 - 30) * 0.5 + 30 = 40
        Assert.Equal(40, result[4].Y);
    }

    // WMA Tests
    [Fact]
    public void WMA_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var wma = new WmaIndicator();

        // Assert
        Assert.Equal(20, wma.Period);
        Assert.Equal("WMA(20)", wma.Name);
    }

    [Fact]
    public void WMA_Calculate_ShouldReturnCorrectValues()
    {
        // Arrange
        var wma = new WmaIndicator(3);
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 30),
            new DataPoint(3, 40)
        });

        // Act
        var result = wma.Calculate(series);

        // Assert
        Assert.Equal(4, result.Count);
        Assert.True(double.IsNaN(result[0].Y));
        Assert.True(double.IsNaN(result[1].Y));

        // WMA[2] = (30*3 + 20*2 + 10*1) / (3+2+1) = (90+40+10) / 6 = 23.333...
        Assert.Equal(23.333, result[2].Y, 3);

        // WMA[3] = (40*3 + 30*2 + 20*1) / 6 = (120+60+20) / 6 = 33.333...
        Assert.Equal(33.333, result[3].Y, 3);
    }

    // Bollinger Bands Tests
    [Fact]
    public void BollingerBands_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var bb = new BollingerBandsIndicator();

        // Assert
        Assert.Equal(20, bb.Period);
        Assert.Equal(2.0, bb.StandardDeviations);
        Assert.Equal("BB(20,2)", bb.Name);
    }

    [Fact]
    public void BollingerBands_Calculate_ShouldReturnCorrectValues()
    {
        // Arrange
        var bb = new BollingerBandsIndicator(3, 1.0);
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 30),
            new DataPoint(3, 20),
            new DataPoint(4, 10)
        });

        // Act
        var result = bb.Calculate(series);

        // Assert
        Assert.Equal(5, result.Count);
        Assert.True(double.IsNaN(result[0].Y));
        Assert.True(double.IsNaN(result[1].Y));

        // At index 2: SMA = 20, values are [10, 20, 30]
        // StdDev = sqrt(((10-20)^2 + (20-20)^2 + (30-20)^2) / 3) = sqrt(200/3) ≈ 8.165
        var bbPoint = result[2] as BollingerBandsDataPoint;
        Assert.NotNull(bbPoint);
        Assert.Equal(20, bbPoint.Middle);
        Assert.Equal(28.165, bbPoint.Upper, 3);
        Assert.Equal(11.835, bbPoint.Lower, 3);
    }

    // Parabolic SAR Tests
    [Fact]
    public void ParabolicSAR_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var sar = new ParabolicSarIndicator();

        // Assert
        Assert.Equal(0.02, sar.AccelerationFactor);
        Assert.Equal(0.2, sar.MaxAcceleration);
        Assert.Equal("SAR(0.02,0.2)", sar.Name);
    }

    [Fact]
    public void ParabolicSAR_Calculate_ShouldReturnValues()
    {
        // Arrange
        var sar = new ParabolicSarIndicator();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 105, 95, 103),
            new OhlcDataPoint(1, 103, 110, 102, 108),
            new OhlcDataPoint(2, 108, 112, 106, 111),
            new OhlcDataPoint(3, 111, 115, 109, 113),
            new OhlcDataPoint(4, 113, 116, 111, 114)
        });

        // Act
        var result = sar.Calculate(series);

        // Assert
        Assert.Equal(5, result.Count);
        Assert.All(result, p => Assert.False(double.IsNaN(p.Y)));
    }

    // Ichimoku Cloud Tests
    [Fact]
    public void IchimokuCloud_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var ichimoku = new IchimokuCloudIndicator();

        // Assert
        Assert.Equal(9, ichimoku.TenkanPeriod);
        Assert.Equal(26, ichimoku.KijunPeriod);
        Assert.Equal(52, ichimoku.SenkouBPeriod);
        Assert.Equal(26, ichimoku.Displacement);
        Assert.Equal("Ichimoku(9,26,52)", ichimoku.Name);
    }

    [Fact]
    public void IchimokuCloud_Calculate_ShouldReturnValues()
    {
        // Arrange
        var ichimoku = new IchimokuCloudIndicator(3, 5, 10, 5);
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 90, 105),
            new OhlcDataPoint(1, 105, 115, 95, 110),
            new OhlcDataPoint(2, 110, 120, 100, 115),
            new OhlcDataPoint(3, 115, 125, 105, 120),
            new OhlcDataPoint(4, 120, 130, 110, 125),
            new OhlcDataPoint(5, 125, 135, 115, 130),
            new OhlcDataPoint(6, 130, 140, 120, 135),
            new OhlcDataPoint(7, 135, 145, 125, 140),
            new OhlcDataPoint(8, 140, 150, 130, 145),
            new OhlcDataPoint(9, 145, 155, 135, 150),
            new OhlcDataPoint(10, 150, 160, 140, 155)
        });

        // Act
        var result = ichimoku.Calculate(series);

        // Assert
        Assert.Equal(11, result.Count);

        var ichimokuPoint = result[10] as IchimokuDataPoint;
        Assert.NotNull(ichimokuPoint);
        Assert.False(double.IsNaN(ichimokuPoint.Tenkan));
        Assert.False(double.IsNaN(ichimokuPoint.Kijun));
    }

    // Pivot Points Tests
    [Fact]
    public void PivotPoints_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var pivot = new PivotPointsIndicator();

        // Assert
        Assert.Equal(PivotPointMethod.Standard, pivot.Method);
        Assert.Equal("Pivot(Standard)", pivot.Name);
    }

    [Fact]
    public void PivotPoints_StandardMethod_ShouldCalculateCorrectly()
    {
        // Arrange
        var pivot = new PivotPointsIndicator(PivotPointMethod.Standard);
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 90, 105)
        });

        // Act
        var result = pivot.Calculate(series);

        // Assert
        Assert.Single(result);
        var pivotPoint = result[0] as PivotPointsDataPoint;
        Assert.NotNull(pivotPoint);

        // Pivot = (110 + 90 + 105) / 3 = 101.667
        Assert.Equal(101.667, pivotPoint.Pivot, 3);

        // R1 = 2 * 101.667 - 90 = 113.333
        Assert.Equal(113.333, pivotPoint.R1, 3);

        // S1 = 2 * 101.667 - 110 = 93.333
        Assert.Equal(93.333, pivotPoint.S1, 3);
    }

    [Fact]
    public void PivotPoints_FibonacciMethod_ShouldCalculateCorrectly()
    {
        // Arrange
        var pivot = new PivotPointsIndicator(PivotPointMethod.Fibonacci);
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 90, 105)
        });

        // Act
        var result = pivot.Calculate(series);

        // Assert
        Assert.Single(result);
        var pivotPoint = result[0] as PivotPointsDataPoint;
        Assert.NotNull(pivotPoint);

        // Pivot = (110 + 90 + 105) / 3 = 101.667
        Assert.Equal(101.667, pivotPoint.Pivot, 3);

        // R1 = 101.667 + 0.382 * (110 - 90) = 109.307
        Assert.Equal(109.307, pivotPoint.R1, 3);
    }

    // VWAP Tests
    [Fact]
    public void VWAP_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var vwap = new VwapIndicator();

        // Assert
        Assert.Equal("VWAP", vwap.Name);
        Assert.False(vwap.UseRollingWindow);
    }

    [Fact]
    public void VWAP_Calculate_ShouldReturnCorrectValues()
    {
        // Arrange
        var vwap = new VwapIndicator();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 90, 100, 1000),
            new OhlcDataPoint(1, 100, 120, 100, 110, 2000),
            new OhlcDataPoint(2, 110, 130, 110, 120, 1500)
        });

        // Act
        var result = vwap.Calculate(series);

        // Assert
        Assert.Equal(3, result.Count);

        // First point: TP = (110+90+100)/3 = 100
        // VWAP = (100 * 1000) / 1000 = 100
        Assert.Equal(100, result[0].Y);

        // Second point: TP = (120+100+110)/3 = 110
        // VWAP = (100*1000 + 110*2000) / (1000+2000) = 320000 / 3000 = 106.667
        Assert.Equal(106.667, result[1].Y, 3);
    }

    [Fact]
    public void VWAP_RollingWindow_ShouldReturnCorrectValues()
    {
        // Arrange
        var vwap = new VwapIndicator
        {
            UseRollingWindow = true,
            RollingPeriod = 2
        };
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 90, 100, 1000),
            new OhlcDataPoint(1, 100, 120, 100, 110, 2000),
            new OhlcDataPoint(2, 110, 130, 110, 120, 1500)
        });

        // Act
        var result = vwap.Calculate(series);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.True(double.IsNaN(result[0].Y)); // Not enough data

        // At index 1: only last 2 points
        // VWAP = (100*1000 + 110*2000) / 3000 = 106.667
        Assert.Equal(106.667, result[1].Y, 3);
    }

    [Fact]
    public void VWAP_ZeroVolume_ShouldReturnNaN()
    {
        // Arrange
        var vwap = new VwapIndicator();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 90, 100, 0)
        });

        // Act
        var result = vwap.Calculate(series);

        // Assert
        Assert.Single(result);
        Assert.True(double.IsNaN(result[0].Y));
    }

    // Integration Tests
    [Fact]
    public void AllIndicators_EmptySeries_ShouldReturnEmpty()
    {
        // Arrange
        var emptySeries = new DataSeries<IDataPoint>(Array.Empty<IDataPoint>());
        var indicators = new IIndicator[]
        {
            new SmaIndicator(),
            new EmaIndicator(),
            new WmaIndicator(),
            new BollingerBandsIndicator(),
            new ParabolicSarIndicator(),
            new IchimokuCloudIndicator(),
            new PivotPointsIndicator(),
            new VwapIndicator()
        };

        // Act & Assert
        foreach (var indicator in indicators)
        {
            var result = indicator.Calculate(emptySeries);
            Assert.Empty(result);
        }
    }

    [Fact]
    public void AllIndicators_ShouldHaveVisibilityControl()
    {
        // Arrange
        var indicators = new IndicatorBase[]
        {
            new SmaIndicator(),
            new EmaIndicator(),
            new WmaIndicator(),
            new BollingerBandsIndicator(),
            new ParabolicSarIndicator(),
            new IchimokuCloudIndicator(),
            new PivotPointsIndicator(),
            new VwapIndicator()
        };

        // Act & Assert
        foreach (var indicator in indicators)
        {
            Assert.True(indicator.IsVisible); // Default is visible

            indicator.IsVisible = false;
            Assert.False(indicator.IsVisible);

            // Should be able to set colors
            indicator.Color = SKColors.Red;
            Assert.Equal(SKColors.Red, indicator.Color);

            // Should be able to set line width
            indicator.LineWidth = 2.5f;
            Assert.Equal(2.5f, indicator.LineWidth);
        }
    }
}
