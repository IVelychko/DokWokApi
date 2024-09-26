using Domain.Abstractions.Repositories;
using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.Order.Commands.AddDeliveryOrder;

public sealed class AddDeliveryOrderCommandValidator : AbstractValidator<AddDeliveryOrderCommand>
{
    private readonly IUserRepository _userRepository;

    public AddDeliveryOrderCommandValidator(IProductRepository productRepository, IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.CustomerName).NotEmpty().Matches(RegularExpressions.FirstName).MinimumLength(2);

        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(RegularExpressions.PhoneNumber).MinimumLength(9);

        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.DeliveryAddress).NotEmpty().Matches(RegularExpressions.Address).MinimumLength(5);

        RuleFor(x => x.PaymentType).NotEmpty().Matches(RegularExpressions.PaymentType).MinimumLength(2);

        RuleFor(x => x.UserId)
            .NotEmpty()
            .Matches(RegularExpressions.Guid)
            .MustAsync(UserExists)
            .WithMessage("There is no user with the ID specified in the UserId property of the Order entity")
            .When(x => x.UserId is not null);

        RuleForEach(x => x.OrderLines).SetValidator(new AddDeliveryOrderLineRequestValidator(productRepository))
            .When(x => x.OrderLines is not null && x.OrderLines.Count > 0);
    }

    private async Task<bool> UserExists(string userId, CancellationToken token) =>
        (await _userRepository.GetUserByIdAsync(userId!)) is not null;
}
