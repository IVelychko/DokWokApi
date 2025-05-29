using Application.Constants;
using Domain.Abstractions.Repositories;
using Domain.Constants;
using Domain.DTOs.Requests.Users;
using Domain.Shared;
using FluentValidation;

namespace Application.Validators.Users;

public sealed class DeleteUserRequestValidator : AbstractValidator<DeleteUserRequest>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public DeleteUserRequestValidator(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;

        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(CustomerToDeleteExists)
            .WithErrorCode(ErrorCodes.EntityNotFound)
            .WithMessage("There is no user with this ID to delete in the database");
    }

    private async Task<bool> CustomerToDeleteExists(long userId, CancellationToken cancellationToken)
    {
        var adminRole = await _userRoleRepository.GetByNameAsync(UserRoles.Admin);
        adminRole = Ensure.EntityExists(adminRole, "Admin role not found");
        return await _userRepository.UserExistsAsync(userId, adminRole.Id);
    }
}
