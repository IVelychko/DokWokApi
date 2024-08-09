using Domain.Abstractions.Repositories;
using Domain.Helpers;
using FluentValidation;

namespace Domain.Validation.Users.AdminLogin;

public sealed class AdminLoginValidator : AbstractValidator<AdminLoginValidationModel>
{
    private readonly IUserRepository _userRepository;

    public AdminLoginValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x)
            .NotNull()
            .WithMessage("The passed admin login model is null")
            .MustAsync(IsUserNameAndPasswordNotNull)
            .WithMessage("User name or password is null")
            .MustAsync(UserExists)
            .WithState(_ => new ValidationFailureState { IsNotFound = true })
            .WithMessage("The credentials are wrong.")
            .MustAsync(IsAdmin)
            .WithMessage("The authorization is denied")
            .MustAsync(IsValidPassword)
            .WithMessage("The credentials are wrong");
    }

    private Task<bool> IsUserNameAndPasswordNotNull(AdminLoginValidationModel adminLogin, CancellationToken cancellationToken)
    {
        var result = adminLogin.UserName is not null && adminLogin.Password is not null;
        return Task.FromResult(result);
    }

    private async Task<bool> UserExists(AdminLoginValidationModel adminLogin, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUserNameAsync(adminLogin.UserName);
        return user is not null;
    }

    private async Task<bool> IsAdmin(AdminLoginValidationModel adminLogin, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUserNameAsync(adminLogin.UserName);
        if (user is null)
        {
            return false;
        }

        var result = await _userRepository.IsInRoleAsync(user, UserRoles.Admin);
        return result.Match(x => x, e => false);
    }

    private async Task<bool> IsValidPassword(AdminLoginValidationModel adminLogin, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUserNameAsync(adminLogin.UserName);
        if (user is null)
        {
            return false;
        }

        return await _userRepository.CheckUserPasswordAsync(user, adminLogin.Password);
    }
}
