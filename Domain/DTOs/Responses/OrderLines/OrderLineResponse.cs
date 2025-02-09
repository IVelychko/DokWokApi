using Domain.DTOs.Responses.Products;

namespace Domain.DTOs.Responses.OrderLines;

public class OrderLineResponse : BaseResponse
{
    public required long OrderId { get; set; }

    public required long ProductId { get; set; }

    public required int Quantity { get; set; }

    public required decimal TotalLinePrice { get; set; }

    public required ProductResponse? Product { get; set; }
}
