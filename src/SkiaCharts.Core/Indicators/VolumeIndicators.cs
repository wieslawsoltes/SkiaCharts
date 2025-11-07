using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Indicators;

/// <summary>
/// OBV (On-Balance Volume) indicator.
/// Measures buying and selling pressure as a cumulative indicator.
/// </summary>
public class ObvIndicator : PanelIndicatorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObvIndicator"/> class.
    /// </summary>
    public ObvIndicator()
    {
        MinValue = double.NaN; // Will be calculated dynamically
        MaxValue = double.NaN;
    }

    /// <inheritdoc/>
    public override string Name => "OBV";

    /// <inheritdoc/>
    public override IDataSeries<IDataPoint> Calculate(IDataSeries<IDataPoint> series)
    {
        var result = new List<IDataPoint>();

        if (series.Count < 2)
        {
            return new DataSeries<IDataPoint>(result);
        }

        double obv = 0;
        double minVal = double.MaxValue;
        double maxVal = double.MinValue;

        // First point
        result.Add(new DataPoint(series[0].X, obv));
        minVal = Math.Min(minVal, obv);
        maxVal = Math.Max(maxVal, obv);

        // Calculate OBV for remaining points
        for (int i = 1; i < series.Count; i++)
        {
            double currentClose = GetClose(series[i]);
            double previousClose = GetClose(series[i - 1]);
            double volume = GetVolume(series[i]);

            if (currentClose > previousClose)
            {
                obv += volume;
            }
            else if (currentClose < previousClose)
            {
                obv -= volume;
            }
            // If close == previous close, OBV remains unchanged

            result.Add(new DataPoint(series[i].X, obv));
            minVal = Math.Min(minVal, obv);
            maxVal = Math.Max(maxVal, obv);
        }

        MinValue = minVal;
        MaxValue = maxVal;

        return new DataSeries<IDataPoint>(result);
    }
}

/// <summary>
/// Volume indicator.
/// Simply displays volume bars.
/// </summary>
public class VolumeIndicator : PanelIndicatorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VolumeIndicator"/> class.
    /// </summary>
    public VolumeIndicator()
    {
        MinValue = 0;
        MaxValue = double.NaN; // Will be calculated dynamically
    }

    /// <inheritdoc/>
    public override string Name => "Volume";

    /// <inheritdoc/>
    public override IDataSeries<IDataPoint> Calculate(IDataSeries<IDataPoint> series)
    {
        var result = new List<IDataPoint>();

        if (series.Count == 0)
        {
            return new DataSeries<IDataPoint>(result);
        }

        double maxVol = double.MinValue;

        for (int i = 0; i < series.Count; i++)
        {
            double volume = GetVolume(series[i]);
            result.Add(new DataPoint(series[i].X, volume));
            maxVol = Math.Max(maxVol, volume);
        }

        MaxValue = maxVol;

        return new DataSeries<IDataPoint>(result);
    }
}
