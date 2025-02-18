using Domain.Abstractions.Repositories;
using Domain.DTOs.Commands.Users;
using Domain.Shared;
using FluentValidation;

namespace Application.Operations.Users.Commands.UpdateUser;

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
            });
    }

    private async Task<bool> UserToUpdateExists(long userId, CancellationToken cancellationToken)
    {
        return await _userRepository.UserExistsAsync(userId);
    }

    private async Task<bool> IsPhoneNumberUnique(UpdateUserCommand user, string phoneNumber, CancellationToken cancellationToken)
    {
        return await _userRepository.IsPhoneNumberUniqueAsync(phoneNumber, user.Id);
    }

    private async Task<bool> IsUserNameUnique(UpdateUserCommand user, string userName, CancellationToken cancellationToken)
    {
        return await _userRepository.IsUserNameUniqueAsync(userName, user.Id);
    }

    private async Task<bool> IsEmailUnique(UpdateUserCommand user, string email, CancellationToken cancellationToken)
    {
        return await _userRepository.IsEmailUniqueAsync(email, user.Id);
    }
}
