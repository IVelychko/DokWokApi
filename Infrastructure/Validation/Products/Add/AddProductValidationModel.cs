namespace Infrastructure.Validation.Products.Add;

public sealed record AddProductValidationModel(
    string Name,
    decimal Price,
    decimal Weight,
    string MeasurementUnit,
    string Description,
    long CategoryId
);
