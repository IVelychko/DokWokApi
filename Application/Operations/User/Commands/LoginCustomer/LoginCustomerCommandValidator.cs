using Domain.Abstractions.Repositories;
using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.User.Commands.LoginCustomer;

public sealed class LoginCustomerCommandValidator : AbstractValidator<LoginCustomerCommand>
{
    private readonly IUserRepository _userRepository;

    public LoginCustomerCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.UserName)
            .NotEmpty()
            .Matches(RegularExpressions.UserName)
            .MinimumLength(5)
            .MustAsync(UserExists)
            .WithErrorCode("404")
            .WithMessage("The credentials are wrong.")
            .MustAsync(IsCustomerOrAdmin)
            .WithMessage("The authorization is denied")
            .DependentRules(() =>
            {
                RuleFor(x => x.Password)
                    .NotEmpty()
                    .Matches(RegularExpressions.Password)
                    .MinimumLength(6);
            });
    }

    private async Task<bool> UserExists(string userName, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUserNameAsync(userName);
        return user is not null;
    }

    private async Task<bool> IsCustomerOrAdmin(string userName, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUserNameWithDetailsAsync(userName);
        if (user is null)
        {
            return false;
        }

        var isCustomerOrAdmin = user.UserRole?.Name == UserRoles.Admin || user.UserRole?.Name == UserRoles.Customer;
        return isCustomerOrAdmin;
    }
}
