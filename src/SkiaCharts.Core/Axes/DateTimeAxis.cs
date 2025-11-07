using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;

namespace SkiaCharts.Core.Axes;

/// <summary>
/// Represents a time-based axis that handles DateTime values.
/// Values are stored as OADate (double) internally for compatibility with IAxis.
/// </summary>
public class DateTimeAxis : IAxis
{
    private DataRange _visibleRange;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeAxis"/> class.
    /// </summary>
    public DateTimeAxis()
    {
        var now = DateTime.Now.ToOADate();
        _visibleRange = new DataRange(now - 7, now); // Default: last 7 days
        Position = AxisPosition.Bottom;
        AutoScale = true;
        ShowGridLines = true;
        ShowLabels = true;
        IsVisible = true;
        Layer = RenderLayer.Grid;
        TargetTickCount = 8;
    }

    /// <inheritdoc/>
    public string? Title { get; set; }

    /// <inheritdoc/>
    public AxisPosition Position { get; set; }

    /// <inheritdoc/>
    public DataRange VisibleRange
    {
        get => _visibleRange;
        set => _visibleRange = value;
    }

    /// <inheritdoc/>
    public bool AutoScale { get; set; }

    /// <inheritdoc/>
    public bool ShowGridLines { get; set; }

    /// <inheritdoc/>
    public bool ShowLabels { get; set; }

    /// <inheritdoc/>
    public double? MinValue { get; set; }

    /// <inheritdoc/>
    public double? MaxValue { get; set; }

    /// <inheritdoc/>
    public bool IsVisible { get; set; }

    /// <inheritdoc/>
    public RenderLayer Layer { get; }

    /// <summary>
    /// Gets or sets the desired number of tick marks.
    /// </summary>
    public int TargetTickCount { get; set; }

    /// <summary>
    /// Gets or sets the format string for tick labels.
    /// If not set, format is automatically determined based on time span.
    /// </summary>
    public string? LabelFormat { get; set; }

    /// <inheritdoc/>
    public IReadOnlyList<TickInfo> GenerateTicks()
    {
        var ticks = new List<TickInfo>();
        var spanDays = _visibleRange.Span;

        if (spanDays <= 0)
        {
            return ticks;
        }

        var minDate = DateTime.FromOADate(_visibleRange.Min);
        var maxDate = DateTime.FromOADate(_visibleRange.Max);

        // Determine appropriate interval based on time span
        var interval = DetermineTickInterval(spanDays);

        // Generate ticks based on interval type
        var currentDate = RoundToInterval(minDate, interval);

        while (currentDate <= maxDate)
        {
            var oaDate = currentDate.ToOADate();
            if (oaDate >= _visibleRange.Min && oaDate <= _visibleRange.Max)
            {
                ticks.Add(new TickInfo(oaDate, FormatValue(oaDate), true));
            }

            currentDate = AddInterval(currentDate, interval);
        }

        return ticks;
    }

    /// <inheritdoc/>
    public string FormatValue(double value)
    {
        try
        {
            var date = DateTime.FromOADate(value);

            if (!string.IsNullOrEmpty(LabelFormat))
            {
                return date.ToString(LabelFormat);
            }

            // Auto-format based on time span
            var spanDays = _visibleRange.Span;

            if (spanDays < 1) // Less than 1 day
            {
                return date.ToString("HH:mm");
            }
            else if (spanDays < 7) // Less than 1 week
            {
                return date.ToString("ddd HH:mm");
            }
            else if (spanDays < 60) // Less than 2 months
            {
                return date.ToString("MMM dd");
            }
            else if (spanDays < 365) // Less than 1 year
            {
                return date.ToString("MMM yyyy");
            }
            else // 1 year or more
            {
                return date.ToString("yyyy");
            }
        }
        catch
        {
            return value.ToString("F0");
        }
    }

    /// <inheritdoc/>
    public DataRange CalculateOptimalRange(DataRange dataRange)
    {
        if (!dataRange.IsValid)
        {
            var now = DateTime.Now.ToOADate();
            return new DataRange(now - 30, now); // Default: last 30 days
        }

        var spanDays = dataRange.Span;
        if (spanDays <= 0)
        {
            // Data has no span, create artificial range
            var center = dataRange.Min;
            return new DataRange(center - 1, center + 1); // +/- 1 day
        }

        // Add 2% padding to time ranges
        return dataRange.WithPadding(0.02);
    }

