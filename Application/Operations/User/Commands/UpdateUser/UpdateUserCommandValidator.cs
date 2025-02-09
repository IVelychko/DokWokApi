using Domain.Abstractions.Repositories;
using Domain.DTOs.Commands.Users;
using Domain.Shared;
using FluentValidation;

namespace Application.Operations.User.Commands.UpdateUser;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(UserToUpdateExists)
            .WithErrorCode("404")
            .WithMessage("There is no user with this ID to update in the database")
            .DependentRules(() =>
            {
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
            });
    }

    private async Task<bool> UserToUpdateExists(long userId, CancellationToken cancellationToken)
    {
        return (await _userRepository.GetUserByIdAsync(userId)) is not null;
    }

    private async Task<bool> IsPhoneNumberNotTaken(UpdateUserCommand user, string phoneNumber, CancellationToken cancellationToken)
    {
        var existingEntity = await _userRepository.GetUserByIdAsync(user.Id);
        if (existingEntity is null)
        {
            return false;
        }

        if (phoneNumber != existingEntity.PhoneNumber)
        {
            var result = await _userRepository.IsPhoneNumberTakenAsync(phoneNumber);
            return result.Match(isTaken => !isTaken, error => false);
        }

        return true;
    }

    private async Task<bool> IsUserNameNotTaken(UpdateUserCommand user, string userName, CancellationToken cancellationToken)
    {
        var existingEntity = await _userRepository.GetUserByIdAsync(user.Id);
        if (existingEntity is null)
        {
            return false;
        }

        if (userName != existingEntity.UserName)
        {
            var result = await _userRepository.IsUserNameTakenAsync(userName);
            return result.Match(isTaken => !isTaken, error => false);
        }

        return true;
    }

    private async Task<bool> IsEmailNotTaken(UpdateUserCommand user, string email, CancellationToken cancellationToken)
    {
        var existingEntity = await _userRepository.GetUserByIdAsync(user.Id);
        if (existingEntity is null)
        {
            return false;
        }

        if (email != existingEntity.Email)
        {
            var result = await _userRepository.IsEmailTakenAsync(email);
            return result.Match(isTaken => !isTaken, error => false);
        }

        return true;
    }
}
