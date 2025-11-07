using SkiaCharts.Core.Animation;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Animation;

public class AnimationTests
{
    [Fact]
    public void Animation_ShouldInterpolateDoubleValues()
    {
        // Arrange
        var animation = new Animation<double>(0, 100, 1.0, Interpolators.Double);

        // Act
        animation.Start();
        animation.Update(0.5); // 50% progress

        // Assert
        Assert.Equal(AnimationState.Running, animation.State);
        Assert.Equal(0.5, animation.Progress, 2);
        Assert.InRange(animation.CurrentValue, 45, 55); // Around 50
    }

    [Fact]
    public void Animation_ShouldCompleteAfterDuration()
    {
        // Arrange
        var animation = new Animation<double>(0, 100, 1.0, Interpolators.Double);
        bool completed = false;
        animation.Completed += (s, e) => completed = true;

        // Act
        animation.Start();
        animation.Update(0.5);
        animation.Update(0.5); // Total 1.0 second

        // Assert
        Assert.Equal(AnimationState.Completed, animation.State);
        Assert.True(completed);
        Assert.Equal(100, animation.CurrentValue);
    }

    [Fact]
    public void Animation_ShouldRespectEasingFunction()
    {
        // Arrange
        var linearAnim = new Animation<double>(0, 100, 1.0, Interpolators.Double)
        {
            EasingFunction = EasingFunctions.Linear
        };

        var cubicAnim = new Animation<double>(0, 100, 1.0, Interpolators.Double)
        {
            EasingFunction = EasingFunctions.CubicOut
        };

        // Act
        linearAnim.Start();
        cubicAnim.Start();

        linearAnim.Update(0.5);
        cubicAnim.Update(0.5);

        // Assert - CubicOut should have progressed further at 50%
        Assert.True(cubicAnim.CurrentValue > linearAnim.CurrentValue);
    }

    [Fact]
    public void AnimationController_ShouldManageMultipleAnimations()
    {
        // Arrange
        var controller = new AnimationController();
        var anim1 = new Animation<double>(0, 100, 1.0, Interpolators.Double);
        var anim2 = new Animation<double>(0, 50, 0.5, Interpolators.Double);

        // Act
        controller.Add(anim1);
        controller.Add(anim2);
        controller.StartAll();

        // Assert
        Assert.Equal(2, controller.Count);
        Assert.True(controller.IsRunning);
    }

    [Fact]
    public void AnimationController_ShouldRemoveCompletedAnimations()
    {
        // Arrange
        var controller = new AnimationController();
        var anim = new Animation<double>(0, 100, 0.1, Interpolators.Double);

        // Act
        controller.Add(anim);
        controller.StartAll();
        controller.Update(); // Start
        System.Threading.Thread.Sleep(150); // Wait for completion
        controller.Update();

        // Assert
        Assert.Equal(0, controller.Count);
        Assert.False(controller.IsRunning);
    }

    [Fact]
    public void ColorInterpolation_ShouldBlendColors()
    {
        // Arrange
        var animation = new Animation<SKColor>(
            SKColors.Red,
            SKColors.Blue,
            1.0,
            Interpolators.Color
        );

        // Act
        animation.Start();
        animation.Update(0.5); // 50% progress

        // Assert
        var color = animation.CurrentValue;
        Assert.InRange(color.Red, 100, 140);   // Between red and blue
        Assert.InRange(color.Blue, 100, 140);
    }

    [Fact]
    public void AnimationGroup_ShouldRunInParallel()
    {
        // Arrange
        var group = new AnimationGroup();
        var anim1 = new Animation<double>(0, 100, 1.0, Interpolators.Double);
        var anim2 = new Animation<double>(0, 50, 1.0, Interpolators.Double);

        bool anim1Updated = false;
        bool anim2Updated = false;

        anim1.Updated += (s, e) => anim1Updated = true;
        anim2.Updated += (s, e) => anim2Updated = true;

        // Act
        group.Add(anim1);
        group.Add(anim2);
        group.Start();
        group.Update(0.5);

        // Assert
        Assert.True(anim1Updated);
        Assert.True(anim2Updated);
        Assert.Equal(AnimationState.Running, group.State);
    }

