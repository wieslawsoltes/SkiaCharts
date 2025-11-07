using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ReactiveUI;

namespace SkiaCharts.Gallery;

/// <summary>
/// Locates views for view models using naming conventions.
/// </summary>
public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
            return null;

        var name = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null)
        {
            var control = (Control)Activator.CreateInstance(type)!;
            control.DataContext = data;
            return control;
        }

        return new TextBlock { Text = $"View not found: {name}" };
    }

    public bool Match(object? data)
    {
        return data is ReactiveObject;
    }
}
