namespace Domain.DTOs.Requests.Shops;

public sealed record UpdateShopRequest(
    long Id,
    string Street,
    string Building,
    string OpeningTime,
    string ClosingTime
);
