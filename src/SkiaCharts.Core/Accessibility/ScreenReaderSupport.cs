using System.Text;

namespace SkiaCharts.Core.Accessibility;

/// <summary>
/// Screen reader support and ARIA label generation.
/// </summary>
public static class ScreenReaderSupport
{
    /// <summary>
    /// Generates an accessible description for a chart.
    /// </summary>
    public static string GenerateChartDescription(ChartDescriptor chart)
    {
        var sb = new StringBuilder();

        // Chart type and title
        sb.Append($"{chart.ChartType} chart");
        if (!string.IsNullOrEmpty(chart.Title))
        {
            sb.Append($" titled '{chart.Title}'");
        }
        sb.Append(". ");

        // Axes
        if (!string.IsNullOrEmpty(chart.XAxisLabel))
        {
            sb.Append($"X-axis shows {chart.XAxisLabel}. ");
        }
        if (!string.IsNullOrEmpty(chart.YAxisLabel))
        {
            sb.Append($"Y-axis shows {chart.YAxisLabel}. ");
        }

        // Series count
        if (chart.SeriesCount > 0)
        {
            sb.Append($"Contains {chart.SeriesCount} data series");
            if (chart.SeriesCount == 1)
            {
                sb.Length -= 1; // Remove 's' from "series"
            }
            sb.Append(". ");
        }

        // Data summary
        if (chart.TotalDataPoints > 0)
        {
            sb.Append($"Total of {chart.TotalDataPoints} data points. ");
        }

        // Value range
        if (chart.MinValue.HasValue && chart.MaxValue.HasValue)
        {
            sb.Append($"Values range from {chart.MinValue:F2} to {chart.MaxValue:F2}. ");
        }

        return sb.ToString().Trim();
    }

    /// <summary>
    /// Generates an accessible description for a data series.
    /// </summary>
    public static string GenerateSeriesDescription(SeriesDescriptor series)
    {
        var sb = new StringBuilder();

        sb.Append($"Series: {series.Name}. ");

        if (series.DataPointCount > 0)
        {
            sb.Append($"{series.DataPointCount} data points. ");
        }

        if (series.MinValue.HasValue && series.MaxValue.HasValue)
        {
            sb.Append($"Range: {series.MinValue:F2} to {series.MaxValue:F2}. ");
        }

        if (series.Average.HasValue)
        {
            sb.Append($"Average: {series.Average:F2}. ");
        }

        if (!string.IsNullOrEmpty(series.Trend))
        {
            sb.Append($"Trend: {series.Trend}. ");
        }

        return sb.ToString().Trim();
    }

