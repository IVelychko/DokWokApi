using Application.Abstractions.Messaging;
using Domain.Models;
using Domain.ResultType;

namespace Application.Operations.Product.Commands.AddProduct;

public sealed record AddProductCommand(
    string Name,
    decimal Price,
    decimal Weight,
    string MeasurementUnit,
    string Description,
    long CategoryId
) : ICommand<Result<ProductModel>>;
