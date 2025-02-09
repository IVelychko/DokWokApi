using Domain.Abstractions.Repositories;
using Domain.DTOs.Commands.Users;
using Domain.Shared;
using FluentValidation;

namespace Application.Operations.User.Commands.UpdatePassword;

public sealed class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private Domain.Entities.User? _userToUpdate;
    
    public UpdatePasswordCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.UserId)
            .NotEmpty()
            .MustAsync(UserToUpdateExists)
            .WithErrorCode("404")
            .WithMessage("There is no user with this ID in the database")
            .MustAsync(IsNotAdmin)
            .WithName("user")
            .WithMessage("Forbidden action")
            .DependentRules(() =>
            {
                RuleFor(x => x.OldPassword)
                    .NotEmpty()
                    .Matches(RegularExpressions.Password)
                    .MinimumLength(6)
                    .Must(IsOldPasswordCorrect)
                    .WithMessage("The credentials are invalid");

                RuleFor(x => x.NewPassword)
                    .NotEmpty()
                    .Matches(RegularExpressions.Password)
                    .MinimumLength(6);
            });
    }

    private async Task<bool> UserToUpdateExists(long userId, CancellationToken cancellationToken)
    {
        _userToUpdate = await _userRepository.GetUserByIdAsync(userId);
        return _userToUpdate is not null;
    }

    private async Task<bool> IsNotAdmin(long userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);
        if (user is null)
        {
            return false;
        }

        var isNotAdmin = user.UserRole?.Name != UserRoles.Admin;
        return isNotAdmin;
    }

    private bool IsOldPasswordCorrect(string oldPassword)
    {
        return _userRepository.CheckUserPassword(_userToUpdate!, oldPassword);
    }
}
