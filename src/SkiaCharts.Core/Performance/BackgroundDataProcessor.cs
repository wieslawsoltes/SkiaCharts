using SkiaCharts.Core.Data;
using System.Collections.Concurrent;

namespace SkiaCharts.Core.Performance;

/// <summary>
/// Provides background data processing capabilities for large datasets.
/// Offloads heavy computation (aggregation, downsampling) to background threads.
/// </summary>
public class BackgroundDataProcessor<T> where T : IDataPoint
{
    private readonly ConcurrentQueue<ProcessingTask<T>> _taskQueue;
    private readonly SemaphoreSlim _semaphore;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private Task? _processingTask;
    private bool _isRunning;

    /// <summary>
    /// Initializes a new instance of the <see cref="BackgroundDataProcessor{T}"/> class.
    /// </summary>
    public BackgroundDataProcessor()
    {
        _taskQueue = new ConcurrentQueue<ProcessingTask<T>>();
        _semaphore = new SemaphoreSlim(0);
        _cancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Gets whether the processor is currently running.
    /// </summary>
    public bool IsRunning => _isRunning;

    /// <summary>
    /// Gets the number of pending tasks.
    /// </summary>
    public int PendingTaskCount => _taskQueue.Count;

    /// <summary>
    /// Event raised when a processing task completes.
    /// </summary>
    public event EventHandler<ProcessingCompletedEventArgs<T>>? ProcessingCompleted;

    /// <summary>
    /// Starts the background processor.
    /// </summary>
    public void Start()
    {
        if (_isRunning)
            return;

        _isRunning = true;
        _processingTask = Task.Run(ProcessQueueAsync);
    }

    /// <summary>
    /// Stops the background processor.
    /// </summary>
    public async Task StopAsync()
    {
        if (!_isRunning)
            return;

        _isRunning = false;
        _cancellationTokenSource.Cancel();

        if (_processingTask != null)
        {
            await _processingTask;
        }
    }

    /// <summary>
    /// Queues data for downsampling using LTTB algorithm.
    /// </summary>
    public void QueueLttbDownsampling(
        IReadOnlyList<T> data,
        int threshold,
        object? userState = null)
    {
        var task = new ProcessingTask<T>
        {
            Type = ProcessingType.LttbDownsampling,
            Data = data,
            Threshold = threshold,
            UserState = userState
        };

        _taskQueue.Enqueue(task);
        _semaphore.Release();
    }

    /// <summary>
    /// Queues data for aggregation.
    /// </summary>
    public void QueueAggregation(
        IReadOnlyList<T> data,
        int binCount,
        AggregationMethod method = AggregationMethod.Average,
        object? userState = null)
    {
        var task = new ProcessingTask<T>
        {
            Type = ProcessingType.Aggregation,
            Data = data,
            BinCount = binCount,
            AggregationMethod = method,
            UserState = userState
        };

        _taskQueue.Enqueue(task);
        _semaphore.Release();
    }

    /// <summary>
    /// Queues data for path simplification using Douglas-Peucker algorithm.
    /// </summary>
    public void QueuePathSimplification(
        IReadOnlyList<T> data,
        double tolerance,
        object? userState = null)
    {
        var task = new ProcessingTask<T>
        {
            Type = ProcessingType.PathSimplification,
            Data = data,
            Tolerance = tolerance,
            UserState = userState
        };

        _taskQueue.Enqueue(task);
        _semaphore.Release();
    }

    /// <summary>
    /// Queues custom processing function.
    /// </summary>
    public void QueueCustomProcessing(
        IReadOnlyList<T> data,
        Func<IReadOnlyList<T>, List<DataPoint>> processor,
        object? userState = null)
    {
        var task = new ProcessingTask<T>
        {
            Type = ProcessingType.Custom,
            Data = data,
            CustomProcessor = processor,
            UserState = userState
        };

        _taskQueue.Enqueue(task);
        _semaphore.Release();
    }

    /// <summary>
    /// Background task processing loop.
    /// </summary>
    private async Task ProcessQueueAsync()
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                await _semaphore.WaitAsync(_cancellationTokenSource.Token);

                if (_taskQueue.TryDequeue(out var task))
                {
                    var result = await ProcessTaskAsync(task);

                    ProcessingCompleted?.Invoke(this, new ProcessingCompletedEventArgs<T>
                    {
                        Result = result,
                        UserState = task.UserState,
                        ProcessingType = task.Type
                    });
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                // Log error but continue processing
                ProcessingCompleted?.Invoke(this, new ProcessingCompletedEventArgs<T>
                {
                    Error = ex
                });
            }
        }
    }

    /// <summary>
    /// Processes a single task.
    /// </summary>
    private Task<List<DataPoint>> ProcessTaskAsync(ProcessingTask<T> task)
    {
        return Task.Run(() =>
        {
            List<DataPoint> result = task.Type switch
            {
                ProcessingType.LttbDownsampling =>
                    LargestTriangleThreeBuckets.Downsample(task.Data, task.Threshold),

                ProcessingType.Aggregation =>
                    DataAggregation.Aggregate(task.Data, task.BinCount, task.AggregationMethod),

                ProcessingType.PathSimplification =>
                    PathSimplification.DouglasPeucker(task.Data, task.Tolerance).Select(p => new DataPoint(p.X, p.Y)).ToList(),

                ProcessingType.Custom =>
                    task.CustomProcessor?.Invoke(task.Data) ?? new List<DataPoint>(),

                _ => new List<DataPoint>()
            };
            return result;
        });
    }

    /// <summary>
    /// Clears all pending tasks.
    /// </summary>
    public void ClearQueue()
    {
        while (_taskQueue.TryDequeue(out _))
        {
            // Drain the queue
        }
    }

    private class ProcessingTask<TData> where TData : IDataPoint
    {
        public ProcessingType Type { get; set; }
        public IReadOnlyList<TData> Data { get; set; } = Array.Empty<TData>();
        public int Threshold { get; set; }
        public int BinCount { get; set; }
        public AggregationMethod AggregationMethod { get; set; }
        public double Tolerance { get; set; }
        public Func<IReadOnlyList<TData>, List<DataPoint>>? CustomProcessor { get; set; }
        public object? UserState { get; set; }
    }
}

/// <summary>
/// Types of background processing operations.
/// </summary>
public enum ProcessingType
{
    /// <summary>LTTB downsampling.</summary>
    LttbDownsampling,

    /// <summary>Data aggregation/binning.</summary>
    Aggregation,

    /// <summary>Path simplification (Douglas-Peucker).</summary>
    PathSimplification,

    /// <summary>Custom processing function.</summary>
    Custom
}

/// <summary>
/// Event args for processing completion.
/// </summary>
public class ProcessingCompletedEventArgs<T> : EventArgs where T : IDataPoint
{
    /// <summary>Gets the processing result.</summary>
    public List<DataPoint> Result { get; init; } = new();

    /// <summary>Gets the user state object.</summary>
    public object? UserState { get; init; }

    /// <summary>Gets the processing type.</summary>
    public ProcessingType ProcessingType { get; init; }

    /// <summary>Gets the error if processing failed.</summary>
    public Exception? Error { get; init; }

    /// <summary>Gets whether processing was successful.</summary>
    public bool Success => Error == null;
}
