using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.Order;

public class OrderLinePutModel
{
    [Required]
    public long? Id { get; set; }

    [Required]
    public long? OrderId { get; set; }

    [Required]
    public long? ProductId { get; set; }

    [Required]
    public int? Quantity { get; set; }

    [Required]
    public decimal? TotalLinePrice { get; set; }
}
