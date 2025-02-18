using Domain.Abstractions.Repositories;
using Domain.DTOs.Commands.Users;
using Domain.Entities;
using Domain.Shared;
using FluentValidation;

namespace Application.Operations.Users.Commands.UpdatePassword;

public sealed class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private User? _userToUpdate;
    
    public UpdatePasswordCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.UserId)
            .NotEmpty()
            .MustAsync(UserToUpdateExists)
            .WithErrorCode("404")
            .WithMessage("There is no user with this ID in the database")
            .Must(IsNotAdmin)
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
        _userToUpdate = await _userRepository.GetUserByIdWithDetailsAsNoTrackingAsync(userId);
        return _userToUpdate is not null;
    }

    private bool IsNotAdmin(long userId)
    {
        return _userToUpdate!.UserRole!.Name != UserRoles.Admin;
    }

    private bool IsOldPasswordCorrect(string oldPassword)
    {
        return _userRepository.CheckUserPassword(_userToUpdate!, oldPassword);
    }
}
