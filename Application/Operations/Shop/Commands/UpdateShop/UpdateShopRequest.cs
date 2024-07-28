namespace Application.Operations.Shop.Commands.UpdateShop;

public sealed record UpdateShopRequest(
    long Id,
    string Street,
    string Building,
    string OpeningTime,
    string ClosingTime
);
