using FluentValidation;

namespace TodoApp.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(v => v.Token)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}
