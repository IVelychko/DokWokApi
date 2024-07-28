namespace Application.Operations.Shop.Commands.AddShop;

public sealed record AddShopRequest(
    string Street, 
    string Building, 
    string OpeningTime, 
    string ClosingTime
);
