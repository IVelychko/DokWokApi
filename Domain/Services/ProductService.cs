using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Errors;
using Domain.Mapping.Extensions;
using Domain.Models;
using Domain.ResultType;

namespace Domain.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<ProductModel>> AddAsync(ProductModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("The passed model is null.");
            return Result<ProductModel>.Failure(error);
        }

        var entity = model.ToEntity();
        var result = await _productRepository.AddAsync(entity);
        return result.Match(p => p.ToModel(), Result<ProductModel>.Failure);
    }

    public async Task<bool?> DeleteAsync(long id)
    {
        return await _productRepository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<ProductModel>> GetAllAsync()
    {
        var entities = await _productRepository.GetAllWithDetailsAsync();
        var models = entities.Select(p => p.ToModel());
        return models;
    }

    public async Task<IEnumerable<ProductModel>> GetAllByCategoryIdAsync(long categoryId)
    {
        var entities = await _productRepository.GetAllWithDetailsByCategoryIdAsync(categoryId);
        var models = entities.Select(p => p.ToModel());
        return models;
    }

    public async Task<ProductModel?> GetByIdAsync(long id)
    {
        var entity = await _productRepository.GetByIdWithDetailsAsync(id);
        return entity?.ToModel();
    }

    public async Task<Result<ProductModel>> UpdateAsync(ProductModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("The passed model is null.");
            return Result<ProductModel>.Failure(error);
        }

        var entity = model.ToEntity();
        var result = await _productRepository.UpdateAsync(entity);
        return result.Match(p => p.ToModel(), Result<ProductModel>.Failure);
    }

    public async Task<Result<bool>> IsNameTakenAsync(string name)
    {
        if (name is null)
        {
            var error = new ValidationError("The passed name is null");
            return Result<bool>.Failure(error);
        }

        var isTakenResult = await _productRepository.IsNameTakenAsync(name);
        return isTakenResult;
    }
}
