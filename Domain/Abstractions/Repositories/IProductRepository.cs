using Domain.Entities;
using Domain.ResultType;

namespace Domain.Abstractions.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetAllWithDetailsAsync();

    Task<IEnumerable<Product>> GetAllWithDetailsByCategoryIdAsync(long categoryId);

    Task<Product?> GetByIdWithDetailsAsync(long id);

    Task<Result<bool>> IsNameTakenAsync(string name);
}
