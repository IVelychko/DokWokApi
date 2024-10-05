using Application.Abstractions.Messaging;
using Domain.Helpers;

namespace Application.Operations.Product.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    long Id,
    string Name,
    decimal Price,
    decimal Weight,
    string MeasurementUnit,
    string Description,
    long CategoryId
) : ICommand<Result<ProductResponse>>;
