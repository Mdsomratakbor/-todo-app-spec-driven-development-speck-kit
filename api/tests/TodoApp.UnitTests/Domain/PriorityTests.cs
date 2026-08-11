using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Domain;

public class PriorityTests
{
    [Fact]
    public void SetProperties_ShouldAssignCorrectly()
    {
        var priority = new Priority
        {
            Id = 1,
            Name = "Low",
            Color = "#27AE60"
        };

        Assert.Equal(1, priority.Id);
        Assert.Equal("Low", priority.Name);
        Assert.Equal("#27AE60", priority.Color);
    }
}
