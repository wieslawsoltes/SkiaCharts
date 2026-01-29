using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaCharts.Core.Exporting;
using SkiaCharts.Core.Theming;

namespace SkiaCharts.Core.Tests.Exporting;

public class ChartExporterTests
{
    [Fact]
    public void Export_ShouldCreatePngFile()
    {
        var chart = new LineChart();
        chart.Series.Add(new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 15),
            new DataPoint(2, 12),
            new DataPoint(3, 18)
        }, "Series 1"));

        var filePath = Path.Combine(Path.GetTempPath(), $"skiacharts-export-{Guid.NewGuid():N}.png");

        try
        {
            ChartExporter.Export(chart, filePath, 400, 300, new ExportSettings { Format = ExportFormat.Png });

            Assert.True(File.Exists(filePath));
            var fileInfo = new FileInfo(filePath);
            Assert.True(fileInfo.Length > 0);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
