using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SkiaCharts.Gallery.Views;

/// <summary>
/// Base class for demo page views.
/// </summary>
public abstract class DemoPageBase : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DemoPageBase"/> class.
    /// </summary>
    protected DemoPageBase()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Initializes the component. Override to customize.
    /// </summary>
    protected virtual void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Gets the demo title.
    /// </summary>
    public abstract string DemoTitle { get; }

    /// <summary>
    /// Gets the demo description.
    /// </summary>
    public abstract string DemoDescription { get; }
}
