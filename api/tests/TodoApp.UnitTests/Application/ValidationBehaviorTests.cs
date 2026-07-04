using FluentValidation;
using MediatR;
using Moq;
using TodoApp.Application.Common.Behaviors;

namespace TodoApp.UnitTests.Application;

public class ValidationBehaviorTests
{
    public record TestRequest : IRequest<string>;

    [Fact]
    public async Task Handle_WhenNoValidators_ShouldCallNext()
    {
        var validators = Array.Empty<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var nextMock = new Mock<RequestHandlerDelegate<string>>();
        nextMock.Setup(n => n()).ReturnsAsync("success");

        var result = await behavior.Handle(new TestRequest(), nextMock.Object, default);

        Assert.Equal("success", result);
        nextMock.Verify(n => n(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAllValidatorsPass_ShouldCallNext()
    {
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var validators = new[] { validatorMock.Object };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var nextMock = new Mock<RequestHandlerDelegate<string>>();
        nextMock.Setup(n => n()).ReturnsAsync("success");

        var result = await behavior.Handle(new TestRequest(), nextMock.Object, default);

        Assert.Equal("success", result);
        nextMock.Verify(n => n(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldThrowValidationException()
    {
        var failures = new List<FluentValidation.Results.ValidationFailure>
        {
            new("Title", "Title is required"),
            new("Description", "Description is too long")
        };

        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(failures));

        var validators = new[] { validatorMock.Object };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var nextMock = new Mock<RequestHandlerDelegate<string>>();

        await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(new TestRequest(), nextMock.Object, default));

        nextMock.Verify(n => n(), Times.Never);
    }
}
