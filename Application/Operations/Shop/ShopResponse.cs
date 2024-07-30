namespace Application.Operations.Shop;

public class ShopResponse : BaseResponse<long>
{
    public required string Street { get; set; }

    public required string Building { get; set; }

    public required string OpeningTime { get; set; }

    public required string ClosingTime { get; set; }
}
