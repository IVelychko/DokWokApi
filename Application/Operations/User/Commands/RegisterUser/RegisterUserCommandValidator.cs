using Domain.Abstractions.Repositories;
using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.User.Commands.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly IUserRepository _userRepository;

    public RegisterUserCommandValidator(IUserRepository userRepository)
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
            .MustAsync(IsUserNameNotTaken)
            .WithMessage("The user name is already taken");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(IsEmailNotTaken)
            .WithMessage("The email is already taken");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(RegularExpressions.PhoneNumber)
            .MinimumLength(9)
            .MustAsync(IsPhoneNumberNotTaken)
            .WithMessage("The phone number is already taken");

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(RegularExpressions.Password)
            .MinimumLength(6);
    }

    private async Task<bool> IsPhoneNumberNotTaken(string phoneNumber, CancellationToken cancellationToken)
    {
        var result = await _userRepository.IsPhoneNumberTakenAsync(phoneNumber);
        return result.Match(isTaken => !isTaken, error => false);
    }

    private async Task<bool> IsUserNameNotTaken(string userName, CancellationToken cancellationToken)
    {
        var result = await _userRepository.IsUserNameTakenAsync(userName);
        return result.Match(isTaken => !isTaken, error => false);
    }

    private async Task<bool> IsEmailNotTaken(string email, CancellationToken cancellationToken)
    {
        var result = await _userRepository.IsEmailTakenAsync(email);
        return result.Match(isTaken => !isTaken, error => false);
    }
}