    /// <inheritdoc/>
    public void Render(IRenderContext context)
    {
        // Basic rendering implementation will be enhanced later
        // For now, this is a placeholder
    }

    /// <summary>
    /// Determines the appropriate tick interval based on the time span.
    /// </summary>
    private DateTimeInterval DetermineTickInterval(double spanDays)
    {
        var spanHours = spanDays * 24;
        var spanMinutes = spanHours * 60;
        var spanSeconds = spanMinutes * 60;

        // Seconds
        if (spanSeconds < 60) return DateTimeInterval.Second;

        // Minutes
        if (spanMinutes < 60) return DateTimeInterval.Minute;
        if (spanMinutes < 360) return DateTimeInterval.FiveMinutes;
        if (spanMinutes < 720) return DateTimeInterval.FifteenMinutes;

        // Hours
        if (spanHours < 24) return DateTimeInterval.Hour;
        if (spanHours < 72) return DateTimeInterval.ThreeHours;
        if (spanHours < 168) return DateTimeInterval.SixHours;

        // Days
        if (spanDays < 14) return DateTimeInterval.Day;
        if (spanDays < 60) return DateTimeInterval.Week;

        // Months
        if (spanDays < 365) return DateTimeInterval.Month;
        if (spanDays < 730) return DateTimeInterval.Quarter;

        // Years
        if (spanDays < 3650) return DateTimeInterval.Year;

        return DateTimeInterval.Decade;
    }

    /// <summary>
    /// Rounds a DateTime to the nearest interval boundary.
    /// </summary>
    private DateTime RoundToInterval(DateTime date, DateTimeInterval interval)
    {
        return interval switch
        {
            DateTimeInterval.Second => new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second),
            DateTimeInterval.Minute => new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0),
            DateTimeInterval.FiveMinutes => new DateTime(date.Year, date.Month, date.Day, date.Hour, (date.Minute / 5) * 5, 0),
            DateTimeInterval.FifteenMinutes => new DateTime(date.Year, date.Month, date.Day, date.Hour, (date.Minute / 15) * 15, 0),
            DateTimeInterval.Hour => new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0),
            DateTimeInterval.ThreeHours => new DateTime(date.Year, date.Month, date.Day, (date.Hour / 3) * 3, 0, 0),
            DateTimeInterval.SixHours => new DateTime(date.Year, date.Month, date.Day, (date.Hour / 6) * 6, 0, 0),
            DateTimeInterval.Day => new DateTime(date.Year, date.Month, date.Day),
            DateTimeInterval.Week => date.AddDays(-(int)date.DayOfWeek),
            DateTimeInterval.Month => new DateTime(date.Year, date.Month, 1),
            DateTimeInterval.Quarter => new DateTime(date.Year, ((date.Month - 1) / 3) * 3 + 1, 1),
            DateTimeInterval.Year => new DateTime(date.Year, 1, 1),
            DateTimeInterval.Decade => new DateTime((date.Year / 10) * 10, 1, 1),
            _ => date
        };
    }

    /// <summary>
    /// Adds one interval to a DateTime.
    /// </summary>
    private DateTime AddInterval(DateTime date, DateTimeInterval interval)
    {
        return interval switch
        {
            DateTimeInterval.Second => date.AddSeconds(1),
            DateTimeInterval.Minute => date.AddMinutes(1),
            DateTimeInterval.FiveMinutes => date.AddMinutes(5),
            DateTimeInterval.FifteenMinutes => date.AddMinutes(15),
            DateTimeInterval.Hour => date.AddHours(1),
            DateTimeInterval.ThreeHours => date.AddHours(3),
            DateTimeInterval.SixHours => date.AddHours(6),
            DateTimeInterval.Day => date.AddDays(1),
            DateTimeInterval.Week => date.AddDays(7),
            DateTimeInterval.Month => date.AddMonths(1),
            DateTimeInterval.Quarter => date.AddMonths(3),
            DateTimeInterval.Year => date.AddYears(1),
            DateTimeInterval.Decade => date.AddYears(10),
            _ => date
        };
    }

    /// <summary>
    /// Defines time intervals for tick generation.
    /// </summary>
    private enum DateTimeInterval
    {
        Second,
        Minute,
        FiveMinutes,
        FifteenMinutes,
        Hour,
        ThreeHours,
        SixHours,
        Day,
        Week,
        Month,
        Quarter,
        Year,
        Decade
    }
}
