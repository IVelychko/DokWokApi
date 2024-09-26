using Domain.Abstractions.Repositories;
using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.Order.Commands.UpdateOrder;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    private readonly IShopRepository _shopRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;

    public UpdateOrderCommandValidator(IShopRepository shopRepository, IOrderRepository orderRepository, IUserRepository userRepository)
    {
        _shopRepository = shopRepository;
        _orderRepository = orderRepository;
        _userRepository = userRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(OrderToUpdateExists)
            .WithErrorCode("404")
            .WithMessage("There is no order with this ID to update in the database");

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
            .MinimumLength(5)
            .When(x => x.DeliveryAddress is not null);

        RuleFor(x => x.TotalOrderPrice)
            .NotEmpty();

        RuleFor(x => x.CreationDate)
            .NotEmpty();

        RuleFor(x => x.Status)
            .NotEmpty()
            .MinimumLength(3);

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

        RuleFor(x => x.ShopId)
            .NotEmpty()
            .MustAsync(ShopExists)
            .WithMessage("There is no shop with the ID specified in the ShopId property of the Order entity")
            .When(x => x.ShopId is not null);
    }

    private async Task<bool> OrderToUpdateExists(long orderId, CancellationToken cancellationToken) =>
        (await _orderRepository.GetByIdAsync(orderId)) is not null;

    private async Task<bool> UserExists(string? userId, CancellationToken cancellationToken) =>
        (await _userRepository.GetUserByIdAsync(userId!)) is not null;

    private async Task<bool> ShopExists(long? shopId, CancellationToken cancellationToken) =>
        (await _shopRepository.GetByIdAsync(shopId.GetValueOrDefault())) is not null;
}
