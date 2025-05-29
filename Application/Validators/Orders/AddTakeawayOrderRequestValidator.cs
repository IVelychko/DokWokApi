using Domain.Abstractions.Repositories;
using Domain.Constants;
using Domain.DTOs.Requests.Orders;
using FluentValidation;

namespace Application.Validators.Orders;

public class AddTakeawayOrderRequestValidator : AbstractValidator<AddTakeawayOrderRequest>
{
    private readonly IShopRepository _shopRepository;
    private readonly IUserRepository _userRepository;

    public AddTakeawayOrderRequestValidator(IProductRepository productRepository, IShopRepository shopRepository, IUserRepository userRepository)
    {
        _shopRepository = shopRepository;
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

        RuleFor(x => x.ShopId)
            .NotEmpty()
            .MustAsync(ShopExists)
            .WithMessage("There is no shop with the ID specified in the ShopId property of the Order entity");

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
            .SetValidator(new AddTakeawayOrderLineRequestValidator(productRepository))
            .When(x => x.OrderLines.Count > 0);
    }

    private async Task<bool> ShopExists(long shopId, CancellationToken token)
    {
        return await _shopRepository.ShopExistsAsync(shopId);
    }

    private async Task<bool> UserExists(long? userId, CancellationToken token)
    {
        return await _userRepository.UserExistsAsync(userId.GetValueOrDefault());
    }
}
