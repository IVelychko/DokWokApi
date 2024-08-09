﻿using FluentValidation;

namespace Application.Operations.Order.Commands.AddDeliveryOrder;

public class AddDeliveryOrderLineRequestValidator : AbstractValidator<AddDeliveryOrderLineRequest>
{
    public AddDeliveryOrderLineRequestValidator()
    {
        RuleFor(x => x.ProductId).NotNull();

        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}