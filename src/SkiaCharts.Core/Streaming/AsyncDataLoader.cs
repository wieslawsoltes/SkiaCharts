using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Streaming;

/// <summary>
/// Provides async loading capabilities for historical data with progress reporting.
/// </summary>
public class AsyncDataLoader
{
    /// <summary>
    /// Event raised when loading progress changes.
    /// </summary>
    public event EventHandler<LoadProgressEventArgs>? ProgressChanged;

    /// <summary>
    /// Event raised when loading completes.
    /// </summary>
    public event EventHandler<LoadCompletedEventArgs>? LoadCompleted;

    /// <summary>
    /// Loads data asynchronously with progress reporting.
    /// </summary>
    /// <param name="dataProvider">Function that provides the data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The loaded data points.</returns>
    public async Task<List<IDataPoint>> LoadAsync(
        Func<IProgress<double>, CancellationToken, Task<List<IDataPoint>>> dataProvider,
        CancellationToken cancellationToken = default)
    {
        var progress = new Progress<double>(p =>
        {
            ProgressChanged?.Invoke(this, new LoadProgressEventArgs(p));
        });

        try
        {
            var data = await Task.Run(async () => await dataProvider(progress, cancellationToken), cancellationToken);
            LoadCompleted?.Invoke(this, new LoadCompletedEventArgs(true, data.Count));
            return data;
        }
        catch (Exception ex)
        {
            LoadCompleted?.Invoke(this, new LoadCompletedEventArgs(false, 0, ex));
            throw;
        }
    }

    /// <summary>
    /// Loads data from a file asynchronously.
    /// </summary>
    /// <param name="filePath">Path to the data file.</param>
    /// <param name="parser">Function to parse a line into a data point.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The loaded data points.</returns>
    public async Task<List<IDataPoint>> LoadFromFileAsync(
        string filePath,
        Func<string, IDataPoint?> parser,
        CancellationToken cancellationToken = default)
    {
        return await LoadAsync(async (progress, ct) =>
        {
            var points = new List<IDataPoint>();

            using var reader = new StreamReader(filePath);

            // Get file length for progress calculation
            var fileLength = new FileInfo(filePath).Length;
            var bytesRead = 0L;

            string? line;
            while ((line = await reader.ReadLineAsync(ct)) != null)
            {
                if (ct.IsCancellationRequested)
                    break;

                var point = parser(line);
                if (point != null)
                {
                    points.Add(point);
                }

                // Update progress
                bytesRead += line.Length + Environment.NewLine.Length;
                var progressPercent = (double)bytesRead / fileLength;
                progress.Report(progressPercent);
            }

            return points;
        }, cancellationToken);
    }

    /// <summary>
    /// Loads data in chunks for very large datasets.
    /// </summary>
    /// <param name="dataProvider">Function that provides data in chunks.</param>
    /// <param name="chunkSize">Size of each chunk.</param>
    /// <param name="onChunkLoaded">Callback invoked for each loaded chunk.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Total number of points loaded.</returns>
    public async Task<int> LoadChunkedAsync(
        Func<int, int, CancellationToken, Task<List<IDataPoint>>> dataProvider,
        int chunkSize,
        Action<List<IDataPoint>> onChunkLoaded,
        CancellationToken cancellationToken = default)
    {
        int offset = 0;
        int totalLoaded = 0;
        bool hasMore = true;

        while (hasMore && !cancellationToken.IsCancellationRequested)
        {
            var chunk = await dataProvider(offset, chunkSize, cancellationToken);

            if (chunk.Count == 0)
            {
                hasMore = false;
            }
            else
            {
                onChunkLoaded(chunk);
                totalLoaded += chunk.Count;
                offset += chunk.Count;

                var progress = offset / (double)(offset + chunkSize); // Approximate
                ProgressChanged?.Invoke(this, new LoadProgressEventArgs(progress));

                if (chunk.Count < chunkSize)
                {
                    hasMore = false;
                }
            }
        }

        LoadCompleted?.Invoke(this, new LoadCompletedEventArgs(!cancellationToken.IsCancellationRequested, totalLoaded));
        return totalLoaded;
    }
}

/// <summary>
/// Event args for load progress updates.
/// </summary>
public class LoadProgressEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadProgressEventArgs"/> class.
    /// </summary>
    /// <param name="progressPercent">Progress as a value between 0 and 1.</param>
    public LoadProgressEventArgs(double progressPercent)
    {
        ProgressPercent = Math.Clamp(progressPercent, 0.0, 1.0);
    }

    /// <summary>
    /// Gets the progress percentage (0.0 to 1.0).
    /// </summary>
    public double ProgressPercent { get; }
}

/// <summary>
/// Event args for load completion.
/// </summary>
public class LoadCompletedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadCompletedEventArgs"/> class.
    /// </summary>
    /// <param name="success">Whether the load was successful.</param>
    /// <param name="pointsLoaded">Number of points loaded.</param>
    /// <param name="exception">Exception if load failed.</param>
    public LoadCompletedEventArgs(bool success, int pointsLoaded, Exception? exception = null)
    {
        Success = success;
        PointsLoaded = pointsLoaded;
        Exception = exception;
    }

    /// <summary>
    /// Gets whether the load was successful.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets the number of points loaded.
    /// </summary>
    public int PointsLoaded { get; }

    /// <summary>
    /// Gets the exception if the load failed.
    /// </summary>
    public Exception? Exception { get; }
}
