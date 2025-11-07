using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Performance;

/// <summary>
/// Provides virtual scrolling and lazy data loading for large datasets.
/// Only loads data that is currently visible in the viewport.
/// </summary>
public class VirtualDataProvider<T> where T : IDataPoint
{
    private readonly IDataProvider<T> _dataProvider;
    private readonly int _pageSize;
    private readonly Dictionary<int, Page<T>> _cache;
    private int _totalCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="VirtualDataProvider{T}"/> class.
    /// </summary>
    /// <param name="dataProvider">The underlying data provider.</param>
    /// <param name="pageSize">Size of each data page (default: 1000).</param>
    public VirtualDataProvider(IDataProvider<T> dataProvider, int pageSize = 1000)
    {
        _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
        _pageSize = pageSize;
        _cache = new Dictionary<int, Page<T>>();
        _totalCount = dataProvider.GetTotalCount();
    }

    /// <summary>
    /// Gets the total number of data points.
    /// </summary>
    public int TotalCount => _totalCount;

    /// <summary>
    /// Gets the page size.
    /// </summary>
    public int PageSize => _pageSize;

    /// <summary>
    /// Gets the number of cached pages.
    /// </summary>
    public int CachedPageCount => _cache.Count;

    /// <summary>
    /// Gets data points within the specified range.
    /// Uses caching and lazy loading to minimize memory usage.
    /// </summary>
    /// <param name="startIndex">Start index (inclusive).</param>
    /// <param name="endIndex">End index (exclusive).</param>
    /// <returns>Data points within the range.</returns>
    public List<T> GetRange(int startIndex, int endIndex)
    {
        if (startIndex < 0 || endIndex > _totalCount || startIndex >= endIndex)
            return new List<T>();

        var result = new List<T>();
        int startPage = startIndex / _pageSize;
        int endPage = (endIndex - 1) / _pageSize;

        for (int pageIndex = startPage; pageIndex <= endPage; pageIndex++)
        {
            var page = GetPage(pageIndex);

            int pageStartIndex = Math.Max(0, startIndex - pageIndex * _pageSize);
            int pageEndIndex = Math.Min(_pageSize, endIndex - pageIndex * _pageSize);

            for (int i = pageStartIndex; i < pageEndIndex && i < page.Data.Count; i++)
            {
                result.Add(page.Data[i]);
            }
        }

        return result;
    }

    /// <summary>
    /// Gets data points within a specific X range (for viewport queries).
    /// </summary>
    /// <param name="minX">Minimum X value.</param>
    /// <param name="maxX">Maximum X value.</param>
    /// <returns>Data points within the X range.</returns>
    public async Task<List<T>> GetRangeByXAsync(double minX, double maxX)
    {
        // Find approximate index range using binary search
        int startIndex = await FindIndexByXAsync(minX);
        int endIndex = await FindIndexByXAsync(maxX) + 1;

        return GetRange(startIndex, Math.Min(endIndex, _totalCount));
    }

    /// <summary>
    /// Finds the approximate index for a given X value.
    /// </summary>
    private async Task<int> FindIndexByXAsync(double x)
    {
        // Simple estimation - assumes uniform X distribution
        // For production, implement proper binary search with async page loading
        var firstPage = GetPage(0);
        var lastPage = GetPage((_totalCount - 1) / _pageSize);

        if (firstPage.Data.Count == 0 || lastPage.Data.Count == 0)
            return 0;

        double minX = firstPage.Data[0].X;
        double maxX = lastPage.Data[lastPage.Data.Count - 1].X;

        if (x <= minX) return 0;
        if (x >= maxX) return _totalCount - 1;

        // Linear interpolation estimate
        double ratio = (x - minX) / (maxX - minX);
        return (int)(ratio * _totalCount);
    }

