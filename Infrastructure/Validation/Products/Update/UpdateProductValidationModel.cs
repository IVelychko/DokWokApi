namespace Infrastructure.Validation.Products.Update;

public sealed record UpdateProductValidationModel(
    long Id,
    string Name,
    decimal Price,
    decimal Weight,
    string MeasurementUnit,
    string Description,
    long CategoryId
);
