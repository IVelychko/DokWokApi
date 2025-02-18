using Domain.Abstractions.Repositories;
using Domain.DTOs.Commands.Users;
using FluentValidation;

namespace Application.Operations.Users.Commands.DeleteUser;

public sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(CustomerToDeleteExists)
            .WithErrorCode("404")
            .WithMessage("There is no user with this ID to delete in the database");
    }

    private async Task<bool> CustomerToDeleteExists(long userId, CancellationToken cancellationToken) =>
        await _userRepository.CustomerExistsAsync(userId);
}
