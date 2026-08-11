using TodoApp.Application.Auth.Commands.RefreshToken;

namespace TodoApp.UnitTests.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandValidatorTests
{
    private readonly RefreshTokenCommandValidator _validator = new();

    [Fact]
    public async Task ValidToken_ShouldNotHaveValidationErrors()
    {
        var command = new RefreshTokenCommand
        {
            Token = "valid-refresh-token"
        };

        var result = await _validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task EmptyToken_ShouldHaveValidationError()
    {
        var command = new RefreshTokenCommand
        {
            Token = ""
        };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Token");
    }
}
