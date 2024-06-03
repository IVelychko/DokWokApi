using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.ProductCategory;
using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.BLL.Services;

public class ProductCategoryService : IProductCategoryService
{
    private readonly IProductCategoryRepository _repository;

    private readonly IMapper _mapper;

    public ProductCategoryService(IProductCategoryRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ProductCategoryModel> AddAsync(ProductCategoryModel model)
    {
        ServiceHelper.CheckForNull(model, "The passed model is null.");
        var entity = _mapper.Map<ProductCategory>(model);
        var addedEntity = await _repository.AddAsync(entity);
        return _mapper.Map<ProductCategoryModel>(addedEntity);
    }

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<ProductCategoryModel>> GetAllAsync()
    {
        var entities = await _repository.GetAll().OrderBy(c => c.Id).ToListAsync();
        var models = _mapper.Map<IEnumerable<ProductCategoryModel>>(entities);
        return models;
    }

    public async Task<ProductCategoryModel?> GetByIdAsync(long id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = _mapper.Map<ProductCategoryModel>(entity);
        return model;
    }

    public async Task<ProductCategoryModel> UpdateAsync(ProductCategoryModel model)
    {
        ServiceHelper.CheckForNull(model, "The passed model is null.");

        var entity = _mapper.Map<ProductCategory>(model);
        var updatedEntity = await _repository.UpdateAsync(entity);
        return _mapper.Map<ProductCategoryModel>(updatedEntity);
    }
}
