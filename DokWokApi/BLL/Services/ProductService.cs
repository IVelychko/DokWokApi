using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Product;
using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.BLL.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    private readonly IMapper _mapper;

    public ProductService(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ProductModel> AddAsync(ProductModel model)
    {
        ServiceHelper.CheckForNull(model, "The passed model is null.");
        var entity = _mapper.Map<Product>(model);
        var addedEntity = await _repository.AddAsync(entity);
        var addedEntityWithDetails = await _repository.GetByIdWithDetailsAsync(addedEntity.Id);
        return _mapper.Map<ProductModel>(addedEntityWithDetails);
    }

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<ProductModel>> GetAllAsync()
    {
        var queryable = _repository.GetAllWithDetails().OrderBy(p => p.Id);
        var entities = await queryable.ToListAsync();
        var models = _mapper.Map<IEnumerable<ProductModel>>(entities);
        return models;
    }

    public async Task<IEnumerable<ProductModel>> GetAllByCategoryIdAsync(long categoryId)
    {
        var entities = _repository.GetAllWithDetails();
        var filteredEntities = entities.Where(p => p.CategoryId == categoryId);
        var list = await filteredEntities.ToListAsync();
        var models = _mapper.Map<IEnumerable<ProductModel>>(list);
        return models;
    }

    public async Task<ProductModel?> GetByIdAsync(long id)
    {
        var entity = await _repository.GetByIdWithDetailsAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = _mapper.Map<ProductModel>(entity);
        return model;
    }

    public async Task<ProductModel> UpdateAsync(ProductModel model)
    {
        ServiceHelper.CheckForNull(model, "The passed model is null.");

        var entity = _mapper.Map<Product>(model);
        var updatedEntity = await _repository.UpdateAsync(entity);
        var updatedEntityWithDetails = await _repository.GetByIdWithDetailsAsync(updatedEntity.Id);
        return _mapper.Map<ProductModel>(updatedEntityWithDetails);
    }
}
