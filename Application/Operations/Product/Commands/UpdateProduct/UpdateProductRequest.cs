namespace Application.Operations.Product.Commands.UpdateProduct;

public sealed record class UpdateProductRequest(
    long Id,
    string Name,
    decimal Price,
    decimal Weight,
    string MeasurementUnit,
    string Description,
    long CategoryId
);
