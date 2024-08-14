﻿using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.User.Commands.RefreshToken;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty();

        RuleFor(x => x.RefreshToken).NotEmpty().Matches(RegularExpressions.Guid);
    }
}
