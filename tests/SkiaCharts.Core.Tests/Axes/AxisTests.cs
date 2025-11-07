using SkiaCharts.Core.Axes;
using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Tests.Axes;

public class AxisTests
{
    #region LinearAxis Tests

    [Fact]
    public void LinearAxis_ShouldGenerateNiceTicks()
    {
        // Arrange
        var axis = new LinearAxis
        {
            VisibleRange = new DataRange(0, 100),
            TargetTickCount = 10
        };

        // Act
        var ticks = axis.GenerateTicks();

        // Assert
        Assert.NotEmpty(ticks);
        Assert.All(ticks, tick =>
        {
            Assert.InRange(tick.Value, 0, 100);
            Assert.NotNull(tick.Label);
        });
    }

    [Fact]
    public void LinearAxis_ShouldFormatValuesCorrectly()
    {
        // Arrange
        var axis = new LinearAxis();

        // Act & Assert
        Assert.Equal("0", axis.FormatValue(0));
        Assert.Equal("1.23", axis.FormatValue(1.234));
        Assert.Equal("12.35", axis.FormatValue(12.345)); // Banker's rounding
        Assert.Equal("1234", axis.FormatValue(1234.5)); // Banker's rounding
    }

    [Fact]
    public void LinearAxis_ShouldCalculateOptimalRange()
    {
        // Arrange
        var axis = new LinearAxis();
        var dataRange = new DataRange(10, 90);

        // Act
        var optimalRange = axis.CalculateOptimalRange(dataRange);

        // Assert
        Assert.True(optimalRange.Min <= dataRange.Min);
        Assert.True(optimalRange.Max >= dataRange.Max);
        Assert.True(optimalRange.Span > dataRange.Span); // Should have padding
    }

    [Fact]
    public void LinearAxis_ShouldHandleZeroSpanData()
    {
        // Arrange
        var axis = new LinearAxis();
        var dataRange = new DataRange(50, 50);

        // Act
        var optimalRange = axis.CalculateOptimalRange(dataRange);

        // Assert
        Assert.True(optimalRange.Span > 0);
        Assert.Equal(50, optimalRange.Center, 0.1);
    }

    [Fact]
    public void LinearAxis_ShouldRespectCustomFormat()
    {
        // Arrange
        var axis = new LinearAxis
        {
            LabelFormat = "F4"
        };

        // Act
        var formatted = axis.FormatValue(1.234567);

        // Assert
        Assert.Equal("1.2346", formatted);
    }

    [Fact]
    public void LinearAxis_ShouldHandleNegativeValues()
    {
        // Arrange
        var axis = new LinearAxis
        {
            VisibleRange = new DataRange(-100, 100),
            TargetTickCount = 10
        };

        // Act
        var ticks = axis.GenerateTicks();

        // Assert
        Assert.NotEmpty(ticks);
        Assert.Contains(ticks, t => t.Value < 0);
        Assert.Contains(ticks, t => t.Value > 0);
    }

    #endregion

    #region DateTimeAxis Tests

