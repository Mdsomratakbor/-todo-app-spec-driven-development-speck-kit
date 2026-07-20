using TodoApp.Application.Auth.Commands.Login;

namespace TodoApp.UnitTests.Application.Auth.Commands.Login;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new LoginCommand
        {
            Email = "user@example.com",
            Password = "password"
        };

        var result = await _validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task EmptyEmail_ShouldHaveValidationError()
    {
        var command = new LoginCommand
        {
            Email = "",
            Password = "password"
        };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task InvalidEmailFormat_ShouldHaveValidationError()
    {
        var command = new LoginCommand
        {
            Email = "not-an-email",
            Password = "password"
        };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task EmptyPassword_ShouldHaveValidationError()
    {
        var command = new LoginCommand
        {
            Email = "user@example.com",
            Password = ""
        };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }
}
