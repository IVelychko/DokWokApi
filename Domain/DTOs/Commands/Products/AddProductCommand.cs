using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Products;
using Domain.Shared;

namespace Domain.DTOs.Commands.Products;

public sealed record AddProductCommand(
    string Name,
    decimal Price,
    decimal Weight,
    string MeasurementUnit,
    string Description,
    long CategoryId
) : ICommand<Result<ProductResponse>>;