    /// <summary>
    /// Generates an accessible description for a data point.
    /// </summary>
    public static string GenerateDataPointDescription(DataPointDescriptor point)
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrEmpty(point.Label))
        {
            sb.Append($"{point.Label}: ");
        }

        sb.Append($"{point.Value:F2}");

        if (!string.IsNullOrEmpty(point.SeriesName))
        {
            sb.Append($" in {point.SeriesName}");
        }

        if (!string.IsNullOrEmpty(point.AdditionalInfo))
        {
            sb.Append($". {point.AdditionalInfo}");
        }

        return sb.ToString().Trim();
    }

    /// <summary>
    /// Generates a sonification audio description for trends.
    /// </summary>
    public static string GenerateTrendAudioDescription(double[] values)
    {
        if (values.Length == 0)
            return "No data";

        if (values.Length == 1)
            return $"Single value: {values[0]:F2}";

        var trend = AnalyzeTrend(values);
        var volatility = CalculateVolatility(values);

        var description = new StringBuilder();
        description.Append($"Data shows {trend} trend");

        if (volatility < 0.1)
            description.Append(" with low volatility");
        else if (volatility > 0.5)
            description.Append(" with high volatility");

        description.Append($". Starting at {values[0]:F2}, ending at {values[^1]:F2}");

        var min = values.Min();
        var max = values.Max();
        description.Append($". Range: {min:F2} to {max:F2}");

        return description.ToString();
    }

    private static string AnalyzeTrend(double[] values)
    {
        if (values.Length < 2)
            return "stable";

        var first = values[0];
        var last = values[^1];
        var change = (last - first) / first;

        if (Math.Abs(change) < 0.05)
            return "stable";
        else if (change > 0.2)
            return "strongly increasing";
        else if (change > 0)
            return "increasing";
        else if (change < -0.2)
            return "strongly decreasing";
        else
            return "decreasing";
    }

    private static double CalculateVolatility(double[] values)
    {
        if (values.Length < 2)
            return 0;

        var mean = values.Average();
        var variance = values.Select(v => Math.Pow(v - mean, 2)).Average();
        var stdDev = Math.Sqrt(variance);

        return stdDev / (mean != 0 ? Math.Abs(mean) : 1);
    }

    /// <summary>
    /// Generates ARIA live region announcement for data updates.
    /// </summary>
    public static string GenerateLiveUpdate(string elementType, string updateType, object? details = null)
    {
        return updateType switch
        {
            "added" => $"{elementType} added{(details != null ? $": {details}" : "")}",
            "removed" => $"{elementType} removed{(details != null ? $": {details}" : "")}",
            "updated" => $"{elementType} updated{(details != null ? $": {details}" : "")}",
            "selected" => $"{elementType} selected{(details != null ? $": {details}" : "")}",
            _ => $"{elementType} changed"
        };
    }
}

/// <summary>
/// Descriptor for chart accessibility metadata.
/// </summary>
public class ChartDescriptor
{
    public required string ChartType { get; init; }
    public string? Title { get; init; }
    public string? XAxisLabel { get; init; }
    public string? YAxisLabel { get; init; }
    public int SeriesCount { get; init; }
    public int TotalDataPoints { get; init; }
    public double? MinValue { get; init; }
    public double? MaxValue { get; init; }
}

/// <summary>
/// Descriptor for series accessibility metadata.
/// </summary>
public class SeriesDescriptor
{
    public required string Name { get; init; }
    public int DataPointCount { get; init; }
    public double? MinValue { get; init; }
    public double? MaxValue { get; init; }
    public double? Average { get; init; }
    public string? Trend { get; init; }
}

/// <summary>
/// Descriptor for data point accessibility metadata.
/// </summary>
public class DataPointDescriptor
{
    public string? Label { get; init; }
    public double Value { get; init; }
    public string? SeriesName { get; init; }
    public string? AdditionalInfo { get; init; }
}

/// <summary>
/// ARIA role types for chart elements.
/// </summary>
public enum AriaRole
{
    Chart,
    Graphics,
    GraphicsSymbol,
    GraphicsDocument,
    Group,
    Text,
    Img,
    Figure
}

/// <summary>
/// ARIA label builder for chart elements.
/// </summary>
public class AriaLabelBuilder
{
    private readonly StringBuilder _label = new();
    private readonly Dictionary<string, string> _properties = new();

    /// <summary>
    /// Sets the role.
    /// </summary>
    public AriaLabelBuilder Role(AriaRole role)
    {
        _properties["role"] = role.ToString().ToLowerInvariant();
        return this;
    }

    /// <summary>
    /// Sets the aria-label.
    /// </summary>
    public AriaLabelBuilder Label(string label)
    {
        _properties["aria-label"] = label;
        return this;
    }

    /// <summary>
    /// Sets the aria-describedby.
    /// </summary>
    public AriaLabelBuilder DescribedBy(string id)
    {
        _properties["aria-describedby"] = id;
        return this;
    }

    /// <summary>
    /// Sets aria-live region.
    /// </summary>
    public AriaLabelBuilder Live(string value = "polite")
    {
        _properties["aria-live"] = value;
        return this;
    }

    /// <summary>
    /// Sets aria-valuemin.
    /// </summary>
    public AriaLabelBuilder ValueMin(double value)
    {
        _properties["aria-valuemin"] = value.ToString("F2");
        return this;
    }

