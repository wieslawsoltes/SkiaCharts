using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;
using SkiaCharts.Core.Data;

namespace SkiaCharts.Avalonia.Controls;

/// <summary>
/// Extended properties for SkiaChartView.
/// </summary>
public partial class SkiaChartView
{
    #region Title Properties

    /// <summary>
    /// Defines the <see cref="Title"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<SkiaChartView, string?>(nameof(Title));

    /// <summary>
    /// Gets or sets the chart title.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="Subtitle"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> SubtitleProperty =
        AvaloniaProperty.Register<SkiaChartView, string?>(nameof(Subtitle));

    /// <summary>
    /// Gets or sets the chart subtitle.
    /// </summary>
    public string? Subtitle
    {
        get => GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    #endregion

    #region Series Properties

    /// <summary>
    /// Defines the <see cref="Series"/> property.
    /// </summary>
    public static readonly StyledProperty<AvaloniaList<DataSeries<IDataPoint>>?> SeriesProperty =
        AvaloniaProperty.Register<SkiaChartView, AvaloniaList<DataSeries<IDataPoint>>?>(
            nameof(Series),
            defaultValue: new AvaloniaList<DataSeries<IDataPoint>>());

    /// <summary>
    /// Gets or sets the data series collection.
    /// </summary>
    public AvaloniaList<DataSeries<IDataPoint>>? Series
    {
        get => GetValue(SeriesProperty);
        set => SetValue(SeriesProperty, value);
    }

    #endregion

    #region Legend Properties

    /// <summary>
    /// Defines the <see cref="ShowLegend"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLegendProperty =
        AvaloniaProperty.Register<SkiaChartView, bool>(
            nameof(ShowLegend),
            defaultValue: true);

    /// <summary>
    /// Gets or sets whether the legend is visible.
    /// </summary>
    public bool ShowLegend
    {
        get => GetValue(ShowLegendProperty);
        set => SetValue(ShowLegendProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="LegendPosition"/> property.
    /// </summary>
    public static readonly StyledProperty<LegendPosition> LegendPositionProperty =
        AvaloniaProperty.Register<SkiaChartView, LegendPosition>(
            nameof(LegendPosition),
            defaultValue: LegendPosition.Right);

    /// <summary>
    /// Gets or sets the legend position.
    /// </summary>
    public LegendPosition LegendPosition
    {
        get => GetValue(LegendPositionProperty);
        set => SetValue(LegendPositionProperty, value);
    }

    #endregion

    #region Grid Properties

    /// <summary>
    /// Defines the <see cref="ShowGrid"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowGridProperty =
        AvaloniaProperty.Register<SkiaChartView, bool>(
            nameof(ShowGrid),
            defaultValue: true);

    /// <summary>
    /// Gets or sets whether the grid is visible.
    /// </summary>
    public bool ShowGrid
    {
        get => GetValue(ShowGridProperty);
        set => SetValue(ShowGridProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="ShowMinorGrid"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowMinorGridProperty =
        AvaloniaProperty.Register<SkiaChartView, bool>(
            nameof(ShowMinorGrid),
            defaultValue: false);

    /// <summary>
    /// Gets or sets whether minor grid lines are visible.
    /// </summary>
    public bool ShowMinorGrid
    {
        get => GetValue(ShowMinorGridProperty);
        set => SetValue(ShowMinorGridProperty, value);
    }

    #endregion

    #region Axis Properties

    /// <summary>
    /// Defines the <see cref="XAxisLabel"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> XAxisLabelProperty =
        AvaloniaProperty.Register<SkiaChartView, string?>(nameof(XAxisLabel));

    /// <summary>
    /// Gets or sets the X-axis label.
    /// </summary>
    public string? XAxisLabel
    {
        get => GetValue(XAxisLabelProperty);
        set => SetValue(XAxisLabelProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="YAxisLabel"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> YAxisLabelProperty =
        AvaloniaProperty.Register<SkiaChartView, string?>(nameof(YAxisLabel));

    /// <summary>
    /// Gets or sets the Y-axis label.
    /// </summary>
    public string? YAxisLabel
    {
        get => GetValue(YAxisLabelProperty);
        set => SetValue(YAxisLabelProperty, value);
    }

    #endregion

    #region Animation Properties

    /// <summary>
    /// Defines the <see cref="EnableAnimations"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> EnableAnimationsProperty =
        AvaloniaProperty.Register<SkiaChartView, bool>(
            nameof(EnableAnimations),
            defaultValue: true);

    /// <summary>
    /// Gets or sets whether animations are enabled.
    /// </summary>
    public bool EnableAnimations
    {
        get => GetValue(EnableAnimationsProperty);
        set => SetValue(EnableAnimationsProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="AnimationDuration"/> property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> AnimationDurationProperty =
        AvaloniaProperty.Register<SkiaChartView, TimeSpan>(
            nameof(AnimationDuration),
            defaultValue: TimeSpan.FromMilliseconds(500));

    /// <summary>
    /// Gets or sets the animation duration.
    /// </summary>
    public TimeSpan AnimationDuration
    {
        get => GetValue(AnimationDurationProperty);
        set => SetValue(AnimationDurationProperty, value);
    }

    #endregion

    #region Interaction Properties

    /// <summary>
    /// Defines the <see cref="EnableTooltips"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> EnableTooltipsProperty =
        AvaloniaProperty.Register<SkiaChartView, bool>(
            nameof(EnableTooltips),
            defaultValue: true);

    /// <summary>
    /// Gets or sets whether tooltips are enabled.
    /// </summary>
    public bool EnableTooltips
    {
        get => GetValue(EnableTooltipsProperty);
        set => SetValue(EnableTooltipsProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="EnableZoom"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> EnableZoomProperty =
        AvaloniaProperty.Register<SkiaChartView, bool>(
            nameof(EnableZoom),
            defaultValue: true);

    /// <summary>
    /// Gets or sets whether zooming is enabled.
    /// </summary>
    public bool EnableZoom
    {
        get => GetValue(EnableZoomProperty);
        set => SetValue(EnableZoomProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="EnablePan"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> EnablePanProperty =
        AvaloniaProperty.Register<SkiaChartView, bool>(
            nameof(EnablePan),
            defaultValue: true);

    /// <summary>
    /// Gets or sets whether panning is enabled.
    /// </summary>
    public bool EnablePan
    {
        get => GetValue(EnablePanProperty);
        set => SetValue(EnablePanProperty, value);
    }

    #endregion

    #region Style Properties

    /// <summary>
    /// Defines the <see cref="LineWidth"/> property.
    /// </summary>
    public static readonly StyledProperty<double> LineWidthProperty =
        AvaloniaProperty.Register<SkiaChartView, double>(
            nameof(LineWidth),
            defaultValue: 2.0);

    /// <summary>
    /// Gets or sets the line width for line charts.
    /// </summary>
    public double LineWidth
    {
        get => GetValue(LineWidthProperty);
        set => SetValue(LineWidthProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="MarkerSize"/> property.
    /// </summary>
    public static readonly StyledProperty<double> MarkerSizeProperty =
        AvaloniaProperty.Register<SkiaChartView, double>(
            nameof(MarkerSize),
            defaultValue: 6.0);

    /// <summary>
    /// Gets or sets the marker size.
    /// </summary>
    public double MarkerSize
    {
        get => GetValue(MarkerSizeProperty);
        set => SetValue(MarkerSizeProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="ShowMarkers"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowMarkersProperty =
        AvaloniaProperty.Register<SkiaChartView, bool>(
            nameof(ShowMarkers),
            defaultValue: true);

    /// <summary>
    /// Gets or sets whether markers are shown on data points.
    /// </summary>
    public bool ShowMarkers
    {
        get => GetValue(ShowMarkersProperty);
        set => SetValue(ShowMarkersProperty, value);
    }

    #endregion
}

/// <summary>
/// Legend position enumeration.
/// </summary>
public enum LegendPosition
{
    /// <summary>No legend.</summary>
    None,
    /// <summary>Top position.</summary>
    Top,
    /// <summary>Right position.</summary>
    Right,
    /// <summary>Bottom position.</summary>
    Bottom,
    /// <summary>Left position.</summary>
    Left,
    /// <summary>Top-right corner.</summary>
    TopRight,
    /// <summary>Top-left corner.</summary>
    TopLeft,
    /// <summary>Bottom-right corner.</summary>
    BottomRight,
    /// <summary>Bottom-left corner.</summary>
    BottomLeft
}
