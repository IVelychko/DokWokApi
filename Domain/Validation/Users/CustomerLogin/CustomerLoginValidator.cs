﻿using Domain.Abstractions.Repositories;
using Domain.Helpers;
using FluentValidation;

namespace Domain.Validation.Users.CustomerLogin;

public sealed class CustomerLoginValidator : AbstractValidator<CustomerLoginValidationModel>
{
    private readonly IUserRepository _userRepository;

    public CustomerLoginValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x)
            .MustAsync(IsUserNameAndPasswordNotNull)
            .WithName("user")
            .WithMessage("User name or password is null")
            .DependentRules(() =>
            {
                RuleFor(x => x)
                    .MustAsync(UserExists)
                    .WithName("user")
                    .WithErrorCode("404")
                    .WithMessage("The credentials are wrong.")
                    .DependentRules(() =>
                    {
                        RuleFor(x => x)
                            .MustAsync(IsCustomerOrAdmin)
                            .WithName("user")
                            .WithMessage("The authorization is denied")
                            .DependentRules(() =>
                            {
                                RuleFor(x => x)
                                    .MustAsync(IsValidPassword)
                                    .WithName("user")
                                    .WithMessage("The credentials are wrong");
                            });
                    });
            });
    }

    private Task<bool> IsUserNameAndPasswordNotNull(CustomerLoginValidationModel customerLogin, CancellationToken cancellationToken)
    {
        var result = customerLogin.UserName is not null && customerLogin.Password is not null;
        return Task.FromResult(result);
    }

    private async Task<bool> UserExists(CustomerLoginValidationModel customerLogin, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUserNameAsync(customerLogin.UserName);
        return user is not null;
    }

    private async Task<bool> IsCustomerOrAdmin(CustomerLoginValidationModel customerLogin, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUserNameAsync(customerLogin.UserName);
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

    private async Task<bool> IsValidPassword(CustomerLoginValidationModel customerLogin, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUserNameAsync(customerLogin.UserName);
        if (user is null)
        {
            return false;
        }

        return await _userRepository.CheckUserPasswordAsync(user, customerLogin.Password);
    }
}
