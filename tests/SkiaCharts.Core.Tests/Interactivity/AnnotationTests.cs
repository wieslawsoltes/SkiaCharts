using SkiaCharts.Core.Interactivity;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Interactivity;

public class AnnotationTests
{
    // Annotation Manager Tests
    [Fact]
    public void AnnotationManager_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var manager = new AnnotationManager();

        // Assert
        Assert.True(manager.IsEnabled);
        Assert.Empty(manager.Annotations);
    }

    [Fact]
    public void AnnotationManager_AddAnnotation_ShouldAddToCollection()
    {
        // Arrange
        var manager = new AnnotationManager();
        var annotation = new PointAnnotation
        {
            X = 100,
            Y = 50
        };

        // Act
        manager.AddAnnotation(annotation);

        // Assert
        Assert.Single(manager.Annotations);
        Assert.Contains(annotation, manager.Annotations);
    }

    [Fact]
    public void AnnotationManager_RemoveAnnotation_ShouldRemoveFromCollection()
    {
        // Arrange
        var manager = new AnnotationManager();
        var annotation = new PointAnnotation
        {
            X = 100,
            Y = 50
        };
        manager.AddAnnotation(annotation);

        // Act
        var removed = manager.RemoveAnnotation(annotation);

        // Assert
        Assert.True(removed);
        Assert.Empty(manager.Annotations);
    }

    [Fact]
    public void AnnotationManager_Clear_ShouldRemoveAllAnnotations()
    {
        // Arrange
        var manager = new AnnotationManager();
        manager.AddAnnotation(new PointAnnotation { X = 100, Y = 50 });
        manager.AddAnnotation(new ThresholdAnnotation { Y = 75 });

        // Act
        manager.Clear();

        // Assert
        Assert.Empty(manager.Annotations);
    }

    [Fact]
    public void AnnotationManager_ShouldFireAnnotationAddedEvent()
    {
        // Arrange
        var manager = new AnnotationManager();
        var annotation = new PointAnnotation { X = 100, Y = 50 };

        bool eventFired = false;
        IAnnotation? addedAnnotation = null;

        manager.AnnotationAdded += (s, e) =>
        {
            eventFired = true;
            addedAnnotation = e.Annotation;
        };

        // Act
        manager.AddAnnotation(annotation);

        // Assert
        Assert.True(eventFired);
        Assert.Equal(annotation, addedAnnotation);
    }

    [Fact]
    public void AnnotationManager_ShouldFireAnnotationRemovedEvent()
    {
        // Arrange
        var manager = new AnnotationManager();
        var annotation = new PointAnnotation { X = 100, Y = 50 };
        manager.AddAnnotation(annotation);

        bool eventFired = false;

        manager.AnnotationRemoved += (s, e) =>
        {
            eventFired = true;
        };

        // Act
        manager.RemoveAnnotation(annotation);

        // Assert
        Assert.True(eventFired);
    }

    [Fact]
    public void AnnotationManager_HitTest_ShouldDetectAnnotation()
    {
        // Arrange
        var manager = new AnnotationManager();
        var annotation = new PointAnnotation
        {
            X = 100,
            Y = 50
        };
        manager.AddAnnotation(annotation);

        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 200, 100),
            ViewBounds = new SKRect(0, 0, 400, 200),
            Zoom = 2.0f
        };

        var screenPos = viewport.DataToScreen(new SKPoint(100, 50));

        // Act
        var hitAnnotation = manager.HitTest(screenPos, viewport);

        // Assert
        Assert.Equal(annotation, hitAnnotation);
    }

    [Fact]
    public void AnnotationManager_HitTest_OutsideBounds_ShouldReturnNull()
    {
        // Arrange
        var manager = new AnnotationManager();
        manager.AddAnnotation(new PointAnnotation { X = 100, Y = 50 });

        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 200, 100),
            ViewBounds = new SKRect(0, 0, 400, 200),
            Zoom = 2.0f
        };

        // Act
        var hitAnnotation = manager.HitTest(new SKPoint(-100, -100), viewport);

        // Assert
        Assert.Null(hitAnnotation);
    }

    [Fact]
    public void AnnotationManager_HandleClick_ShouldFireEvent()
    {
        // Arrange
        var manager = new AnnotationManager();
        var annotation = new PointAnnotation
        {
            X = 100,
            Y = 50
        };
        manager.AddAnnotation(annotation);

        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 200, 100),
            ViewBounds = new SKRect(0, 0, 400, 200),
            Zoom = 2.0f
        };

        var screenPos = viewport.DataToScreen(new SKPoint(100, 50));

        bool eventFired = false;
        IAnnotation? clickedAnnotation = null;

        manager.AnnotationClicked += (s, e) =>
        {
            eventFired = true;
            clickedAnnotation = e.Annotation;
        };

        // Act
        var handled = manager.HandleClick(screenPos, viewport);

        // Assert
        Assert.True(handled);
        Assert.True(eventFired);
        Assert.Equal(annotation, clickedAnnotation);
    }

    [Fact]
    public void AnnotationManager_Render_ShouldNotThrow()
    {
        // Arrange
        var manager = new AnnotationManager();
        manager.AddAnnotation(new PointAnnotation { X = 100, Y = 50, LabelText = "Test" });
        manager.AddAnnotation(new ThresholdAnnotation { Y = 75, LabelText = "Threshold" });
        manager.AddAnnotation(new RangeAnnotation { StartX = 50, EndX = 150, LabelText = "Range" });

        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 200, 100),
            ViewBounds = new SKRect(0, 0, 400, 200),
            Zoom = 2.0f
        };

        var chartBounds = new SKRect(0, 0, 400, 200);

        using var surface = SKSurface.Create(new SKImageInfo(400, 200));
        var canvas = surface.Canvas;

        // Act & Assert - Should not throw
        manager.Render(canvas, chartBounds, viewport);
    }

    // Point Annotation Tests
    [Fact]
    public void PointAnnotation_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var annotation = new PointAnnotation
        {
            X = 100,
            Y = 50,
            Name = "Test Point",
            MarkerSize = 10,
            MarkerColor = SKColors.Blue,
            MarkerType = PointMarkerType.Square,
            LabelText = "Point Label"
        };

        // Assert
        Assert.Equal(100, annotation.X);
        Assert.Equal(50, annotation.Y);
        Assert.Equal("Test Point", annotation.Name);
        Assert.Equal(10, annotation.MarkerSize);
        Assert.Equal(SKColors.Blue, annotation.MarkerColor);
        Assert.Equal(PointMarkerType.Square, annotation.MarkerType);
        Assert.Equal("Point Label", annotation.LabelText);
        Assert.True(annotation.IsVisible);
        Assert.True(annotation.ShowLabel);
    }

    [Fact]
    public void PointAnnotation_Render_ShouldDrawAllMarkerTypes()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 200, 100),
            ViewBounds = new SKRect(0, 0, 400, 200),
            Zoom = 2.0f
        };

        var chartBounds = new SKRect(0, 0, 400, 200);

        using var surface = SKSurface.Create(new SKImageInfo(400, 200));
        var canvas = surface.Canvas;

        var markerTypes = new[]
        {
            PointMarkerType.Circle,
            PointMarkerType.Square,
            PointMarkerType.Triangle,
            PointMarkerType.Diamond,
            PointMarkerType.Cross
        };

        foreach (var markerType in markerTypes)
        {
            var annotation = new PointAnnotation
            {
                X = 100,
                Y = 50,
                MarkerType = markerType
            };

            // Act & Assert - Should not throw
            annotation.Render(canvas, chartBounds, viewport);
        }
    }

    [Fact]
    public void PointAnnotation_HitTest_ShouldDetectClick()
    {
        // Arrange
        var annotation = new PointAnnotation
        {
            X = 100,
            Y = 50,
            MarkerSize = 8
        };

        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 200, 100),
            ViewBounds = new SKRect(0, 0, 400, 200),
            Zoom = 2.0f
        };

        var screenPos = viewport.DataToScreen(new SKPoint(100, 50));

        // Act
        var hit = annotation.HitTest(screenPos, viewport);

        // Assert
        Assert.True(hit);
    }

    [Fact]
    public void PointAnnotation_HitTest_OutsideMarker_ShouldReturnFalse()
    {
        // Arrange
        var annotation = new PointAnnotation
        {
            X = 100,
            Y = 50
        };

        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 200, 100),
            ViewBounds = new SKRect(0, 0, 400, 200),
            Zoom = 2.0f
        };

        // Act
        var hit = annotation.HitTest(new SKPoint(0, 0), viewport);

        // Assert
        Assert.False(hit);
    }

    // Range Annotation Tests
    [Fact]
    public void RangeAnnotation_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var annotation = new RangeAnnotation
        {
            StartX = 50,
            EndX = 150,
            Name = "Test Range",
            FillColor = new SKColor(255, 0, 0, 100),
            BorderColor = SKColors.Red,
            BorderWidth = 2,
            LabelText = "Range Label"
        };

        // Assert
        Assert.Equal(50, annotation.StartX);
        Assert.Equal(150, annotation.EndX);
        Assert.Equal("Test Range", annotation.Name);
        Assert.Equal(new SKColor(255, 0, 0, 100), annotation.FillColor);
        Assert.Equal(SKColors.Red, annotation.BorderColor);
        Assert.Equal(2, annotation.BorderWidth);
        Assert.Equal("Range Label", annotation.LabelText);
        Assert.True(annotation.IsVisible);
        Assert.True(annotation.ShowLabel);
    }

    [Fact]
    public void RangeAnnotation_Render_ShouldNotThrow()
    {
        // Arrange
        var annotation = new RangeAnnotation
        {
            StartX = 50,
            EndX = 150,
            LabelText = "Test Range"
        };

        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 200, 100),
            ViewBounds = new SKRect(0, 0, 400, 200),
            Zoom = 2.0f
        };

        var chartBounds = new SKRect(0, 0, 400, 200);

        using var surface = SKSurface.Create(new SKImageInfo(400, 200));
        var canvas = surface.Canvas;

        // Act & Assert - Should not throw
        annotation.Render(canvas, chartBounds, viewport);
    }

    [Fact]
    public void RangeAnnotation_HitTest_InsideRange_ShouldReturnTrue()
    {
        // Arrange
        var annotation = new RangeAnnotation
        {
            StartX = 50,
            EndX = 150
        };

        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 200, 100),
            ViewBounds = new SKRect(0, 0, 400, 200),
            Zoom = 2.0f
        };

        var middlePos = viewport.DataToScreen(new SKPoint(100, 50)); // Middle of range

        // Act
        var hit = annotation.HitTest(middlePos, viewport);

        // Assert
        Assert.True(hit);
    }

    [Fact]
    public void RangeAnnotation_HitTest_OutsideRange_ShouldReturnFalse()
    {
        // Arrange
        var annotation = new RangeAnnotation
        {
            StartX = 50,
            EndX = 150
        };

        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 200, 100),
            ViewBounds = new SKRect(0, 0, 400, 200),
            Zoom = 2.0f
        };

        var outsidePos = viewport.DataToScreen(new SKPoint(200, 50)); // Outside range

        // Act
        var hit = annotation.HitTest(outsidePos, viewport);

        // Assert
        Assert.False(hit);
    }

    [Fact]
    public void RangeAnnotation_Render_AllLabelPositions_ShouldNotThrow()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 200, 100),
            ViewBounds = new SKRect(0, 0, 400, 200),
            Zoom = 2.0f
        };

        var chartBounds = new SKRect(0, 0, 400, 200);

        using var surface = SKSurface.Create(new SKImageInfo(400, 200));
        var canvas = surface.Canvas;

        var positions = new[]
        {
            RangeLabelPosition.Top,
            RangeLabelPosition.Middle,
            RangeLabelPosition.Bottom
        };

        foreach (var position in positions)
        {
            var annotation = new RangeAnnotation
            {
                StartX = 50,
                EndX = 150,
                LabelText = "Test",
                LabelPosition = position
            };

            // Act & Assert - Should not throw
            annotation.Render(canvas, chartBounds, viewport);
        }
    }

    // Threshold Annotation Tests
    [Fact]
    public void ThresholdAnnotation_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var annotation = new ThresholdAnnotation
        {
            Y = 75,
            Name = "Test Threshold",
            LineColor = SKColors.Green,
            LineWidth = 3,
            LineStyle = ThresholdLineStyle.Dashed,
            LabelText = "Threshold Label"
        };

        // Assert
        Assert.Equal(75, annotation.Y);
        Assert.Equal("Test Threshold", annotation.Name);
        Assert.Equal(SKColors.Green, annotation.LineColor);
        Assert.Equal(3, annotation.LineWidth);
        Assert.Equal(ThresholdLineStyle.Dashed, annotation.LineStyle);
        Assert.Equal("Threshold Label", annotation.LabelText);
        Assert.True(annotation.IsVisible);
        Assert.True(annotation.ShowLabel);
    }

    [Fact]
    public void ThresholdAnnotation_Render_AllLineStyles_ShouldNotThrow()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 200, 100),
            ViewBounds = new SKRect(0, 0, 400, 200),
            Zoom = 2.0f
        };

        var chartBounds = new SKRect(0, 0, 400, 200);

        using var surface = SKSurface.Create(new SKImageInfo(400, 200));
        var canvas = surface.Canvas;

        var lineStyles = new[]
        {
            ThresholdLineStyle.Solid,
            ThresholdLineStyle.Dashed,
            ThresholdLineStyle.Dotted
        };

        foreach (var lineStyle in lineStyles)
        {
            var annotation = new ThresholdAnnotation
            {
                Y = 50,
                LineStyle = lineStyle
            };

            // Act & Assert - Should not throw
            annotation.Render(canvas, chartBounds, viewport);
        }
    }

    [Fact]
    public void ThresholdAnnotation_Render_AllLabelPositions_ShouldNotThrow()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 200, 100),
            ViewBounds = new SKRect(0, 0, 400, 200),
            Zoom = 2.0f
        };

        var chartBounds = new SKRect(0, 0, 400, 200);

        using var surface = SKSurface.Create(new SKImageInfo(400, 200));
        var canvas = surface.Canvas;

        var positions = new[]
        {
            ThresholdLabelPosition.Left,
            ThresholdLabelPosition.Center,
            ThresholdLabelPosition.Right
        };

        foreach (var position in positions)
        {
            var annotation = new ThresholdAnnotation
            {
                Y = 50,
                LabelText = "Test",
                LabelPosition = position
            };

            // Act & Assert - Should not throw
            annotation.Render(canvas, chartBounds, viewport);
        }
    }

    [Fact]
    public void ThresholdAnnotation_HitTest_OnLine_ShouldReturnTrue()
    {
        // Arrange
        var annotation = new ThresholdAnnotation
        {
            Y = 50,
            LineWidth = 2
        };

        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 200, 100),
            ViewBounds = new SKRect(0, 0, 400, 200),
            Zoom = 2.0f
        };

        var linePos = viewport.DataToScreen(new SKPoint(100, 50)); // On the line

        // Act
        var hit = annotation.HitTest(linePos, viewport);

        // Assert
        Assert.True(hit);
    }

    [Fact]
    public void ThresholdAnnotation_HitTest_AwayFromLine_ShouldReturnFalse()
    {
        // Arrange
        var annotation = new ThresholdAnnotation
        {
            Y = 50
        };

        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 200, 100),
            ViewBounds = new SKRect(0, 0, 400, 200),
            Zoom = 2.0f
        };

        var awayPos = viewport.DataToScreen(new SKPoint(100, 0)); // Far from line

        // Act
        var hit = annotation.HitTest(awayPos, viewport);

        // Assert
        Assert.False(hit);
    }

    // Custom Annotation Tests
    [Fact]
    public void CustomAnnotation_ShouldCallRenderFunction()
    {
        // Arrange
        bool renderCalled = false;

        var annotation = new CustomAnnotation
        {
            RenderFunction = (canvas, chartBounds, viewport) =>
            {
                renderCalled = true;
            }
        };

        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 200, 100),
            ViewBounds = new SKRect(0, 0, 400, 200),
            Zoom = 2.0f
        };

        var chartBounds = new SKRect(0, 0, 400, 200);

        using var surface = SKSurface.Create(new SKImageInfo(400, 200));
        var canvas = surface.Canvas;

        // Act
        annotation.Render(canvas, chartBounds, viewport);

        // Assert
        Assert.True(renderCalled);
    }

    [Fact]
    public void CustomAnnotation_ShouldCallHitTestFunction()
    {
        // Arrange
        bool hitTestCalled = false;

        var annotation = new CustomAnnotation
        {
            HitTestFunction = (position, viewport) =>
            {
                hitTestCalled = true;
                return true;
            }
        };

        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 200, 100),
            ViewBounds = new SKRect(0, 0, 400, 200),
            Zoom = 2.0f
        };

        // Act
        var result = annotation.HitTest(new SKPoint(100, 50), viewport);

        // Assert
        Assert.True(hitTestCalled);
        Assert.True(result);
    }

    [Fact]
    public void CustomAnnotation_WithoutFunctions_ShouldNotThrow()
    {
        // Arrange
        var annotation = new CustomAnnotation();

        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 200, 100),
            ViewBounds = new SKRect(0, 0, 400, 200),
            Zoom = 2.0f
        };

        var chartBounds = new SKRect(0, 0, 400, 200);

        using var surface = SKSurface.Create(new SKImageInfo(400, 200));
        var canvas = surface.Canvas;

        // Act & Assert - Should not throw
        annotation.Render(canvas, chartBounds, viewport);
        var result = annotation.HitTest(new SKPoint(100, 50), viewport);
        Assert.False(result);
    }

    // General Annotation Tests
    [Fact]
    public void Annotation_IsVisible_ShouldControlRendering()
    {
        // Arrange
        var annotation = new PointAnnotation
        {
            X = 100,
            Y = 50,
            IsVisible = false
        };

        var manager = new AnnotationManager();
        manager.AddAnnotation(annotation);

        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 200, 100),
            ViewBounds = new SKRect(0, 0, 400, 200),
            Zoom = 2.0f
        };

        var chartBounds = new SKRect(0, 0, 400, 200);

        using var surface = SKSurface.Create(new SKImageInfo(400, 200));
        var canvas = surface.Canvas;

        // Act - Should not render invisible annotation
        manager.Render(canvas, chartBounds, viewport);

        // Assert - Annotation should be in collection but not rendered
        Assert.Single(manager.Annotations);
        Assert.False(annotation.IsVisible);
    }

    [Fact]
    public void Annotation_Data_ShouldStoreAssociatedData()
    {
        // Arrange
        var customData = new { Name = "Test", Value = 42 };

        var annotation = new PointAnnotation
        {
            X = 100,
            Y = 50,
            Data = customData
        };

        // Act & Assert
        Assert.NotNull(annotation.Data);
        Assert.Equal(customData, annotation.Data);
    }
}
