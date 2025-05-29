using Domain.Abstractions.Repositories;
using Domain.Constants;
using Domain.DTOs.Requests.Orders;
using FluentValidation;

namespace Application.Validators.Orders;

public sealed class AddDeliveryOrderRequestValidator : AbstractValidator<AddDeliveryOrderRequest>
{
    private readonly IUserRepository _userRepository;

    public AddDeliveryOrderRequestValidator(IProductRepository productRepository, IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .Matches(RegularExpressions.FirstName)
            .MinimumLength(2);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(RegularExpressions.PhoneNumber)
            .MinimumLength(9);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.DeliveryAddress)
            .NotEmpty()
            .Matches(RegularExpressions.Address)
            .MinimumLength(5);

        RuleFor(x => x.PaymentType)
            .NotEmpty()
            .Matches(RegularExpressions.PaymentType)
            .MinimumLength(2);

        RuleFor(x => x.UserId)
            .NotEmpty()
            .MustAsync(UserExists)
            .WithMessage("There is no user with the ID specified in the UserId property of the Order entity")
            .When(x => x.UserId is not null);

        RuleForEach(x => x.OrderLines)
            .SetValidator(new AddDeliveryOrderLineRequestValidator(productRepository))
            .When(x => x.OrderLines.Count > 0);
    }
    
    private async Task<bool> UserExists(long? userId, CancellationToken token)
    {
        return await _userRepository.UserExistsAsync(userId.GetValueOrDefault());
    }
}
