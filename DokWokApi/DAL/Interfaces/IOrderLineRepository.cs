using DokWokApi.DAL.Entities;

namespace DokWokApi.DAL.Interfaces;

public interface IOrderLineRepository : IRepository<OrderLine>
{
    IQueryable<OrderLine> GetAllWithDetails();

    Task<OrderLine?> GetByIdWithDetailsAsync(long id);

    Task<OrderLine?> GetByOrderAndProductIdsAsync(long orderId, long productId);

    Task<OrderLine?> GetByOrderAndProductIdsWithDetailsAsync(long orderId, long productId);
}
