using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.ProductCategory;
using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.BLL.Services;

public class ProductCategoryService : IProductCategoryService
{
    private readonly IProductCategoryRepository repository;

    private readonly IMapper mapper;

    public ProductCategoryService(IProductCategoryRepository repository, IMapper mapper)
    {
        this.repository = repository;
        this.mapper = mapper;
    }

    public async Task<ProductCategoryModel> AddAsync(ProductCategoryModel model)
    {
        ServiceHelper.CheckForNull(model, "The passed model is null.");
        var entity = mapper.Map<ProductCategory>(model);
        var addedEntity = await repository.AddAsync(entity);
        return mapper.Map<ProductCategoryModel>(addedEntity);
    }

    public async Task DeleteAsync(long id)
    {
        await repository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<ProductCategoryModel>> GetAllAsync()
    {
        var entities = await repository.GetAll().ToListAsync();
        var models = mapper.Map<IEnumerable<ProductCategoryModel>>(entities);
        return models;
    }

    public async Task<ProductCategoryModel?> GetByIdAsync(long id)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = mapper.Map<ProductCategoryModel>(entity);
        return model;
    }

    public async Task<ProductCategoryModel> UpdateAsync(ProductCategoryModel model)
    {
        ServiceHelper.CheckForNull(model, "The passed model is null.");

        var entity = mapper.Map<ProductCategory>(model);
        var updatedEntity = await repository.UpdateAsync(entity);
        return mapper.Map<ProductCategoryModel>(updatedEntity);
    }
}
