using Domain.Abstractions.Services;
using Domain.Abstractions.Repositories;
using Domain.Models;
using Domain.ResultType;
using Domain.Exceptions;
using Domain.Mapping.Extensions;

namespace Domain.Services;

public class ProductCategoryService : IProductCategoryService
{
    private readonly IProductCategoryRepository _productCategoryRepository;

    public ProductCategoryService(IProductCategoryRepository productCategoryRepository)
    {
        _productCategoryRepository = productCategoryRepository;
    }

    public async Task<Result<ProductCategoryModel>> AddAsync(ProductCategoryModel model)
    {
        if (model is null)
        {
            var exception = new ValidationException("The passed model is null.");
            return new Result<ProductCategoryModel>(exception);
        }

        var entity = model.ToEntity();
        var result = await _productCategoryRepository.AddAsync(entity);

        return result.Match(c => c.ToModel(),
            e => new Result<ProductCategoryModel>(e));
    }

    public async Task<bool?> DeleteAsync(long id)
    {
        return await _productCategoryRepository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<ProductCategoryModel>> GetAllAsync()
    {
        var entities = await _productCategoryRepository.GetAllAsync();
        var models = entities.Select(c => c.ToModel());
        return models;
    }

    public async Task<ProductCategoryModel?> GetByIdAsync(long id)
    {
        var entity = await _productCategoryRepository.GetByIdAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = entity.ToModel();
        return model;
    }

    public async Task<Result<ProductCategoryModel>> UpdateAsync(ProductCategoryModel model)
    {
        if (model is null)
        {
            var exception = new ValidationException("The passed model is null.");
            return new Result<ProductCategoryModel>(exception);
        }

        var entity = model.ToEntity();
        var result = await _productCategoryRepository.UpdateAsync(entity);

        return result.Match(c => c.ToModel(),
            e => new Result<ProductCategoryModel>(e));
    }

    public async Task<Result<bool>> IsNameTakenAsync(string name)
    {
        if (name is null)
        {
            var exception = new ValidationException("The passed name is null");
            return new Result<bool>(exception);
        }

        var isTakenResult = await _productCategoryRepository.IsNameTakenAsync(name);
        return isTakenResult;
    }
}
