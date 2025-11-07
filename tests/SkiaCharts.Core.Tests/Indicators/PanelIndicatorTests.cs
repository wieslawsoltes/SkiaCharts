using SkiaCharts.Core.Data;
using SkiaCharts.Core.Indicators;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Indicators;

public class PanelIndicatorTests
{
    // RSI Tests
    [Fact]
    public void RSI_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var rsi = new RsiIndicator();

        // Assert
        Assert.Equal(14, rsi.Period);
        Assert.Equal("RSI(14)", rsi.Name);
        Assert.Equal(0, rsi.MinValue);
        Assert.Equal(100, rsi.MaxValue);
    }

    [Fact]
    public void RSI_Calculate_ShouldReturnValues()
    {
        // Arrange
        var rsi = new RsiIndicator(5);
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 102),
            new DataPoint(2, 104),
            new DataPoint(3, 103),
            new DataPoint(4, 105),
            new DataPoint(5, 107),
            new DataPoint(6, 106),
            new DataPoint(7, 108)
        });

        // Act
        var result = rsi.Calculate(series);

        // Assert
        Assert.Equal(8, result.Count);
        // First Period values should be NaN
        for (int i = 0; i < 5; i++)
        {
            Assert.True(double.IsNaN(result[i].Y));
        }
        // After Period, should have valid values between 0 and 100
        for (int i = 5; i < result.Count; i++)
        {
            Assert.False(double.IsNaN(result[i].Y));
            Assert.InRange(result[i].Y, 0, 100);
        }
    }

    // MACD Tests
    [Fact]
    public void MACD_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var macd = new MacdIndicator();

        // Assert
        Assert.Equal(12, macd.FastPeriod);
        Assert.Equal(26, macd.SlowPeriod);
        Assert.Equal(9, macd.SignalPeriod);
        Assert.Equal("MACD(12,26,9)", macd.Name);
    }

    [Fact]
    public void MACD_Calculate_ShouldReturnValues()
    {
        // Arrange
        var macd = new MacdIndicator(5, 10, 3);
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 102),
            new DataPoint(2, 104),
            new DataPoint(3, 106),
            new DataPoint(4, 108),
            new DataPoint(5, 110),
            new DataPoint(6, 112),
            new DataPoint(7, 114),
            new DataPoint(8, 116),
            new DataPoint(9, 118),
            new DataPoint(10, 120),
            new DataPoint(11, 122),
            new DataPoint(12, 124)
        });

        // Act
        var result = macd.Calculate(series);

        // Assert
        Assert.Equal(13, result.Count);

        var macdPoint = result[12] as MacdDataPoint;
        Assert.NotNull(macdPoint);
        Assert.False(double.IsNaN(macdPoint.Macd));
        Assert.False(double.IsNaN(macdPoint.Signal));
        Assert.False(double.IsNaN(macdPoint.Histogram));
    }

    // Stochastic Tests
    [Fact]
    public void Stochastic_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var stoch = new StochasticIndicator();

        // Assert
        Assert.Equal(14, stoch.KPeriod);
        Assert.Equal(3, stoch.DPeriod);
        Assert.Equal(3, stoch.Smooth);
        Assert.Equal("Stoch(14,3,3)", stoch.Name);
        Assert.Equal(0, stoch.MinValue);
        Assert.Equal(100, stoch.MaxValue);
    }

    [Fact]
    public void Stochastic_Calculate_ShouldReturnValues()
    {
        // Arrange
        var stoch = new StochasticIndicator(5, 3, 1);
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 105, 95, 103),
            new OhlcDataPoint(1, 103, 108, 98, 106),
            new OhlcDataPoint(2, 106, 111, 101, 109),
            new OhlcDataPoint(3, 109, 114, 104, 112),
            new OhlcDataPoint(4, 112, 117, 107, 115),
            new OhlcDataPoint(5, 115, 120, 110, 118),
            new OhlcDataPoint(6, 118, 123, 113, 121),
            new OhlcDataPoint(7, 121, 126, 116, 124)
        });

        // Act
        var result = stoch.Calculate(series);

        // Assert
        Assert.Equal(8, result.Count);

        var stochPoint = result[7] as StochasticDataPoint;
        Assert.NotNull(stochPoint);
        Assert.False(double.IsNaN(stochPoint.K));
        Assert.InRange(stochPoint.K, 0, 100);
    }

    // OBV Tests
    [Fact]
    public void OBV_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var obv = new ObvIndicator();

        // Assert
        Assert.Equal("OBV", obv.Name);
    }

    [Fact]
    public void OBV_Calculate_ShouldAccumulateVolume()
    {
        // Arrange
        var obv = new ObvIndicator();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 105, 95, 100, 1000),
            new OhlcDataPoint(1, 100, 110, 100, 105, 1500), // Price up, OBV += 1500
            new OhlcDataPoint(2, 105, 115, 105, 103, 1200), // Price down, OBV -= 1200
            new OhlcDataPoint(3, 103, 113, 103, 103, 1000)  // Price same, OBV unchanged
        });

        // Act
        var result = obv.Calculate(series);

        // Assert
        Assert.Equal(4, result.Count);
        Assert.Equal(0, result[0].Y);      // Initial
        Assert.Equal(1500, result[1].Y);   // 0 + 1500
        Assert.Equal(300, result[2].Y);    // 1500 - 1200
        Assert.Equal(300, result[3].Y);    // 300 (unchanged)
    }

    // Volume Tests
    [Fact]
    public void Volume_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var volume = new VolumeIndicator();

        // Assert
        Assert.Equal("Volume", volume.Name);
        Assert.Equal(0, volume.MinValue);
    }

    [Fact]
    public void Volume_Calculate_ShouldReturnVolumes()
    {
        // Arrange
        var volume = new VolumeIndicator();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 105, 95, 100, 1000),
            new OhlcDataPoint(1, 100, 110, 100, 105, 1500),
            new OhlcDataPoint(2, 105, 115, 105, 110, 2000)
        });

        // Act
        var result = volume.Calculate(series);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(1000, result[0].Y);
        Assert.Equal(1500, result[1].Y);
        Assert.Equal(2000, result[2].Y);
        Assert.Equal(2000, volume.MaxValue);
    }

    // ATR Tests
    [Fact]
    public void ATR_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var atr = new AtrIndicator();

        // Assert
        Assert.Equal(14, atr.Period);
        Assert.Equal("ATR(14)", atr.Name);
        Assert.Equal(0, atr.MinValue);
    }

    [Fact]
    public void ATR_Calculate_ShouldReturnValues()
    {
        // Arrange
        var atr = new AtrIndicator(5);
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 90, 105),
            new OhlcDataPoint(1, 105, 115, 95, 110),
            new OhlcDataPoint(2, 110, 120, 100, 115),
            new OhlcDataPoint(3, 115, 125, 105, 120),
            new OhlcDataPoint(4, 120, 130, 110, 125),
            new OhlcDataPoint(5, 125, 135, 115, 130),
            new OhlcDataPoint(6, 130, 140, 120, 135)
        });

        // Act
        var result = atr.Calculate(series);

        // Assert
        // ATR adds first point as NaN, then processes remaining points
        // With 7 input points and period 5, we get first ATR at index 5
        Assert.True(result.Count >= 7);
        // First period values should be NaN
        for (int i = 0; i <= 4; i++)
        {
            Assert.True(double.IsNaN(result[i].Y));
        }
        // After period, should have valid positive values
        Assert.False(double.IsNaN(result[5].Y));
        Assert.True(result[5].Y > 0);
    }

    // CCI Tests
    [Fact]
    public void CCI_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var cci = new CciIndicator();

        // Assert
        Assert.Equal(20, cci.Period);
        Assert.Equal("CCI(20)", cci.Name);
        Assert.Equal(-200, cci.MinValue);
        Assert.Equal(200, cci.MaxValue);
    }

    [Fact]
    public void CCI_Calculate_ShouldReturnValues()
    {
        // Arrange
        var cci = new CciIndicator(5);
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 90, 100),
            new OhlcDataPoint(1, 100, 115, 95, 105),
            new OhlcDataPoint(2, 105, 120, 100, 110),
            new OhlcDataPoint(3, 110, 125, 105, 115),
            new OhlcDataPoint(4, 115, 130, 110, 120),
            new OhlcDataPoint(5, 120, 135, 115, 125),
            new OhlcDataPoint(6, 125, 140, 120, 130)
        });

        // Act
        var result = cci.Calculate(series);

        // Assert
        Assert.Equal(7, result.Count);
        // First Period-1 values should be NaN
        for (int i = 0; i < 4; i++)
        {
            Assert.True(double.IsNaN(result[i].Y));
        }
        // After period, should have valid values
        Assert.False(double.IsNaN(result[4].Y));
    }

    // ADX Tests
    [Fact]
    public void ADX_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var adx = new AdxIndicator();

        // Assert
        Assert.Equal(14, adx.Period);
        Assert.Equal("ADX(14)", adx.Name);
        Assert.Equal(0, adx.MinValue);
        Assert.Equal(100, adx.MaxValue);
    }

    [Fact]
    public void ADX_Calculate_ShouldReturnValues()
    {
        // Arrange
        var adx = new AdxIndicator(5);
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
            new OhlcDataPoint(10, 150, 160, 140, 155),
            new OhlcDataPoint(11, 155, 165, 145, 160)
        });

        // Act
        var result = adx.Calculate(series);

        // Assert
        Assert.True(result.Count > 0);

        // Check that we have ADX data points
        if (result.Count > 5)
        {
            var adxPoint = result[5] as AdxDataPoint;
            Assert.NotNull(adxPoint);
            // +DI and -DI should be available
            Assert.False(double.IsNaN(adxPoint.PlusDi));
            Assert.False(double.IsNaN(adxPoint.MinusDi));
        }
    }

    // Williams %R Tests
    [Fact]
    public void WilliamsR_Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var williamsR = new WilliamsRIndicator();

        // Assert
        Assert.Equal(14, williamsR.Period);
        Assert.Equal("Williams %R(14)", williamsR.Name);
        Assert.Equal(-100, williamsR.MinValue);
        Assert.Equal(0, williamsR.MaxValue);
    }

    [Fact]
    public void WilliamsR_Calculate_ShouldReturnValues()
    {
        // Arrange
        var williamsR = new WilliamsRIndicator(5);
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 90, 100),
            new OhlcDataPoint(1, 100, 115, 95, 105),
            new OhlcDataPoint(2, 105, 120, 100, 110),
            new OhlcDataPoint(3, 110, 125, 105, 115),
            new OhlcDataPoint(4, 115, 130, 110, 120),
            new OhlcDataPoint(5, 120, 135, 115, 125),
            new OhlcDataPoint(6, 125, 140, 120, 130)
        });

        // Act
        var result = williamsR.Calculate(series);

        // Assert
        Assert.Equal(7, result.Count);
        // First Period-1 values should be NaN
        for (int i = 0; i < 4; i++)
        {
            Assert.True(double.IsNaN(result[i].Y));
        }
        // After period, should have valid values between -100 and 0
        for (int i = 4; i < result.Count; i++)
        {
            Assert.False(double.IsNaN(result[i].Y));
            Assert.InRange(result[i].Y, -100, 0);
        }
    }

    // Integration Tests
    [Fact]
    public void AllPanelIndicators_EmptySeries_ShouldReturnEmpty()
    {
        // Arrange
        var emptySeries = new DataSeries<IDataPoint>(Array.Empty<IDataPoint>());
        var indicators = new IIndicator[]
        {
            new RsiIndicator(),
            new MacdIndicator(),
            new StochasticIndicator(),
            new ObvIndicator(),
            new VolumeIndicator(),
            new AtrIndicator(),
            new CciIndicator(),
            new AdxIndicator(),
            new WilliamsRIndicator()
        };

        // Act & Assert
        foreach (var indicator in indicators)
        {
            var result = indicator.Calculate(emptySeries);
            Assert.Empty(result);
        }
    }

    [Fact]
    public void AllPanelIndicators_ShouldHaveMinMaxValues()
    {
        // Arrange
        var indicators = new PanelIndicatorBase[]
        {
            new RsiIndicator(),
            new MacdIndicator(),
            new StochasticIndicator(),
            new ObvIndicator(),
            new VolumeIndicator(),
            new AtrIndicator(),
            new CciIndicator(),
            new AdxIndicator(),
            new WilliamsRIndicator()
        };

        // Act & Assert
        foreach (var indicator in indicators)
        {
            // MinValue and MaxValue should be defined (even if NaN for dynamic range)
            Assert.True(double.IsNaN(indicator.MinValue) || !double.IsInfinity(indicator.MinValue));
            Assert.True(double.IsNaN(indicator.MaxValue) || !double.IsInfinity(indicator.MaxValue));
        }
    }

    [Fact]
    public void AllPanelIndicators_ShouldHaveVisibilityControl()
    {
        // Arrange
        var indicators = new IndicatorBase[]
        {
            new RsiIndicator(),
            new MacdIndicator(),
            new StochasticIndicator(),
            new ObvIndicator(),
            new VolumeIndicator(),
            new AtrIndicator(),
            new CciIndicator(),
            new AdxIndicator(),
            new WilliamsRIndicator()
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

    [Fact]
    public void RSI_InsufficientData_ShouldReturnEmpty()
    {
        // Arrange
        var rsi = new RsiIndicator(20);
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 102)
        });

        // Act
        var result = rsi.Calculate(series);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void MACD_InsufficientData_ShouldReturnEmpty()
    {
        // Arrange
        var macd = new MacdIndicator(12, 26, 9);
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 102)
        });

        // Act
        var result = macd.Calculate(series);

        // Assert
        Assert.Empty(result);
    }
}
