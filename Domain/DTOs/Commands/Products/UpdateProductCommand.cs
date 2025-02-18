using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Products;

namespace Domain.DTOs.Commands.Products;

public sealed record UpdateProductCommand(
    long Id,
    string Name,
    decimal Price,
    decimal Weight,
    string MeasurementUnit,
    string Description,
    long CategoryId
) : ICommand<ProductResponse>;
