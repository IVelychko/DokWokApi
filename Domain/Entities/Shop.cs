namespace Domain.Entities;

public class Shop : BaseEntity
{
    public string Street { get; set; } = string.Empty;

    public string Building { get; set; } = string.Empty;

    public string OpeningTime { get; set; } = string.Empty;

    public string ClosingTime { get; set; } = string.Empty;

    public ICollection<Order> Orders { get; set; } = [];
}
