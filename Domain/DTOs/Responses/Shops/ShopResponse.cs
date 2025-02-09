namespace Domain.DTOs.Responses.Shops;

public class ShopResponse : BaseResponse
{
    public required string Street { get; set; }

    public required string Building { get; set; }

    public required string OpeningTime { get; set; }

    public required string ClosingTime { get; set; }
}
