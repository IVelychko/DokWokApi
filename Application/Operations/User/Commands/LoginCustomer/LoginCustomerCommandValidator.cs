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

        RuleFor(x => x.UserName)
            .NotEmpty()
            .Matches(RegularExpressions.UserName)
            .MinimumLength(5)
            .MustAsync(UserExists)
            .WithName("user")
            .WithErrorCode("404")
            .WithMessage("The credentials are wrong.")
            .MustAsync(IsCustomerOrAdmin)
            .WithName("user")
            .WithMessage("The authorization is denied")
            .DependentRules(() =>
            {
                RuleFor(x => x.Password)
                    .NotEmpty()
                    .Matches(RegularExpressions.Password)
                    .MinimumLength(6)
                    .MustAsync(IsValidPassword)
                    .WithName("user")
                    .WithMessage("The credentials are wrong");
            });
    }

    private async Task<bool> UserExists(string userName, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUserNameAsync(userName);
        return user is not null;
    }

    private async Task<bool> IsCustomerOrAdmin(string userName, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUserNameAsync(userName);
        if (user is null)
        {
            return false;
        }

        var isCustomerResult = await _userRepository.IsInRoleAsync(user, UserRoles.Customer);
        var isCustomer = isCustomerResult.Match(x => x, e => false);
        var isAdminResult = await _userRepository.IsInRoleAsync(user, UserRoles.Admin);
        var isAdmin = isAdminResult.Match(x => x, e => false);
        return isCustomer || isAdmin;
    }

    private async Task<bool> IsValidPassword(LoginCustomerCommand customerLogin, string password, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUserNameAsync(customerLogin.UserName);
        if (user is null)
        {
            return false;
        }

        return await _userRepository.CheckUserPasswordAsync(user, password);
    }
}
