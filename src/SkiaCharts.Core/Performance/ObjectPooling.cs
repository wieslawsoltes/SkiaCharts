using SkiaSharp;
using System.Collections.Concurrent;

namespace SkiaCharts.Core.Performance;

/// <summary>
/// Provides object pooling for SkiaSharp objects to reduce GC pressure and allocation overhead.
/// </summary>
public static class ObjectPooling
{
    private static readonly SKPathPool _pathPool = new();
    private static readonly SKPaintPool _paintPool = new();

    /// <summary>
    /// Gets the shared SKPath pool instance.
    /// </summary>
    public static SKPathPool PathPool => _pathPool;

    /// <summary>
    /// Gets the shared SKPaint pool instance.
    /// </summary>
    public static SKPaintPool PaintPool => _paintPool;
}

/// <summary>
/// Object pool for SKPath instances.
/// </summary>
public class SKPathPool
{
    private readonly ConcurrentBag<SKPath> _pool = new();
    private int _totalCreated;
    private int _currentPooled;

    /// <summary>
    /// Gets the maximum number of pooled objects to keep.
    /// </summary>
    public int MaxPoolSize { get; set; } = 100;

    /// <summary>
    /// Gets the total number of SKPath objects created.
    /// </summary>
    public int TotalCreated => _totalCreated;

    /// <summary>
    /// Gets the current number of pooled objects.
    /// </summary>
    public int CurrentPooled => _currentPooled;

    /// <summary>
    /// Rents an SKPath from the pool or creates a new one if pool is empty.
    /// </summary>
    /// <returns>An SKPath instance ready for use.</returns>
    public SKPath Rent()
    {
        if (_pool.TryTake(out var path))
        {
            Interlocked.Decrement(ref _currentPooled);
            path.Rewind(); // Reset the path
            return path;
        }

        Interlocked.Increment(ref _totalCreated);
        return new SKPath();
    }

    /// <summary>
    /// Returns an SKPath to the pool for reuse.
    /// </summary>
    /// <param name="path">The path to return.</param>
    public void Return(SKPath path)
    {
        if (path == null)
            return;

        // Only pool if we haven't exceeded max size
        if (_currentPooled < MaxPoolSize)
        {
            path.Rewind(); // Clear the path
            _pool.Add(path);
            Interlocked.Increment(ref _currentPooled);
        }
        else
        {
            path.Dispose();
        }
    }

    /// <summary>
    /// Clears the pool and disposes all pooled objects.
    /// </summary>
    public void Clear()
    {
        while (_pool.TryTake(out var path))
        {
            path.Dispose();
            Interlocked.Decrement(ref _currentPooled);
        }
    }

    /// <summary>
    /// Gets pool statistics.
    /// </summary>
    /// <returns>A tuple containing total created, currently pooled, and hit rate.</returns>
    public (int TotalCreated, int CurrentPooled, double HitRate) GetStatistics()
    {
        double hitRate = _totalCreated > 0
            ? 1.0 - ((double)_totalCreated / (_totalCreated + _currentPooled))
            : 0.0;

        return (_totalCreated, _currentPooled, hitRate);
    }
}

/// <summary>
/// Object pool for SKPaint instances.
/// </summary>
public class SKPaintPool
{
    private readonly ConcurrentBag<SKPaint> _pool = new();
    private int _totalCreated;
    private int _currentPooled;

    /// <summary>
    /// Gets the maximum number of pooled objects to keep.
    /// </summary>
    public int MaxPoolSize { get; set; } = 100;

    /// <summary>
    /// Gets the total number of SKPaint objects created.
    /// </summary>
    public int TotalCreated => _totalCreated;

    /// <summary>
    /// Gets the current number of pooled objects.
    /// </summary>
    public int CurrentPooled => _currentPooled;

    /// <summary>
    /// Rents an SKPaint from the pool or creates a new one if pool is empty.
    /// </summary>
    /// <returns>An SKPaint instance ready for use.</returns>
    public SKPaint Rent()
    {
        if (_pool.TryTake(out var paint))
        {
            Interlocked.Decrement(ref _currentPooled);
            ResetPaint(paint);
            return paint;
        }

        Interlocked.Increment(ref _totalCreated);
        return new SKPaint();
    }

    /// <summary>
    /// Returns an SKPaint to the pool for reuse.
    /// </summary>
    /// <param name="paint">The paint to return.</param>
    public void Return(SKPaint paint)
    {
        if (paint == null)
            return;

        // Only pool if we haven't exceeded max size
        if (_currentPooled < MaxPoolSize)
        {
            ResetPaint(paint);
            _pool.Add(paint);
            Interlocked.Increment(ref _currentPooled);
        }
        else
        {
            paint.Dispose();
        }
    }

    /// <summary>
    /// Clears the pool and disposes all pooled objects.
    /// </summary>
    public void Clear()
    {
        while (_pool.TryTake(out var paint))
        {
            paint.Dispose();
            Interlocked.Decrement(ref _currentPooled);
        }
    }

    /// <summary>
    /// Resets a paint object to default state.
    /// </summary>
    private static void ResetPaint(SKPaint paint)
    {
        paint.Reset();
        paint.IsAntialias = true; // Common default
    }

    /// <summary>
    /// Gets pool statistics.
    /// </summary>
    /// <returns>A tuple containing total created, currently pooled, and hit rate.</returns>
    public (int TotalCreated, int CurrentPooled, double HitRate) GetStatistics()
    {
        double hitRate = _totalCreated > 0
            ? 1.0 - ((double)_totalCreated / (_totalCreated + _currentPooled))
            : 0.0;

        return (_totalCreated, _currentPooled, hitRate);
    }
}

/// <summary>
/// Helper struct for using pooled objects with automatic return via IDisposable.
/// </summary>
/// <typeparam name="T">The type of pooled object.</typeparam>
public readonly struct PooledObject<T> : IDisposable where T : class, IDisposable
{
    private readonly T _obj;
    private readonly Action<T> _returnAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="PooledObject{T}"/> struct.
    /// </summary>
    /// <param name="obj">The pooled object.</param>
    /// <param name="returnAction">Action to return the object to the pool.</param>
    public PooledObject(T obj, Action<T> returnAction)
    {
        _obj = obj;
        _returnAction = returnAction;
    }

    /// <summary>
    /// Gets the pooled object.
    /// </summary>
    public T Object => _obj;

    /// <summary>
    /// Returns the object to the pool.
    /// </summary>
    public void Dispose()
    {
        _returnAction?.Invoke(_obj);
    }

    public static implicit operator T(PooledObject<T> pooled) => pooled.Object;
}

/// <summary>
/// Extension methods for convenient pooling usage.
/// </summary>
public static class PoolingExtensions
{
    /// <summary>
    /// Rents an SKPath from the global pool with automatic return on dispose.
    /// </summary>
    /// <returns>A pooled path wrapper.</returns>
    public static PooledObject<SKPath> RentPath()
    {
        var path = ObjectPooling.PathPool.Rent();
        return new PooledObject<SKPath>(path, ObjectPooling.PathPool.Return);
    }

    /// <summary>
    /// Rents an SKPaint from the global pool with automatic return on dispose.
    /// </summary>
    /// <returns>A pooled paint wrapper.</returns>
    public static PooledObject<SKPaint> RentPaint()
    {
        var paint = ObjectPooling.PaintPool.Rent();
        return new PooledObject<SKPaint>(paint, ObjectPooling.PaintPool.Return);
    }
}