    /// <summary>
    /// Sets aria-valuemax.
    /// </summary>
    public AriaLabelBuilder ValueMax(double value)
    {
        _properties["aria-valuemax"] = value.ToString("F2");
        return this;
    }

    /// <summary>
    /// Sets aria-valuenow.
    /// </summary>
    public AriaLabelBuilder ValueNow(double value)
    {
        _properties["aria-valuenow"] = value.ToString("F2");
        return this;
    }

    /// <summary>
    /// Sets aria-valuetext.
    /// </summary>
    public AriaLabelBuilder ValueText(string text)
    {
        _properties["aria-valuetext"] = text;
        return this;
    }

    /// <summary>
    /// Builds the ARIA attribute string.
    /// </summary>
    public Dictionary<string, string> Build() => new(_properties);

    /// <summary>
    /// Creates a label for a chart.
    /// </summary>
    public static AriaLabelBuilder ForChart(string title, string chartType) =>
        new AriaLabelBuilder()
            .Role(AriaRole.Chart)
            .Label($"{chartType} chart{(string.IsNullOrEmpty(title) ? "" : $": {title}")}");

    /// <summary>
    /// Creates a label for a data series.
    /// </summary>
    public static AriaLabelBuilder ForSeries(string name, int pointCount) =>
        new AriaLabelBuilder()
            .Role(AriaRole.Group)
            .Label($"Data series: {name} with {pointCount} points");

    /// <summary>
    /// Creates a label for a data point.
    /// </summary>
    public static AriaLabelBuilder ForDataPoint(string label, double value) =>
        new AriaLabelBuilder()
            .Role(AriaRole.GraphicsSymbol)
            .Label($"{label}: {value:F2}")
            .ValueNow(value)
            .ValueText($"{value:F2}");
}

/// <summary>
/// Accessibility announcement manager for live updates.
/// </summary>
public class AccessibilityAnnouncer
{
    private readonly Queue<string> _announcements = new();
    private string? _currentAnnouncement;

    /// <summary>
    /// Event raised when a new announcement should be made.
    /// </summary>
    public event EventHandler<AnnouncementEventArgs>? AnnouncementRequested;

    /// <summary>
    /// Announces a message to screen readers.
    /// </summary>
    public void Announce(string message, AnnouncementPriority priority = AnnouncementPriority.Polite)
    {
        var announcement = new Announcement
        {
            Message = message,
            Priority = priority,
            Timestamp = DateTime.UtcNow
        };

        if (priority == AnnouncementPriority.Assertive)
        {
            _currentAnnouncement = message;
            AnnouncementRequested?.Invoke(this, new AnnouncementEventArgs(announcement));
        }
        else
        {
            _announcements.Enqueue(message);
        }
    }

    /// <summary>
    /// Gets the next announcement to be made.
    /// </summary>
    public string? GetNextAnnouncement()
    {
        if (_announcements.Count > 0)
        {
            _currentAnnouncement = _announcements.Dequeue();
            return _currentAnnouncement;
        }

        return null;
    }

    /// <summary>
    /// Clears all pending announcements.
    /// </summary>
    public void Clear()
    {
        _announcements.Clear();
        _currentAnnouncement = null;
    }
}

/// <summary>
/// Announcement priority levels.
/// </summary>
public enum AnnouncementPriority
{
    /// <summary>Polite announcement (waits for user to be idle).</summary>
    Polite,

    /// <summary>Assertive announcement (interrupts immediately).</summary>
    Assertive
}

/// <summary>
/// Represents an accessibility announcement.
/// </summary>
public class Announcement
{
    public required string Message { get; init; }
    public AnnouncementPriority Priority { get; init; }
    public DateTime Timestamp { get; init; }
}

/// <summary>
/// Event args for announcement requests.
/// </summary>
public class AnnouncementEventArgs : EventArgs
{
    public Announcement Announcement { get; }

    public AnnouncementEventArgs(Announcement announcement)
    {
        Announcement = announcement;
    }
}
