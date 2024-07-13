using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.ProductCategory;
using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using DokWokApi.Exceptions;
using LanguageExt.Common;
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

    public async Task<Result<ProductCategoryModel>> AddAsync(ProductCategoryModel model)
    {
        if (model is null)
        {
            var exception = new ValidationException("The passed model is null.");
            return new Result<ProductCategoryModel>(exception);
        }

        var entity = _mapper.Map<ProductCategory>(model);
        var result = await _repository.AddAsync(entity);

        return result.Match(c => _mapper.Map<ProductCategoryModel>(c),
            e => new Result<ProductCategoryModel>(e));
    }

    public async Task<bool?> DeleteAsync(long id)
    {
        return await _repository.DeleteByIdAsync(id);
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

    public async Task<Result<ProductCategoryModel>> UpdateAsync(ProductCategoryModel model)
    {
        if (model is null)
        {
            var exception = new ValidationException("The passed model is null.");
            return new Result<ProductCategoryModel>(exception);
        }

        var entity = _mapper.Map<ProductCategory>(model);
        var result = await _repository.UpdateAsync(entity);

        return result.Match(c => _mapper.Map<ProductCategoryModel>(c),
            e => new Result<ProductCategoryModel>(e));
    }

    public async Task<Result<bool>> IsNameTaken(string name)
    {
        if (name is null)
        {
            var exception = new ValidationException("The passed name is null");
            return new Result<bool>(exception);
        }

        var category = await _repository.GetAll().FirstOrDefaultAsync(c => c.Name == name);
        return category is not null;
    }
}
