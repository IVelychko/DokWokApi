﻿using Domain.Abstractions.Repositories;
using Domain.Constants;
using Domain.DTOs.Requests.Users;
using FluentValidation;

namespace Application.Validators.Auth;

public sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    private readonly IUserRepository _userRepository;

    public RegisterUserRequestValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Matches(RegularExpressions.FirstName)
            .MinimumLength(2);

        RuleFor(x => x.UserName)
            .NotEmpty()
            .Matches(RegularExpressions.UserName)
            .MinimumLength(5)
            .MustAsync(IsUserNameUnique)
            .WithMessage("The user name is already taken");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(IsEmailUnique)
            .WithMessage("The email is already taken");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(RegularExpressions.PhoneNumber)
            .MinimumLength(9)
            .MustAsync(IsPhoneNumberUnique)
            .WithMessage("The phone number is already taken");

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(RegularExpressions.Password)
            .MinimumLength(6);
    }

    private async Task<bool> IsPhoneNumberUnique(string phoneNumber, CancellationToken cancellationToken)
    {
        return await _userRepository.IsPhoneNumberUniqueAsync(phoneNumber);
    }

    private async Task<bool> IsUserNameUnique(string userName, CancellationToken cancellationToken)
    {
        return await _userRepository.IsUserNameUniqueAsync(userName);
    }

    private async Task<bool> IsEmailUnique(string email, CancellationToken cancellationToken)
    {
        return await _userRepository.IsEmailUniqueAsync(email);
    }
}
