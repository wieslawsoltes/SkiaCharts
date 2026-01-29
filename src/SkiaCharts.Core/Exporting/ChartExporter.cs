using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Theming;
using SkiaSharp;

namespace SkiaCharts.Core.Exporting;

/// <summary>
/// Exports charts to raster and document formats.
/// </summary>
public static class ChartExporter
{
    /// <summary>
    /// Exports a chart to the specified file path.
    /// </summary>
    /// <param name="chart">The chart to export.</param>
    /// <param name="filePath">The output file path.</param>
    /// <param name="width">The logical width in pixels (96 DPI).</param>
    /// <param name="height">The logical height in pixels (96 DPI).</param>
    /// <param name="settings">Optional export settings.</param>
    public static void Export(ChartBase chart, string filePath, int width, int height, ExportSettings? settings = null)
    {
        if (chart == null) throw new ArgumentNullException(nameof(chart));
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("File path cannot be empty.", nameof(filePath));
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive.");
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive.");

        settings ??= ExportSettings.ForWeb();

        var format = ResolveFormat(filePath, settings.Format);
        EnsureOutputDirectory(filePath);

        var previousTheme = chart.Theme;
        var exportTheme = settings.ExportTheme?.Clone() ?? chart.Theme?.Clone() ?? ThemePresets.Light;

        if (format == ExportFormat.Png && settings.TransparentBackground)
        {
            exportTheme.Background.Color = SKColors.Transparent;
        }

        chart.Theme = exportTheme;

        try
        {
            switch (format)
            {
                case ExportFormat.Png:
                case ExportFormat.Jpeg:
                case ExportFormat.WebP:
                    ExportRaster(chart, filePath, width, height, settings, format);
                    break;
                case ExportFormat.Pdf:
                    ExportPdf(chart, filePath, width, height, settings);
                    break;
                case ExportFormat.Svg:
                    throw new NotSupportedException("SVG export requires SkiaSharp.Svg, which is not referenced.");
                default:
                    throw new NotSupportedException($"Export format '{format}' is not supported.");
            }
        }
        finally
        {
            chart.Theme = previousTheme;
        }
    }

    private static void ExportRaster(ChartBase chart, string filePath, int width, int height, ExportSettings settings, ExportFormat format)
    {
        var scale = Math.Max(0.1f, settings.Dpi / 96f);
        var pixelWidth = Math.Max(1, (int)Math.Round(width * scale));
        var pixelHeight = Math.Max(1, (int)Math.Round(height * scale));

        var info = new SKImageInfo(pixelWidth, pixelHeight, SKColorType.Bgra8888, SKAlphaType.Premul);
        using var surface = SKSurface.Create(info);
        var canvas = surface.Canvas;

        if (Math.Abs(scale - 1f) > 0.0001f)
        {
            canvas.Scale(scale);
        }

        chart.Render(canvas, width, height);

        using var image = surface.Snapshot();
        var encodedFormat = format switch
        {
            ExportFormat.Png => SKEncodedImageFormat.Png,
            ExportFormat.Jpeg => SKEncodedImageFormat.Jpeg,
            ExportFormat.WebP => SKEncodedImageFormat.Webp,
            _ => throw new NotSupportedException($"Raster format '{format}' is not supported.")
        };

        var quality = format == ExportFormat.Jpeg ? settings.JpegQuality : 100;
        using var data = image.Encode(encodedFormat, quality);
        using var stream = File.Open(filePath, FileMode.Create, FileAccess.Write);
        data.SaveTo(stream);
    }

    private static void ExportPdf(ChartBase chart, string filePath, int width, int height, ExportSettings settings)
    {
        var dpi = Math.Max(72, settings.Dpi);
        var pointsWidth = width * 72f / dpi;
        var pointsHeight = height * 72f / dpi;

        var metadata = new SKDocumentPdfMetadata
        {
            RasterDpi = dpi
        };

        using var document = SKDocument.CreatePdf(filePath, metadata);
        using var canvas = document.BeginPage(pointsWidth, pointsHeight);
        var scale = pointsWidth / width;

        if (Math.Abs(scale - 1f) > 0.0001f)
        {
            canvas.Scale(scale);
        }

        chart.Render(canvas, width, height);
        document.EndPage();
        document.Close();
    }

    private static ExportFormat ResolveFormat(string filePath, ExportFormat fallbackFormat)
    {
        var extension = Path.GetExtension(filePath)?.ToLowerInvariant();

        return extension switch
        {
            ".png" => ExportFormat.Png,
            ".jpg" => ExportFormat.Jpeg,
            ".jpeg" => ExportFormat.Jpeg,
            ".webp" => ExportFormat.WebP,
            ".pdf" => ExportFormat.Pdf,
            ".svg" => ExportFormat.Svg,
            _ => fallbackFormat
        };
    }

    private static void EnsureOutputDirectory(string filePath)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
