namespace SkiaCharts.Core.Data;

/// <summary>
/// Represents a 2D grid of values for heatmap visualization.
/// </summary>
public class HeatmapData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HeatmapData"/> class.
    /// </summary>
    /// <param name="values">The 2D array of values [row, column].</param>
    /// <param name="xLabels">Optional X-axis labels (column labels).</param>
    /// <param name="yLabels">Optional Y-axis labels (row labels).</param>
    public HeatmapData(double[,] values, string[]? xLabels = null, string[]? yLabels = null)
    {
        Values = values ?? throw new ArgumentNullException(nameof(values));
        Rows = values.GetLength(0);
        Columns = values.GetLength(1);
        XLabels = xLabels;
        YLabels = yLabels;

        CalculateMinMax();
    }

    /// <summary>
    /// Gets the 2D array of values [row, column].
    /// </summary>
    public double[,] Values { get; }

    /// <summary>
    /// Gets the number of rows.
    /// </summary>
    public int Rows { get; }

    /// <summary>
    /// Gets the number of columns.
    /// </summary>
    public int Columns { get; }

    /// <summary>
    /// Gets the X-axis labels (column labels).
    /// </summary>
    public string[]? XLabels { get; }

    /// <summary>
    /// Gets the Y-axis labels (row labels).
    /// </summary>
    public string[]? YLabels { get; }

    /// <summary>
    /// Gets the minimum value in the dataset.
    /// </summary>
    public double MinValue { get; private set; }

    /// <summary>
    /// Gets the maximum value in the dataset.
    /// </summary>
    public double MaxValue { get; private set; }

    private void CalculateMinMax()
    {
        MinValue = double.MaxValue;
        MaxValue = double.MinValue;

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                var value = Values[row, col];
                if (!double.IsNaN(value) && !double.IsInfinity(value))
                {
                    MinValue = Math.Min(MinValue, value);
                    MaxValue = Math.Max(MaxValue, value);
                }
            }
        }

        // Handle edge case where all values are invalid
        if (MinValue == double.MaxValue || MaxValue == double.MinValue)
        {
            MinValue = 0;
            MaxValue = 1;
        }
    }

    /// <summary>
    /// Gets the value at the specified row and column.
    /// </summary>
    public double GetValue(int row, int col)
    {
        if (row < 0 || row >= Rows || col < 0 || col >= Columns)
        {
            return double.NaN;
        }
        return Values[row, col];
    }

    /// <summary>
    /// Gets the X label for the specified column.
    /// </summary>
    public string GetXLabel(int col)
    {
        if (XLabels != null && col >= 0 && col < XLabels.Length)
        {
            return XLabels[col];
        }
        return col.ToString();
    }

    /// <summary>
    /// Gets the Y label for the specified row.
    /// </summary>
    public string GetYLabel(int row)
    {
        if (YLabels != null && row >= 0 && row < YLabels.Length)
        {
            return YLabels[row];
        }
        return row.ToString();
    }
}
