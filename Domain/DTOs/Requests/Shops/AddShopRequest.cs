namespace Domain.DTOs.Requests.Shops;

public sealed record AddShopRequest(
    string Street, 
    string Building, 
    string OpeningTime, 
    string ClosingTime
);
