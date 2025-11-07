using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using SkiaCharts.Gallery.Models;
using SkiaCharts.Gallery.Services;

namespace SkiaCharts.Gallery.ViewModels;

/// <summary>
/// View model for the main application window.
/// </summary>
public class MainWindowViewModel : ReactiveObject
{
    private readonly NavigationService _navigationService;
    private string _searchQuery = string.Empty;
    private DemoCategory? _selectedCategory;
    private DemoPage? _selectedDemo;
    private bool _isSidebarOpen = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
    {
        _navigationService = new NavigationService();

        // Subscribe to navigation changes
        _navigationService.WhenAnyValue(x => x.CurrentDemo)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(demo => SelectedDemo = demo);

        // Subscribe to view model changes
        _navigationService.WhenAnyValue(x => x.CurrentViewModel)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentViewModel)));

        // Commands
        NavigateToHomeCommand = ReactiveCommand.Create(NavigateToHome, outputScheduler: RxApp.MainThreadScheduler);
        NavigateToDemoCommand = ReactiveCommand.Create<DemoPage>(NavigateToDemo, outputScheduler: RxApp.MainThreadScheduler);
        SelectCategoryCommand = ReactiveCommand.Create<DemoCategory>(SelectCategory, outputScheduler: RxApp.MainThreadScheduler);
        ToggleSidebarCommand = ReactiveCommand.Create(ToggleSidebar, outputScheduler: RxApp.MainThreadScheduler);
        SearchCommand = ReactiveCommand.Create(PerformSearch, outputScheduler: RxApp.MainThreadScheduler);

        // Initialize with all demos - defer to ensure UI thread is ready
        System.Reactive.Linq.Observable.Start(PerformSearch, RxApp.MainThreadScheduler);
    }

    #region Properties

    /// <summary>
    /// Gets the list of all categories.
    /// </summary>
    public List<DemoCategory> Categories => DemoCatalog.Categories;

    /// <summary>
    /// Gets or sets the search query.
    /// </summary>
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            this.RaiseAndSetIfChanged(ref _searchQuery, value);
            PerformSearch();
        }
    }

    /// <summary>
    /// Gets or sets the selected category.
    /// </summary>
    public DemoCategory? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedCategory, value);
            FilterDemos();
        }
    }

    /// <summary>
    /// Gets or sets the selected demo.
    /// </summary>
    public DemoPage? SelectedDemo
    {
        get => _selectedDemo;
        set => this.RaiseAndSetIfChanged(ref _selectedDemo, value);
    }

    /// <summary>
    /// Gets the filtered list of demos based on search and category.
    /// </summary>
    public ObservableCollection<DemoPage> FilteredDemos { get; } = new();

    /// <summary>
    /// Gets or sets whether the sidebar is open.
    /// </summary>
    public bool IsSidebarOpen
    {
        get => _isSidebarOpen;
        set => this.RaiseAndSetIfChanged(ref _isSidebarOpen, value);
    }

    /// <summary>
    /// Gets the navigation service.
    /// </summary>
    public NavigationService NavigationService => _navigationService;

    /// <summary>
    /// Gets the current view model for the selected demo.
    /// </summary>
    public object? CurrentViewModel => _navigationService.CurrentViewModel;

    /// <summary>
    /// Gets the total number of demos.
    /// </summary>
    public int TotalDemoCount => DemoCatalog.AllDemos.Count();

    /// <summary>
    /// Gets the gallery version.
    /// </summary>
    public string Version => "v1.0.0";

    #endregion

    #region Commands

    /// <summary>
    /// Gets the command to navigate to home.
    /// </summary>
    public ReactiveCommand<Unit, Unit> NavigateToHomeCommand { get; }

    /// <summary>
    /// Gets the command to navigate to a demo.
    /// </summary>
    public ReactiveCommand<DemoPage, Unit> NavigateToDemoCommand { get; }

    /// <summary>
    /// Gets the command to select a category.
    /// </summary>
    public ReactiveCommand<DemoCategory, Unit> SelectCategoryCommand { get; }

    /// <summary>
    /// Gets the command to toggle the sidebar.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ToggleSidebarCommand { get; }

    /// <summary>
    /// Gets the command to perform a search.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SearchCommand { get; }

    #endregion

    #region Methods

    private void NavigateToHome()
    {
        _navigationService.NavigateToHome();
        SelectedCategory = null;
    }

    private void NavigateToDemo(DemoPage demo)
    {
        _navigationService.NavigateToDemo(demo);
    }

    private void SelectCategory(DemoCategory category)
    {
        SelectedCategory = category;
    }

    private void ToggleSidebar()
    {
        IsSidebarOpen = !IsSidebarOpen;
    }

    private void PerformSearch()
    {
        FilterDemos();
    }

    private void FilterDemos()
    {
        FilteredDemos.Clear();

        IEnumerable<DemoPage> demos;

        // Filter by category first
        if (SelectedCategory != null)
        {
            demos = SelectedCategory.Demos;
        }
        else
        {
            demos = DemoCatalog.AllDemos;
        }

        // Then filter by search query
        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var query = SearchQuery.ToLowerInvariant();
            demos = demos.Where(d =>
                d.Title.ToLowerInvariant().Contains(query) ||
                d.Description.ToLowerInvariant().Contains(query) ||
                d.Tags.Any(tag => tag.ToLowerInvariant().Contains(query)));
        }

        foreach (var demo in demos)
        {
            FilteredDemos.Add(demo);
        }
    }

    #endregion
}
