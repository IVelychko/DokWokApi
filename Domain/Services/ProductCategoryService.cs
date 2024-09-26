using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Errors;
using Domain.Mapping.Extensions;
using Domain.Models;
using Domain.ResultType;

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
            var error = new ValidationError("productCategoryModel", "The passed model is null.");
            return Result<ProductCategoryModel>.Failure(error);
        }

        var entity = model.ToEntity();
        var result = await _productCategoryRepository.AddAsync(entity);
        return result.Match(c => c.ToModel(),
            Result<ProductCategoryModel>.Failure);
    }

    public async Task<bool?> DeleteAsync(long id)
    {
        return await _productCategoryRepository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<ProductCategoryModel>> GetAllAsync(PageInfo? pageInfo = null)
    {
        var entities = await _productCategoryRepository.GetAllAsync(pageInfo);
        var models = entities.Select(c => c.ToModel());
        return models;
    }

    public async Task<ProductCategoryModel?> GetByIdAsync(long id)
    {
        var entity = await _productCategoryRepository.GetByIdAsync(id);
        return entity?.ToModel();
    }

    public async Task<Result<ProductCategoryModel>> UpdateAsync(ProductCategoryModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("productCategoryModel", "The passed model is null.");
            return Result<ProductCategoryModel>.Failure(error);
        }

        var entity = model.ToEntity();
        var result = await _productCategoryRepository.UpdateAsync(entity);
        return result.Match(c => c.ToModel(), Result<ProductCategoryModel>.Failure);
    }

    public async Task<Result<bool>> IsNameTakenAsync(string name)
    {
        if (name is null)
        {
            var error = new ValidationError(nameof(name), "The passed name is null");
            return Result<bool>.Failure(error);
        }

        var isTakenResult = await _productCategoryRepository.IsNameTakenAsync(name);
        return isTakenResult;
    }
}