    /// <summary>
    /// Gets a page of data, loading from cache or provider.
    /// </summary>
    private Page<T> GetPage(int pageIndex)
    {
        if (_cache.TryGetValue(pageIndex, out var page))
        {
            page.LastAccessTime = DateTime.Now;
            return page;
        }

        // Load page from provider
        int startIndex = pageIndex * _pageSize;
        int count = Math.Min(_pageSize, _totalCount - startIndex);

        var data = _dataProvider.GetData(startIndex, count);
        page = new Page<T>
        {
            Index = pageIndex,
            Data = data,
            LastAccessTime = DateTime.Now
        };

        _cache[pageIndex] = page;

        // Cleanup old pages if cache is too large
        if (_cache.Count > 10)
        {
            CleanupCache();
        }

        return page;
    }

    /// <summary>
    /// Removes least recently used pages from cache.
    /// </summary>
    private void CleanupCache()
    {
        var sorted = _cache.OrderBy(kvp => kvp.Value.LastAccessTime).ToList();
        int removeCount = _cache.Count - 10;

        for (int i = 0; i < removeCount; i++)
        {
            _cache.Remove(sorted[i].Key);
        }
    }

    /// <summary>
    /// Clears all cached data.
    /// </summary>
    public void ClearCache()
    {
        _cache.Clear();
    }

    /// <summary>
    /// Preloads pages for a given range to improve performance.
    /// </summary>
    /// <param name="startIndex">Start index.</param>
    /// <param name="endIndex">End index.</param>
    public void PreloadRange(int startIndex, int endIndex)
    {
        int startPage = startIndex / _pageSize;
        int endPage = (endIndex - 1) / _pageSize;

        for (int pageIndex = startPage; pageIndex <= endPage; pageIndex++)
        {
            if (!_cache.ContainsKey(pageIndex))
            {
                GetPage(pageIndex);
            }
        }
    }

    /// <summary>
    /// Gets cache statistics.
    /// </summary>
    public CacheStatistics GetStatistics()
    {
        int totalPages = (_totalCount + _pageSize - 1) / _pageSize;
        return new CacheStatistics
        {
            TotalPages = totalPages,
            CachedPages = _cache.Count,
            CacheHitRate = _cache.Count / (double)totalPages,
            MemoryEstimateMB = (_cache.Count * _pageSize * 16) / (1024.0 * 1024.0) // Rough estimate
        };
    }

    private class Page<TData>
    {
        public int Index { get; set; }
        public List<TData> Data { get; set; } = new();
        public DateTime LastAccessTime { get; set; }
    }
}

/// <summary>
/// Interface for providing data to the virtual data provider.
/// </summary>
public interface IDataProvider<T> where T : IDataPoint
{
    /// <summary>
    /// Gets the total number of data points available.
    /// </summary>
    int GetTotalCount();

    /// <summary>
    /// Gets a range of data points.
    /// </summary>
    /// <param name="startIndex">Start index (inclusive).</param>
    /// <param name="count">Number of points to retrieve.</param>
    List<T> GetData(int startIndex, int count);
}

/// <summary>
/// Simple in-memory data provider for testing.
/// </summary>
public class InMemoryDataProvider<T> : IDataProvider<T> where T : IDataPoint
{
    private readonly IReadOnlyList<T> _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryDataProvider{T}"/> class.
    /// </summary>
    public InMemoryDataProvider(IReadOnlyList<T> data)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
    }

    /// <inheritdoc/>
    public int GetTotalCount() => _data.Count;

    /// <inheritdoc/>
    public List<T> GetData(int startIndex, int count)
    {
        if (startIndex < 0 || startIndex >= _data.Count)
            return new List<T>();

        int actualCount = Math.Min(count, _data.Count - startIndex);
        return _data.Skip(startIndex).Take(actualCount).ToList();
    }
}

/// <summary>
/// Cache statistics for virtual data provider.
/// </summary>
public class CacheStatistics
{
    /// <summary>Gets the total number of pages.</summary>
    public int TotalPages { get; init; }

    /// <summary>Gets the number of cached pages.</summary>
    public int CachedPages { get; init; }

    /// <summary>Gets the cache hit rate.</summary>
    public double CacheHitRate { get; init; }

    /// <summary>Gets the estimated memory usage in MB.</summary>
    public double MemoryEstimateMB { get; init; }
}
