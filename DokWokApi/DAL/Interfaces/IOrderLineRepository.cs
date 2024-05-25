using DokWokApi.DAL.Entities;

namespace DokWokApi.DAL.Interfaces;

public interface IOrderLineRepository : IRepository<OrderLine>
{
    IQueryable<OrderLine> GetAllWithDetails();

    Task<OrderLine?> GetByIdWithDetailsAsync(long id);
}
