using System.Diagnostics;

namespace SkiaCharts.Core.Animation;

/// <summary>
/// Manages and coordinates multiple animations.
/// </summary>
public class AnimationController
{
    private readonly List<IAnimationUpdatable> _animations = new();
    private readonly Stopwatch _stopwatch = new();
    private double _lastTime;
    private bool _isRunning;

    /// <summary>
    /// Gets a value indicating whether the controller is currently running.
    /// </summary>
    public bool IsRunning => _isRunning;

    /// <summary>
    /// Gets the current frame rate (FPS).
    /// </summary>
    public double FrameRate { get; private set; }

    /// <summary>
    /// Event raised when all animations have completed.
    /// </summary>
    public event EventHandler? AllAnimationsCompleted;

    /// <summary>
    /// Adds an animation to the controller.
    /// </summary>
    /// <typeparam name="T">The type of value being animated.</typeparam>
    /// <param name="animation">The animation to add.</param>
    public void Add<T>(Animation<T> animation)
    {
        _animations.Add(new AnimationWrapper<T>(animation));
    }

    /// <summary>
    /// Removes an animation from the controller.
    /// </summary>
    /// <typeparam name="T">The type of value being animated.</typeparam>
    /// <param name="animation">The animation to remove.</param>
    public void Remove<T>(Animation<T> animation)
    {
        var wrapper = _animations.OfType<AnimationWrapper<T>>()
            .FirstOrDefault(w => w.Animation == animation);
        if (wrapper != null)
        {
            _animations.Remove(wrapper);
        }
    }

    /// <summary>
    /// Removes all animations.
    /// </summary>
    public void Clear()
    {
        _animations.Clear();
    }

    /// <summary>
    /// Starts all animations.
    /// </summary>
    public void StartAll()
    {
        foreach (var animation in _animations)
        {
            animation.Start();
        }

        if (!_isRunning)
        {
            _isRunning = true;
            _stopwatch.Restart();
            _lastTime = 0;
        }
    }

    /// <summary>
    /// Pauses all animations.
    /// </summary>
    public void PauseAll()
    {
        foreach (var animation in _animations)
        {
            animation.Pause();
        }
        _isRunning = false;
        _stopwatch.Stop();
    }

    /// <summary>
    /// Resumes all animations.
    /// </summary>
    public void ResumeAll()
    {
        foreach (var animation in _animations)
        {
            animation.Resume();
        }
        _isRunning = true;
        _stopwatch.Start();
    }

    /// <summary>
    /// Stops all animations.
    /// </summary>
    public void StopAll()
    {
        foreach (var animation in _animations)
        {
            animation.Stop();
        }
        _isRunning = false;
        _stopwatch.Stop();
    }

    /// <summary>
    /// Updates all animations. Call this from your render loop.
    /// </summary>
    /// <returns>True if any animations are still running; otherwise, false.</returns>
    public bool Update()
    {
        if (!_isRunning)
        {
            return false;
        }

        // Calculate delta time
        var currentTime = _stopwatch.Elapsed.TotalSeconds;
        var deltaTime = currentTime - _lastTime;
        _lastTime = currentTime;

        // Calculate FPS
        FrameRate = deltaTime > 0 ? 1.0 / deltaTime : 0;

        // Update all animations
        var anyRunning = false;
        for (int i = _animations.Count - 1; i >= 0; i--)
        {
            var animation = _animations[i];
            var isRunning = animation.Update(deltaTime);

            if (!isRunning)
            {
                // Animation completed, remove it
                _animations.RemoveAt(i);
            }
            else
            {
                anyRunning = true;
            }
        }

        // If no animations are running, raise completion event
        if (!anyRunning && _animations.Count == 0)
        {
            _isRunning = false;
            _stopwatch.Stop();
            AllAnimationsCompleted?.Invoke(this, EventArgs.Empty);
            return false;
        }

        return anyRunning;
    }

    /// <summary>
    /// Gets the count of active animations.
    /// </summary>
    public int Count => _animations.Count;

    private interface IAnimationUpdatable
    {
        void Start();
        void Pause();
        void Resume();
        void Stop();
        bool Update(double deltaTime);
    }

    private class AnimationWrapper<T> : IAnimationUpdatable
    {
        public AnimationWrapper(Animation<T> animation)
        {
            Animation = animation;
        }

        public Animation<T> Animation { get; }

        public void Start() => Animation.Start();
        public void Pause() => Animation.Pause();
        public void Resume() => Animation.Resume();
        public void Stop() => Animation.Stop();
        public bool Update(double deltaTime) => Animation.Update(deltaTime);
    }
}
