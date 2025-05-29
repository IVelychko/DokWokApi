using Application.Constants;
using Domain.Abstractions.Repositories;
using Domain.Constants;
using Domain.DTOs.Requests.Users;
using FluentValidation;

namespace Application.Validators.Users;

public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserRequestValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(UserToUpdateExists)
            .WithErrorCode(ErrorCodes.EntityNotFound)
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

    private async Task<bool> IsPhoneNumberUnique(UpdateUserRequest user, string phoneNumber, CancellationToken cancellationToken)
    {
        return await _userRepository.IsPhoneNumberUniqueAsync(phoneNumber, user.Id);
    }

    private async Task<bool> IsUserNameUnique(UpdateUserRequest user, string userName, CancellationToken cancellationToken)
    {
        return await _userRepository.IsUserNameUniqueAsync(userName, user.Id);
    }

    private async Task<bool> IsEmailUnique(UpdateUserRequest user, string email, CancellationToken cancellationToken)
    {
        return await _userRepository.IsEmailUniqueAsync(email, user.Id);
    }
}
