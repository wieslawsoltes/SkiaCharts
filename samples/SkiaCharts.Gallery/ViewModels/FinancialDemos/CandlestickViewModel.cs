using System;
using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.FinancialDemos;

public class CandlestickViewModel : ReactiveObject
{
    public CandlestickViewModel()
    {
        Chart = new CandlestickChart
        {
            Configuration = new CandlestickChartConfiguration
            {
                ShowVolume = false
            },
            DefaultStyle = new CandlestickSeriesStyle
            {
                CandleType = CandleType.Candlestick,
                BullishColor = new SKColor(38, 166, 154), // Teal
                BearishColor = new SKColor(239, 83, 80),  // Red
                WickWidth = 1.5f,
                CandleWidthRatio = 0.7,
                UseHollowCandles = true
            }
        };

        // Generate sample stock price data
        var points = new List<IDataPoint>();
        var random = new Random(42);
        double price = 100.0;

        for (int i = 0; i < 30; i++)
        {
            double open = price;

            // Simulate price movement
            double change = (random.NextDouble() - 0.5) * 5;
            price += change;

            double high = Math.Max(open, price) + random.NextDouble() * 2;
            double low = Math.Min(open, price) - random.NextDouble() * 2;
            double close = price;
            double volume = 1000000 + random.Next(0, 500000);

            points.Add(new OhlcDataPoint(i, open, high, low, close, volume));
        }

        var series = new DataSeries<IDataPoint>(points, "Stock Price");
        Chart.Series.Add(series);
    }

    public CandlestickChart Chart { get; }
}
