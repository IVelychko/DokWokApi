namespace Infrastructure.Validation.Shops.Add;

public sealed record AddShopValidationModel(
    string Street,
    string Building,
    string OpeningTime,
    string ClosingTime
);
