using SkiaCharts.Core.Drawing;
using SkiaCharts.Core.Rendering;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Drawing;

public class DrawingToolsTests
{
    // TrendLine Tests
    [Fact]
    public void TrendLine_Constructor_ShouldInitialize()
    {
        // Arrange & Act
        var trendLine = new TrendLine(0, 100, 10, 150);

        // Assert
        Assert.NotNull(trendLine.Id);
        Assert.True(trendLine.IsVisible);
        Assert.False(trendLine.IsSelected);
        Assert.Equal(0, trendLine.X1);
        Assert.Equal(100, trendLine.Y1);
        Assert.Equal(10, trendLine.X2);
        Assert.Equal(150, trendLine.Y2);
    }

    [Fact]
    public void TrendLine_Serialization_ShouldRoundTrip()
    {
        // Arrange
        var original = new TrendLine(0, 100, 10, 150)
        {
            ExtendLine = true,
            Color = SKColors.Red,
            LineWidth = 3f
        };

        // Act
        var data = original.Serialize();
        var deserialized = new TrendLine();
        deserialized.Deserialize(data);

        // Assert
        Assert.Equal(original.X1, deserialized.X1);
        Assert.Equal(original.Y1, deserialized.Y1);
        Assert.Equal(original.X2, deserialized.X2);
        Assert.Equal(original.Y2, deserialized.Y2);
        Assert.Equal(original.ExtendLine, deserialized.ExtendLine);
        Assert.Equal(original.Color, deserialized.Color);
        Assert.Equal(original.LineWidth, deserialized.LineWidth);
    }

    // FibonacciRetracement Tests
    [Fact]
    public void FibonacciRetracement_Constructor_ShouldInitialize()
    {
        // Arrange & Act
        var fib = new FibonacciRetracement(0, 100, 10, 150);

        // Assert
        Assert.NotNull(fib.Id);
        Assert.Equal(0, fib.X1);
        Assert.Equal(100, fib.Y1);
        Assert.Equal(10, fib.X2);
        Assert.Equal(150, fib.Y2);
        Assert.NotEmpty(fib.Levels);
        Assert.True(fib.ShowLabels);
    }

    [Fact]
    public void FibonacciRetracement_Serialization_ShouldRoundTrip()
    {
        // Arrange
        var original = new FibonacciRetracement(0, 100, 10, 150)
        {
            ShowLabels = false,
            ShowPrices = true,
            LabelFontSize = 14f
        };

        // Act
        var data = original.Serialize();
        var deserialized = new FibonacciRetracement();
        deserialized.Deserialize(data);

        // Assert
        Assert.Equal(original.X1, deserialized.X1);
        Assert.Equal(original.Y1, deserialized.Y1);
        Assert.Equal(original.X2, deserialized.X2);
        Assert.Equal(original.Y2, deserialized.Y2);
        Assert.Equal(original.ShowLabels, deserialized.ShowLabels);
        Assert.Equal(original.ShowPrices, deserialized.ShowPrices);
        Assert.Equal(original.LabelFontSize, deserialized.LabelFontSize);
    }

    // FibonacciExtension Tests
    [Fact]
    public void FibonacciExtension_Constructor_ShouldInitialize()
    {
        // Arrange & Act
        var fib = new FibonacciExtension(0, 100, 5, 150, 10, 120);

        // Assert
        Assert.NotNull(fib.Id);
        Assert.Equal(0, fib.X1);
        Assert.Equal(100, fib.Y1);
        Assert.Equal(5, fib.X2);
        Assert.Equal(150, fib.Y2);
        Assert.Equal(10, fib.X3);
        Assert.Equal(120, fib.Y3);
        Assert.NotEmpty(fib.Levels);
    }

    // HorizontalLine Tests
    [Fact]
    public void HorizontalLine_Constructor_ShouldInitialize()
    {
        // Arrange & Act
        var line = new HorizontalLine(100);

        // Assert
        Assert.NotNull(line.Id);
        Assert.Equal(100, line.Y);
        Assert.True(line.ShowLabel);
    }

    [Fact]
    public void HorizontalLine_Serialization_ShouldRoundTrip()
    {
        // Arrange
        var original = new HorizontalLine(100)
        {
            Label = "Support",
            ShowLabel = true,
            LabelFontSize = 12f
        };

        // Act
        var data = original.Serialize();
        var deserialized = new HorizontalLine();
        deserialized.Deserialize(data);

        // Assert
        Assert.Equal(original.Y, deserialized.Y);
        Assert.Equal(original.Label, deserialized.Label);
        Assert.Equal(original.ShowLabel, deserialized.ShowLabel);
        Assert.Equal(original.LabelFontSize, deserialized.LabelFontSize);
    }

