using TodoApp.Domain.Enums;

namespace TodoApp.UnitTests.Domain;

public class EnumTests
{
    [Theory]
    [InlineData(Priority.Low, 1)]
    [InlineData(Priority.Medium, 2)]
    [InlineData(Priority.High, 3)]
    [InlineData(Priority.Urgent, 4)]
    public void PriorityEnum_ShouldHaveCorrectValues(Priority priority, int expectedId)
    {
        Assert.Equal(expectedId, (int)priority);
    }

    [Theory]
    [InlineData(Status.Pending, 1)]
    [InlineData(Status.InProgress, 2)]
    [InlineData(Status.Completed, 3)]
    public void StatusEnum_ShouldHaveCorrectValues(Status status, int expectedId)
    {
        Assert.Equal(expectedId, (int)status);
    }
}
