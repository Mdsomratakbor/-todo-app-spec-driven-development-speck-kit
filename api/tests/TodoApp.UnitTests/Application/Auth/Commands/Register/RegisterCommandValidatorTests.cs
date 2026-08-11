using TodoApp.Application.Auth.Commands.Register;

namespace TodoApp.UnitTests.Application.Auth.Commands.Register;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new RegisterCommand
        {
            Email = "user@example.com",
            Password = "SecurePass1"
        };

        var result = await _validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task EmptyEmail_ShouldHaveValidationError()
    {
        var command = new RegisterCommand
        {
            Email = "",
            Password = "SecurePass1"
        };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task InvalidEmailFormat_ShouldHaveValidationError()
    {
        var command = new RegisterCommand
        {
            Email = "not-an-email",
            Password = "SecurePass1"
        };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task EmptyPassword_ShouldHaveValidationError()
    {
        var command = new RegisterCommand
        {
            Email = "user@example.com",
            Password = ""
        };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Fact]
    public async Task TooShortPassword_ShouldHaveValidationError()
    {
        var command = new RegisterCommand
        {
            Email = "user@example.com",
            Password = "Ab1"
        };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Fact]
    public async Task PasswordMissingUppercase_ShouldHaveValidationError()
    {
        var command = new RegisterCommand
        {
            Email = "user@example.com",
            Password = "securepass1"
        };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Fact]
    public async Task PasswordMissingLowercase_ShouldHaveValidationError()
    {
        var command = new RegisterCommand
        {
            Email = "user@example.com",
            Password = "SECUREPASS1"
        };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Fact]
    public async Task PasswordMissingDigit_ShouldHaveValidationError()
    {
        var command = new RegisterCommand
        {
            Email = "user@example.com",
            Password = "SecurePass"
        };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }
}