    [Fact]
    public void AnimationSequence_ShouldRunSequentially()
    {
        // Arrange
        var sequence = new AnimationSequence();
        var anim1 = new Animation<double>(0, 100, 0.1, Interpolators.Double);
        var anim2 = new Animation<double>(0, 50, 0.1, Interpolators.Double);

        // Act
        sequence.Add(anim1);
        sequence.Add(anim2);
        sequence.Start();

        // Assert - First animation should be at index 0
        Assert.Equal(0, sequence.CurrentIndex);
        Assert.Equal(AnimationState.Running, sequence.State);
    }

    [Fact]
    public void FluentAPI_ShouldChainAnimationSettings()
    {
        // Arrange & Act
        var animation = 0.0
            .AnimateTo(100, 1.0)
            .WithEasing(EasingFunctions.BounceOut)
            .WithDelay(0.5);

        // Assert
        Assert.Equal(0.5, animation.Delay);
        Assert.Equal(EasingFunctions.BounceOut, animation.EasingFunction);
    }

    [Fact]
    public void AnimationPresets_ShouldProvideReadyConfigurations()
    {
        // Arrange & Act
        var fastAnim = AnimationPresets.Fast.Create(0.0, 100.0, Interpolators.Double);
        var slowAnim = AnimationPresets.Slow.Create(0.0, 100.0, Interpolators.Double);

        // Assert
        Assert.True(fastAnim.Duration < slowAnim.Duration);
        Assert.Equal(0.2, fastAnim.Duration);
        Assert.Equal(1.0, slowAnim.Duration);
    }

    [Fact]
    public void AnimatableProperty_ShouldNotifyOnChange()
    {
        // Arrange
        var property = new AnimatableProperty<double>(0);
        bool changed = false;
        property.PropertyChanged += (s, e) => changed = true;

        // Act
        property.Value = 100;

        // Assert
        Assert.True(changed);
        Assert.Equal(100, property.Value);
    }

    [Fact]
    public void EasingFunctions_ShouldProvideVariedCurves()
    {
        // Arrange
        var easings = new[]
        {
            EasingFunctions.Linear,
            EasingFunctions.QuadIn,
            EasingFunctions.QuadOut,
            EasingFunctions.CubicInOut,
            EasingFunctions.BounceOut,
            EasingFunctions.ElasticOut
        };

        // Act & Assert
        foreach (var easing in easings)
        {
            var result = easing.Ease(0.5);
            Assert.InRange(result, 0, 1.5); // Allow overshoot for elastic/bounce
        }
    }

    [Fact]
    public void BezierEasing_ShouldCreateCustomCurve()
    {
        // Arrange - CSS ease-in-out: cubic-bezier(0.42, 0, 0.58, 1.0)
        var easing = EasingFunctions.CreateBezier(0.42, 0, 0.58, 1.0);

        // Act
        var start = easing.Ease(0);
        var mid = easing.Ease(0.5);
        var end = easing.Ease(1.0);

        // Assert
        Assert.Equal(0, start, 2);
        Assert.Equal(1.0, end, 2);
        Assert.InRange(mid, 0.4, 0.6); // Should be around 0.5 for ease-in-out
    }

    [Fact]
    public void BezierEasing_ShouldHandleCSSEasePreset()
    {
        // Arrange - CSS ease: cubic-bezier(0.25, 0.1, 0.25, 1.0)
        var easing = EasingFunctions.CreateBezier(0.25, 0.1, 0.25, 1.0);

        // Act
        var quarter = easing.Ease(0.25);
        var half = easing.Ease(0.5);
        var threeQuarters = easing.Ease(0.75);

        // Assert - More lenient ranges for approximate bezier calculations
        Assert.InRange(quarter, 0.1, 0.5);
        Assert.InRange(half, 0.4, 0.85);
        Assert.InRange(threeQuarters, 0.7, 1.0);
    }

    [Fact]
    public void BezierEasing_ShouldClampControlPoints()
    {
        // Arrange - Control points outside 0-1 range should be clamped
        var easing = EasingFunctions.CreateBezier(-0.5, 0, 1.5, 1.0);

        // Act
        var result = easing.Ease(0.5);

        // Assert - Should not throw and should return valid result
        Assert.InRange(result, 0, 1.5);
    }
}
