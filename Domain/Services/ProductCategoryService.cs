using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Helpers;
using Domain.Mapping.Extensions;
using Domain.Models;

namespace Domain.Services;

public class ProductCategoryService : IProductCategoryService
{
    private readonly IProductCategoryRepository _productCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductCategoryService(IProductCategoryRepository productCategoryRepository, IUnitOfWork unitOfWork)
    {
        _productCategoryRepository = productCategoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductCategoryModel>> AddAsync(ProductCategoryModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("productCategoryModel", "The passed model is null.");
            return Result<ProductCategoryModel>.Failure(error);
        }

        var entity = model.ToEntity();
        await _productCategoryRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        var createdEntity = await _productCategoryRepository.GetByIdAsync(entity.Id) ?? throw new DbException("There was a database error");
        return createdEntity.ToModel();
    }

    public async Task DeleteAsync(long id)
    {
        await _productCategoryRepository.DeleteByIdAsync(id);
        await _unitOfWork.SaveChangesAsync();
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
        _productCategoryRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        var updatedEntity = await _productCategoryRepository.GetByIdAsync(entity.Id) ?? throw new DbException("There was a database error");
        return updatedEntity.ToModel();
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
