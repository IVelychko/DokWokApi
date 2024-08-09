namespace Infrastructure.Validation.Shops.Update;

public sealed record UpdateShopValidationModel(
    long Id,
    string Street,
    string Building,
    string OpeningTime,
    string ClosingTime
);
