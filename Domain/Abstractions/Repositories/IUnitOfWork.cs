namespace Domain.Abstractions.Repositories;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}
