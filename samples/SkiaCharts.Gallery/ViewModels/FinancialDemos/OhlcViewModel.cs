using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.FinancialDemos;

public class OhlcViewModel : ReactiveObject
{
    public OhlcViewModel()
    {
        Chart = new CandlestickChart
        {
            Configuration = new CandlestickChartConfiguration
            {
                ShowVolume = false
            },
            DefaultStyle = new CandlestickSeriesStyle
            {
                CandleType = CandleType.OhlcBar,  // OHLC bars instead of candlesticks
                BullishColor = new SKColor(76, 175, 80),  // Green
                BearishColor = new SKColor(244, 67, 54),  // Red
                WickWidth = 2f,
                CandleWidthRatio = 0.6,
                OhlcTickRatio = 0.5
            }
        };

        // Generate sample commodity price data
        var points = new List<IDataPoint>();
        var random = new Random(42);
        double price = 50.0;

        for (int i = 0; i < 40; i++)
        {
            double open = price;

            // Simulate price movement with more volatility
            double change = (random.NextDouble() - 0.5) * 3;
            price += change;

            double high = Math.Max(open, price) + random.NextDouble() * 1.5;
            double low = Math.Min(open, price) - random.NextDouble() * 1.5;
            double close = price;
            double volume = 500000 + random.Next(0, 300000);

            points.Add(new OhlcDataPoint(i, open, high, low, close, volume));
        }

        var series = new DataSeries<IDataPoint>(points, "Commodity Price");
        Chart.Series.Add(series);
    }

    public CandlestickChart Chart { get; }
}
