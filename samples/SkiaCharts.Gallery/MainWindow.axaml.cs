using Avalonia.Controls;
using SkiaCharts.Gallery.ViewModels;

namespace SkiaCharts.Gallery;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}