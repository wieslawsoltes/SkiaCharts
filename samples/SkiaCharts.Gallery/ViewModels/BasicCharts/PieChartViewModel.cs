using System.Collections.Generic;
using ReactiveUI;
using SkiaCharts.Core.Charts;

namespace SkiaCharts.Gallery.ViewModels.BasicCharts;

public class PieChartViewModel : ReactiveObject
{
    public PieChartViewModel()
    {
        // Create pie chart
        Chart = new PieChart
        {
            Configuration = new PieChartConfiguration
            {
                IsDonut = false,
                StartAngle = -90f,
                LabelPosition = PieLabelPosition.Outside,
                LabelContent = PieLabelContent.NameAndPercentage,
                RadiusRatio = 0.8
            }
        };

        // Create sample data - market share
        var points = new List<PieDataPoint>
        {
            new PieDataPoint(35, "Product A"),
            new PieDataPoint(25, "Product B"),
            new PieDataPoint(20, "Product C"),
            new PieDataPoint(12, "Product D"),
            new PieDataPoint(8, "Product E")
        };

        var series = new Core.Data.DataSeries<Core.Data.IDataPoint>(points, "Market Share");
        Chart.Series.Add(series);
    }

    public PieChart Chart { get; }
}
