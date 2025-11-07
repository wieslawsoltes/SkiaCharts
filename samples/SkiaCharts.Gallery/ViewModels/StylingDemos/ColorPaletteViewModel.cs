using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaCharts.Core.Theming;
using SkiaSharp;

namespace SkiaCharts.Gallery.ViewModels.StylingDemos;

public class ColorPaletteViewModel : ReactiveObject
{
    private ColorPalette _selectedPalette;

    public ColorPaletteViewModel()
    {
        // Get all available palettes
        Palettes = new ObservableCollection<ColorPalette>
        {
            ColorPalettes.Default,
            ColorPalettes.Vibrant,
            ColorPalettes.Pastel,
            ColorPalettes.Professional,
            ColorPalettes.BluesSequential,
            ColorPalettes.GreensSequential,
            ColorPalettes.RedsSequential,
            ColorPalettes.Heat
        };

        _selectedPalette = ColorPalettes.Default;

        // Create a bar chart to show the palette colors
        CreateChart();
    }

    public ObservableCollection<ColorPalette> Palettes { get; }

    public ColorPalette SelectedPalette
    {
        get => _selectedPalette;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedPalette, value);
            UpdateChartColors();
        }
    }

    public BarChart Chart { get; private set; } = null!;

    private void CreateChart()
    {
        Chart = new BarChart
        {
            Configuration = new BarChartConfiguration
            {
                Orientation = BarOrientation.Vertical,
                StackMode = BarStackMode.None
            }
        };

        // Create 8 series to show palette colors
        for (int i = 0; i < 8; i++)
        {
            var points = new List<IDataPoint>
            {
                new DataPoint(i + 1, 50 + (i * 5))
            };
            var series = new DataSeries<IDataPoint>(points, $"Color {i + 1}");
            Chart.Series.Add(series);
        }

        UpdateChartColors();
    }

    private void UpdateChartColors()
    {
        if (Chart == null) return;

        for (int i = 0; i < Chart.Series.Count && i < SelectedPalette.Colors.Count; i++)
        {
            var series = Chart.Series[i];
            var color = SelectedPalette.GetColor(i);
            Chart.SetSeriesStyle(series, new BarSeriesStyle
            {
                FillColor = color,
                CornerRadius = 4f,
                BarWidthRatio = 0.8
            });
        }

        this.RaisePropertyChanged(nameof(Chart));
    }
}
