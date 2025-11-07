namespace SkiaCharts.Core.Animation;

/// <summary>
/// Runs multiple animations sequentially (one after another).
/// </summary>
public class AnimationSequence
{
    private readonly List<IAnimationItem> _animations = new();
    private AnimationState _state = AnimationState.NotStarted;
    private int _currentIndex = 0;

    /// <summary>
    /// Gets the current state of the sequence.
    /// </summary>
    public AnimationState State => _state;

    /// <summary>
    /// Gets the current animation index.
    /// </summary>
    public int CurrentIndex => _currentIndex;

    /// <summary>
    /// Event raised when all animations complete.
    /// </summary>
    public event EventHandler? Completed;

    /// <summary>
    /// Adds an animation to the sequence.
    /// </summary>
    public void Add<T>(Animation<T> animation)
    {
        _animations.Add(new AnimationWrapper<T>(animation));
    }

    /// <summary>
    /// Starts the sequence.
    /// </summary>
    public void Start()
    {
        if (_animations.Count == 0)
        {
            return;
        }

        _currentIndex = 0;
        _animations[_currentIndex].Start();
        _state = AnimationState.Running;
    }

    /// <summary>
    /// Updates the current animation in the sequence.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since last update.</param>
    /// <returns>True if the sequence is still running; otherwise, false.</returns>
    public bool Update(double deltaTime)
    {
        if (_state != AnimationState.Running || _animations.Count == 0)
        {
            return false;
        }

        // Update current animation
        bool isRunning = _animations[_currentIndex].Update(deltaTime);

        if (!isRunning)
        {
            // Current animation completed, move to next
            _currentIndex++;

            if (_currentIndex < _animations.Count)
            {
                // Start next animation
                _animations[_currentIndex].Start();
                return true;
            }
            else
            {
                // All animations completed
                _state = AnimationState.Completed;
                Completed?.Invoke(this, EventArgs.Empty);
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Stops the sequence.
    /// </summary>
    public void Stop()
    {
        if (_currentIndex < _animations.Count)
        {
            _animations[_currentIndex].Stop();
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