    [Fact]
    public void DateTimeAxis_ShouldGenerateTicksForDays()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 10);
        var axis = new DateTimeAxis
        {
            VisibleRange = new DataRange(startDate.ToOADate(), endDate.ToOADate())
        };

        // Act
        var ticks = axis.GenerateTicks();

        // Assert
        Assert.NotEmpty(ticks);
        Assert.True(ticks.Count > 0);
        Assert.All(ticks, tick =>
        {
            var date = DateTime.FromOADate(tick.Value);
            Assert.InRange(date, startDate, endDate);
        });
    }

    [Fact]
    public void DateTimeAxis_ShouldFormatDatesCorrectly()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15, 14, 30, 0);
        var axis = new DateTimeAxis
        {
            VisibleRange = new DataRange(date.ToOADate() - 1, date.ToOADate() + 1)
        };

        // Act
        var formatted = axis.FormatValue(date.ToOADate());

        // Assert
        Assert.NotEmpty(formatted);
        Assert.NotEqual("0", formatted);
    }

    [Fact]
    public void DateTimeAxis_ShouldHandleHourlyData()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0);
        var endDate = new DateTime(2024, 1, 1, 12, 0, 0);
        var axis = new DateTimeAxis
        {
            VisibleRange = new DataRange(startDate.ToOADate(), endDate.ToOADate())
        };

        // Act
        var ticks = axis.GenerateTicks();

        // Assert
        Assert.NotEmpty(ticks);
        // Should have multiple ticks for a 12-hour span
        Assert.InRange(ticks.Count, 2, 20);
    }

    [Fact]
    public void DateTimeAxis_ShouldHandleYearlyData()
    {
        // Arrange
        var startDate = new DateTime(2020, 1, 1);
        var endDate = new DateTime(2024, 12, 31);
        var axis = new DateTimeAxis
        {
            VisibleRange = new DataRange(startDate.ToOADate(), endDate.ToOADate())
        };

        // Act
        var ticks = axis.GenerateTicks();

        // Assert
        Assert.NotEmpty(ticks);
        Assert.True(ticks.Count > 0);
    }

    [Fact]
    public void DateTimeAxis_ShouldRespectCustomFormat()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15);
        var axis = new DateTimeAxis
        {
            LabelFormat = "yyyy-MM-dd"
        };

        // Act
        var formatted = axis.FormatValue(date.ToOADate());

        // Assert
        Assert.Equal("2024-01-15", formatted);
    }

    [Fact]
    public void DateTimeAxis_ShouldCalculateOptimalRange()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 10);
        var axis = new DateTimeAxis();
        var dataRange = new DataRange(startDate.ToOADate(), endDate.ToOADate());

        // Act
        var optimalRange = axis.CalculateOptimalRange(dataRange);

        // Assert
        Assert.True(optimalRange.Span >= dataRange.Span);
    }

    #endregion

    #region CategoryAxis Tests

    [Fact]
    public void CategoryAxis_ShouldStoreCategories()
    {
        // Arrange
        var categories = new[] { "Jan", "Feb", "Mar", "Apr" };
        var axis = new CategoryAxis(categories);

        // Assert
        Assert.Equal(4, axis.CategoryCount);
        Assert.Equal("Jan", axis.GetCategory(0));
        Assert.Equal("Mar", axis.GetCategory(2));
    }

    [Fact]
    public void CategoryAxis_ShouldGenerateTicksForEachCategory()
    {
        // Arrange
        var categories = new[] { "Q1", "Q2", "Q3", "Q4" };
        var axis = new CategoryAxis(categories);

        // Act
        var ticks = axis.GenerateTicks();

        // Assert
        Assert.Equal(4, ticks.Count);
        Assert.Equal("Q1", ticks[0].Label);
        Assert.Equal("Q4", ticks[3].Label);
    }

    [Fact]
    public void CategoryAxis_ShouldFormatValuesByIndex()
    {
        // Arrange
        var categories = new[] { "Apple", "Banana", "Cherry" };
        var axis = new CategoryAxis(categories);

        // Act
        var formatted0 = axis.FormatValue(0);
        var formatted1 = axis.FormatValue(1);
        var formatted2 = axis.FormatValue(2);

        // Assert
        Assert.Equal("Apple", formatted0);
        Assert.Equal("Banana", formatted1);
        Assert.Equal("Cherry", formatted2);
    }

    [Fact]
    public void CategoryAxis_ShouldHandleOutOfRangeIndices()
    {
        // Arrange
        var categories = new[] { "A", "B", "C" };
        var axis = new CategoryAxis(categories);

        // Act
        var formatted = axis.FormatValue(99);

        // Assert
        Assert.Equal(string.Empty, formatted);
    }

    [Fact]
    public void CategoryAxis_ShouldAllowAddingCategories()
    {
        // Arrange
        var axis = new CategoryAxis();

        // Act
        axis.AddCategory("First");
        axis.AddCategory("Second");
        axis.AddCategories(new[] { "Third", "Fourth" });

        // Assert
        Assert.Equal(4, axis.CategoryCount);
        Assert.Equal("Third", axis.GetCategory(2));
    }

    [Fact]
    public void CategoryAxis_ShouldCalculateOptimalRange()
    {
        // Arrange
        var categories = new[] { "A", "B", "C", "D", "E" };
        var axis = new CategoryAxis(categories);

        // Act
        var optimalRange = axis.CalculateOptimalRange(new DataRange(0, 4));

        // Assert
        Assert.Equal(-0.5, optimalRange.Min);
        Assert.Equal(4.5, optimalRange.Max);
    }

    [Fact]
    public void CategoryAxis_ShouldFindCategoryIndex()
    {
        // Arrange
        var categories = new[] { "Red", "Green", "Blue" };
        var axis = new CategoryAxis(categories);

        // Act
        var index = axis.GetCategoryIndex("Green");

        // Assert
        Assert.Equal(1, index);
    }

    [Fact]
    public void CategoryAxis_ShouldReturnNegativeOneForMissingCategory()
    {
        // Arrange
        var categories = new[] { "Red", "Green", "Blue" };
        var axis = new CategoryAxis(categories);

        // Act
        var index = axis.GetCategoryIndex("Yellow");

        // Assert
        Assert.Equal(-1, index);
    }

    [Fact]
    public void CategoryAxis_ShouldSkipLabelsWhenTooMany()
    {
        // Arrange
        var manyCategories = Enumerable.Range(1, 100).Select(i => $"Cat{i}").ToArray();
        var axis = new CategoryAxis(manyCategories)
        {
            MaxLabelsToShow = 10
        };

        // Act
        var ticks = axis.GenerateTicks();

        // Assert
        Assert.Equal(100, ticks.Count); // Should generate all ticks
        var labelsShown = ticks.Count(t => t.IsMajor);
        Assert.InRange(labelsShown, 5, 15); // But only show ~10 labels
    }

    [Fact]
    public void CategoryAxis_ShouldClearCategories()
    {
        // Arrange
        var axis = new CategoryAxis(new[] { "A", "B", "C" });

        // Act
        axis.ClearCategories();

        // Assert
        Assert.Equal(0, axis.CategoryCount);
    }

    #endregion

    #region General Axis Tests

    [Fact]
    public void AllAxes_ShouldImplementIAxis()
    {
        // Arrange & Act
        IAxis linear = new LinearAxis();
        IAxis dateTime = new DateTimeAxis();
        IAxis category = new CategoryAxis();

        // Assert
        Assert.NotNull(linear);
        Assert.NotNull(dateTime);
        Assert.NotNull(category);
    }

    [Fact]
    public void AllAxes_ShouldHaveDefaultProperties()
    {
        // Arrange
        var axes = new IAxis[]
        {
            new LinearAxis(),
            new DateTimeAxis(),
            new CategoryAxis()
        };

        // Assert
        foreach (var axis in axes)
        {
            Assert.True(axis.IsVisible);
            Assert.True(axis.AutoScale);
            Assert.True(axis.ShowGridLines);
            Assert.True(axis.ShowLabels);
        }
    }

    [Fact]
    public void AllAxes_ShouldAllowSettingProperties()
    {
        // Arrange
        var axis = new LinearAxis();

        // Act
        axis.Title = "Test Axis";
        axis.Position = AxisPosition.Left;
        axis.AutoScale = false;
        axis.ShowGridLines = false;
        axis.MinValue = 0;
        axis.MaxValue = 100;

        // Assert
        Assert.Equal("Test Axis", axis.Title);
        Assert.Equal(AxisPosition.Left, axis.Position);
        Assert.False(axis.AutoScale);
        Assert.False(axis.ShowGridLines);
        Assert.Equal(0, axis.MinValue);
        Assert.Equal(100, axis.MaxValue);
    }

    #endregion
}
