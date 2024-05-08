using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models;
using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.BLL.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository repository;

    private readonly IMapper mapper;

    public ProductService(IProductRepository repository, IMapper mapper)
    {
        this.repository = repository;
        this.mapper = mapper;
    }

    private static void CheckForNull(ProductModel? model, string errorMessage)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model), errorMessage);
        }
    }

    public async Task<ProductModel> AddAsync(ProductModel model)
    {
        CheckForNull(model, "The passed model is null.");
        var entity = mapper.Map<Product>(model);
        var addedEntity = await repository.AddAsync(entity);
        var addedEntityWithDetails = await repository.GetByIdWithDetailsAsync(addedEntity.Id);
        return mapper.Map<ProductModel>(addedEntityWithDetails);
    }

    public async Task DeleteAsync(long id)
    {
        await repository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<ProductModel>> GetAllAsync()
    {
        var queryable = repository.GetAllWithDetails();
        var entities = await queryable.ToListAsync();
        var models = mapper.Map<IEnumerable<ProductModel>>(entities);
        return models;
    }

    public async Task<ProductModel?> GetByIdAsync(long id)
    {
        var entity = await repository.GetByIdWithDetailsAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = mapper.Map<ProductModel>(entity);
        return model;
    }

    public async Task<ProductModel> UpdateAsync(ProductModel model)
    {
        CheckForNull(model, "The passed model is null.");

        var entity = mapper.Map<Product>(model);
        var updatedEntity = await repository.UpdateAsync(entity);
        var updatedEntityWithDetails = await repository.GetByIdWithDetailsAsync(updatedEntity.Id);
        return mapper.Map<ProductModel>(updatedEntityWithDetails);
    }
}
