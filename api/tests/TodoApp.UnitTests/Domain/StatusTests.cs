using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Domain;

public class StatusTests
{
    [Fact]
    public void SetProperties_ShouldAssignCorrectly()
    {
        var status = new Status
        {
            Id = 1,
            Name = "Pending",
            Color = "#F39C12"
        };

        Assert.Equal(1, status.Id);
        Assert.Equal("Pending", status.Name);
        Assert.Equal("#F39C12", status.Color);
    }
}