    // VerticalLine Tests
    [Fact]
    public void VerticalLine_Constructor_ShouldInitialize()
    {
        // Arrange & Act
        var line = new VerticalLine(5);

        // Assert
        Assert.NotNull(line.Id);
        Assert.Equal(5, line.X);
        Assert.True(line.ShowLabel);
    }

    // Rectangle Tests
    [Fact]
    public void Rectangle_Constructor_ShouldInitialize()
    {
        // Arrange & Act
        var rect = new Rectangle(0, 100, 10, 150);

        // Assert
        Assert.NotNull(rect.Id);
        Assert.Equal(0, rect.X1);
        Assert.Equal(100, rect.Y1);
        Assert.Equal(10, rect.X2);
        Assert.Equal(150, rect.Y2);
        Assert.False(rect.Fill);
    }

    [Fact]
    public void Rectangle_Serialization_ShouldRoundTrip()
    {
        // Arrange
        var original = new Rectangle(0, 100, 10, 150)
        {
            Fill = true,
            FillColor = new SKColor(255, 0, 0, 100)
        };

        // Act
        var data = original.Serialize();
        var deserialized = new Rectangle();
        deserialized.Deserialize(data);

        // Assert
        Assert.Equal(original.X1, deserialized.X1);
        Assert.Equal(original.Y1, deserialized.Y1);
        Assert.Equal(original.X2, deserialized.X2);
        Assert.Equal(original.Y2, deserialized.Y2);
        Assert.Equal(original.Fill, deserialized.Fill);
    }

    // Ellipse Tests
    [Fact]
    public void Ellipse_Constructor_ShouldInitialize()
    {
        // Arrange & Act
        var ellipse = new Ellipse(0, 100, 10, 150);

        // Assert
        Assert.NotNull(ellipse.Id);
        Assert.Equal(0, ellipse.X1);
        Assert.Equal(100, ellipse.Y1);
        Assert.Equal(10, ellipse.X2);
        Assert.Equal(150, ellipse.Y2);
        Assert.False(ellipse.Fill);
    }

    // TextAnnotation Tests
    [Fact]
    public void TextAnnotation_Constructor_ShouldInitialize()
    {
        // Arrange & Act
        var text = new TextAnnotation(5, 100, "Test");

        // Assert
        Assert.NotNull(text.Id);
        Assert.Equal(5, text.X);
        Assert.Equal(100, text.Y);
        Assert.Equal("Test", text.Text);
        Assert.True(text.ShowBackground);
    }

    [Fact]
    public void TextAnnotation_Serialization_ShouldRoundTrip()
    {
        // Arrange
        var original = new TextAnnotation(5, 100, "Test")
        {
            FontSize = 16f,
            Bold = true,
            Italic = false,
            ShowBackground = true
        };

        // Act
        var data = original.Serialize();
        var deserialized = new TextAnnotation();
        deserialized.Deserialize(data);

        // Assert
        Assert.Equal(original.X, deserialized.X);
        Assert.Equal(original.Y, deserialized.Y);
        Assert.Equal(original.Text, deserialized.Text);
        Assert.Equal(original.FontSize, deserialized.FontSize);
        Assert.Equal(original.Bold, deserialized.Bold);
        Assert.Equal(original.Italic, deserialized.Italic);
        Assert.Equal(original.ShowBackground, deserialized.ShowBackground);
    }

    // DrawingManager Tests
    [Fact]
    public void DrawingManager_AddDrawing_ShouldAddToCollection()
    {
        // Arrange
        var manager = new DrawingManager();
        var trendLine = new TrendLine(0, 100, 10, 150);

        // Act
        manager.AddDrawing(trendLine);

        // Assert
        Assert.Single(manager.Drawings);
        Assert.Equal(trendLine, manager.Drawings[0]);
    }

    [Fact]
    public void DrawingManager_RemoveDrawing_ShouldRemoveFromCollection()
    {
        // Arrange
        var manager = new DrawingManager();
        var trendLine = new TrendLine(0, 100, 10, 150);
        manager.AddDrawing(trendLine);

        // Act
        var removed = manager.RemoveDrawing(trendLine);

        // Assert
        Assert.True(removed);
        Assert.Empty(manager.Drawings);
    }

