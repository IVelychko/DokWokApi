using Domain.Models;

namespace Application.Operations.OrderLine;

public class OrderLineResponse : BaseResponse<long>
{
    //public required long Id { get; set; }

    public required long OrderId { get; set; }

    public required long ProductId { get; set; }

    public required int Quantity { get; set; }

    public required decimal TotalLinePrice { get; set; }

    public required ProductModel? Product { get; set; }
}
