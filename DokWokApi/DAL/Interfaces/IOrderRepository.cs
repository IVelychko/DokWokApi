using DokWokApi.DAL.Entities;

namespace DokWokApi.DAL.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    IQueryable<Order> GetAllWithDetails();

    Task<Order?> GetByIdWithDetailsAsync(long id);
}
