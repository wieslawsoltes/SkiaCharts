namespace SkiaCharts.Core.Animation;

/// <summary>
/// Runs multiple animations in parallel.
/// </summary>
public class AnimationGroup
{
    private readonly List<IAnimationItem> _animations = new();
    private AnimationState _state = AnimationState.NotStarted;

    /// <summary>
    /// Gets the current state of the group.
    /// </summary>
    public AnimationState State => _state;

    /// <summary>
    /// Event raised when all animations complete.
    /// </summary>
    public event EventHandler? Completed;

    /// <summary>
    /// Adds an animation to the group.
    /// </summary>
    public void Add<T>(Animation<T> animation)
    {
        _animations.Add(new AnimationWrapper<T>(animation));
    }

    /// <summary>
    /// Starts all animations in parallel.
    /// </summary>
    public void Start()
    {
        foreach (var animation in _animations)
        {
            animation.Start();
        }
        _state = AnimationState.Running;
    }

    /// <summary>
    /// Updates all animations.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since last update.</param>
    /// <returns>True if any animation is still running; otherwise, false.</returns>
    public bool Update(double deltaTime)
    {
        if (_state != AnimationState.Running)
        {
            return false;
        }

        bool anyRunning = false;
        foreach (var animation in _animations)
        {
            if (animation.Update(deltaTime))
            {
                anyRunning = true;
            }
        }

        if (!anyRunning)
        {
            _state = AnimationState.Completed;
            Completed?.Invoke(this, EventArgs.Empty);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Stops all animations.
    /// </summary>
    public void Stop()
    {
        foreach (var animation in _animations)
        {
            animation.Stop();
        }
        _state = AnimationState.Cancelled;
    }

    private interface IAnimationItem
    {
        void Start();
        bool Update(double deltaTime);
        void Stop();
    }

    private class AnimationWrapper<T> : IAnimationItem
    {
        private readonly Animation<T> _animation;

        public AnimationWrapper(Animation<T> animation)
        {
            _animation = animation;
        }

        public void Start() => _animation.Start();
        public bool Update(double deltaTime) => _animation.Update(deltaTime);
        public void Stop() => _animation.Stop();
    }
}
