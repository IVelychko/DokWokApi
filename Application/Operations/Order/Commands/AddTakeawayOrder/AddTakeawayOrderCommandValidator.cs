using Domain.Abstractions.Repositories;
using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.Order.Commands.AddTakeawayOrder;

public class AddTakeawayOrderCommandValidator : AbstractValidator<AddTakeawayOrderCommand>
{
    private readonly IShopRepository _shopRepository;
    private readonly IUserRepository _userRepository;

    public AddTakeawayOrderCommandValidator(IProductRepository productRepository, IShopRepository shopRepository, IUserRepository userRepository)
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
            .Matches(RegularExpressions.Guid)
            .MustAsync(UserExists)
            .WithMessage("There is no user with the ID specified in the UserId property of the Order entity")
            .When(x => x.UserId is not null);

        RuleForEach(x => x.OrderLines).SetValidator(new AddTakeawayOrderLineRequestValidator(productRepository))
            .When(x => x.OrderLines is not null && x.OrderLines.Count > 0);
    }

    private async Task<bool> ShopExists(long shopId, CancellationToken token) =>
        (await _shopRepository.GetByIdAsync(shopId)) is not null;

    private async Task<bool> UserExists(string? userId, CancellationToken token) =>
        (await _userRepository.GetUserByIdAsync(userId!)) is not null;
}
