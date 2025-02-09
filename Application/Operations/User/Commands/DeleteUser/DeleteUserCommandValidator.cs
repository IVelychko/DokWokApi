using Domain.Abstractions.Repositories;
using Domain.DTOs.Commands.Users;
using FluentValidation;

namespace Application.Operations.User.Commands.DeleteUser;

public sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(UserToDeleteExists)
            .WithErrorCode("404")
            .WithMessage("There is no user with this ID to delete in the database");
    }

    private async Task<bool> UserToDeleteExists(long userId, CancellationToken cancellationToken) =>
        (await _userRepository.GetCustomerByIdAsync(userId)) is not null;
}
