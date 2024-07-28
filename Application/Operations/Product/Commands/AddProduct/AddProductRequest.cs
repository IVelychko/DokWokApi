namespace Application.Operations.Product.Commands.AddProduct;

public sealed record AddProductRequest(
    string Name,
    decimal Price,
    decimal Weight,
    string MeasurementUnit,
    string Description,
    long CategoryId
);
