using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;

namespace SkiaCharts.Gallery.ViewModels.BasicCharts;

public class DonutChartViewModel : ReactiveObject
{
    public DonutChartViewModel()
    {
        // Create donut chart
        Chart = new PieChart
        {
            Configuration = new PieChartConfiguration
            {
                IsDonut = true,
                InnerRadiusRatio = 0.6,
                StartAngle = -90f,
                LabelPosition = PieLabelPosition.Outside,
                LabelContent = PieLabelContent.NameAndPercentage,
                RadiusRatio = 0.8
            }
        };

        // Create sample data - browser usage
        var points = new List<PieDataPoint>
        {
            new PieDataPoint(42, "Chrome"),
            new PieDataPoint(28, "Safari"),
            new PieDataPoint(15, "Firefox"),
            new PieDataPoint(8, "Edge"),
            new PieDataPoint(7, "Others")
        };

        var series = new Core.Data.DataSeries<Core.Data.IDataPoint>(points, "Browser Usage");
        Chart.Series.Add(series);
    }

    public PieChart Chart { get; }
}