    [Fact]
    public void DrawingManager_RemoveDrawingById_ShouldRemoveFromCollection()
    {
        // Arrange
        var manager = new DrawingManager();
        var trendLine = new TrendLine(0, 100, 10, 150);
        manager.AddDrawing(trendLine);
        var id = trendLine.Id;

        // Act
        var removed = manager.RemoveDrawingById(id);

        // Assert
        Assert.True(removed);
        Assert.Empty(manager.Drawings);
    }

    [Fact]
    public void DrawingManager_GetDrawingById_ShouldReturnDrawing()
    {
        // Arrange
        var manager = new DrawingManager();
        var trendLine = new TrendLine(0, 100, 10, 150);
        manager.AddDrawing(trendLine);
        var id = trendLine.Id;

        // Act
        var drawing = manager.GetDrawingById(id);

        // Assert
        Assert.NotNull(drawing);
        Assert.Equal(trendLine, drawing);
    }

    [Fact]
    public void DrawingManager_SelectDrawing_ShouldSelectOnlyOne()
    {
        // Arrange
        var manager = new DrawingManager();
        var trendLine1 = new TrendLine(0, 100, 10, 150);
        var trendLine2 = new TrendLine(5, 120, 15, 170);
        manager.AddDrawing(trendLine1);
        manager.AddDrawing(trendLine2);

        // Act
        manager.SelectDrawing(trendLine1);

        // Assert
        Assert.True(trendLine1.IsSelected);
        Assert.False(trendLine2.IsSelected);
    }

    [Fact]
    public void DrawingManager_GetSelectedDrawing_ShouldReturnSelectedDrawing()
    {
        // Arrange
        var manager = new DrawingManager();
        var trendLine = new TrendLine(0, 100, 10, 150);
        manager.AddDrawing(trendLine);
        manager.SelectDrawing(trendLine);

        // Act
        var selected = manager.GetSelectedDrawing();

        // Assert
        Assert.NotNull(selected);
        Assert.Equal(trendLine, selected);
    }

    [Fact]
    public void DrawingManager_Clear_ShouldRemoveAllDrawings()
    {
        // Arrange
        var manager = new DrawingManager();
        manager.AddDrawing(new TrendLine(0, 100, 10, 150));
        manager.AddDrawing(new HorizontalLine(100));

        // Act
        manager.Clear();

        // Assert
        Assert.Empty(manager.Drawings);
    }

    [Fact]
    public void DrawingManager_SerializeToJson_ShouldProduceValidJson()
    {
        // Arrange
        var manager = new DrawingManager();
        manager.AddDrawing(new TrendLine(0, 100, 10, 150));
        manager.AddDrawing(new HorizontalLine(100));

        // Act
        var json = manager.SerializeToJson();

        // Assert
        Assert.NotEmpty(json);
        Assert.Contains("TrendLine", json);
        Assert.Contains("HorizontalLine", json);
    }

    [Fact]
    public void DrawingManager_DeserializeFromJson_ShouldRestoreDrawings()
    {
        // Arrange
        var manager = new DrawingManager();
        manager.AddDrawing(new TrendLine(0, 100, 10, 150));
        manager.AddDrawing(new HorizontalLine(100));
        var json = manager.SerializeToJson();

        // Act
        var newManager = new DrawingManager();
        var success = newManager.DeserializeFromJson(json);

        // Assert
        Assert.True(success);
        Assert.Equal(2, newManager.Drawings.Count);
    }

    [Fact]
    public void DrawingManager_ExportToData_ShouldReturnList()
    {
        // Arrange
        var manager = new DrawingManager();
        manager.AddDrawing(new TrendLine(0, 100, 10, 150));
        manager.AddDrawing(new HorizontalLine(100));

        // Act
        var data = manager.ExportToData();

        // Assert
        Assert.Equal(2, data.Count);
        Assert.Contains(data, d => d.ContainsValue("TrendLine"));
        Assert.Contains(data, d => d.ContainsValue("HorizontalLine"));
    }

    [Fact]
    public void DrawingManager_ImportFromData_ShouldRestoreDrawings()
    {
        // Arrange
        var manager = new DrawingManager();
        manager.AddDrawing(new TrendLine(0, 100, 10, 150));
        manager.AddDrawing(new HorizontalLine(100));
        var data = manager.ExportToData();

        // Act
        var newManager = new DrawingManager();
        var success = newManager.ImportFromData(data);

        // Assert
        Assert.True(success);
        Assert.Equal(2, newManager.Drawings.Count);
    }
}
