using System;
using ReactiveUI;
using SkiaCharts.Gallery.Models;

namespace SkiaCharts.Gallery.Services;

/// <summary>
/// Service for managing navigation within the application.
/// </summary>
public class NavigationService : ReactiveObject
{
    private DemoPage? _currentDemo;
    private object? _currentViewModel;

    /// <summary>
    /// Gets or sets the currently selected demo.
    /// </summary>
    public DemoPage? CurrentDemo
    {
        get => _currentDemo;
        set => this.RaiseAndSetIfChanged(ref _currentDemo, value);
    }

    /// <summary>
    /// Gets or sets the current view model instance.
    /// </summary>
    public object? CurrentViewModel
    {
        get => _currentViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    /// <summary>
    /// Navigates to a specific demo.
    /// </summary>
    public void NavigateToDemo(DemoPage demo)
    {
        if (demo == null)
            throw new ArgumentNullException(nameof(demo));

        CurrentDemo = demo;

        // Create view model instance if type is specified
        if (demo.ViewModelType != null)
        {
            CurrentViewModel = Activator.CreateInstance(demo.ViewModelType);
        }
        else
        {
            CurrentViewModel = null;
        }
    }

    /// <summary>
    /// Navigates back to the home page.
    /// </summary>
    public void NavigateToHome()
    {
        CurrentDemo = null;
        CurrentViewModel = null;
    }
}
