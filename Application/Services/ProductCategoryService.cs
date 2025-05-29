using Application.Extensions;
using Application.Mapping.Extensions;
using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Abstractions.Validation;
using Domain.DTOs.Requests.ProductCategories;
using Domain.DTOs.Responses.ProductCategories;
using Domain.Shared;

namespace Application.Services;

public class ProductCategoryService : IProductCategoryService
{
    private readonly IProductCategoryRepository _productCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductCategoryServiceValidator _validator;

    public ProductCategoryService(
        IProductCategoryRepository productCategoryRepository,
        IUnitOfWork unitOfWork,
        IProductCategoryServiceValidator validator)
    {
        _productCategoryRepository = productCategoryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ProductCategoryResponse> AddAsync(AddProductCategoryRequest request)
    {
        await ValidateAddCategoryRequestAsync(request);
        var entity = request.ToEntity();
        await _productCategoryRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.ToResponse();
    }

    public async Task DeleteAsync(long id)
    {
        await ValidateDeleteCategoryRequestAsync(id);
        var entity = await _productCategoryRepository.GetByIdAsync(id);
        entity = Ensure.EntityExists(entity, "The category was not found");
        _productCategoryRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IList<ProductCategoryResponse>> GetAllAsync()
    {
        var entities = await _productCategoryRepository.GetAllAsync();
        return entities.Select(p => p.ToResponse()).ToList();
    }

    public async Task<ProductCategoryResponse> GetByIdAsync(long id)
    {
        var entity = await _productCategoryRepository.GetByIdAsync(id);
        entity = Ensure.EntityExists(entity, "The category was not found");
        return entity.ToResponse();
    }

    public async Task<ProductCategoryResponse> UpdateAsync(UpdateProductCategoryRequest request)
    {
        await ValidateUpdateCategoryRequestAsync(request);
        var entity = request.ToEntity();
        _productCategoryRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.ToResponse();
    }

    public async Task<bool> IsNameUniqueAsync(string name)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(name, nameof(name));
        return await _productCategoryRepository.IsNameUniqueAsync(name);
    }
    
    private async Task ValidateUpdateCategoryRequestAsync(UpdateProductCategoryRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var validationResult = await _validator.ValidateUpdateCategoryAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task ValidateAddCategoryRequestAsync(AddProductCategoryRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var validationResult = await _validator.ValidateAddCategoryAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task ValidateDeleteCategoryRequestAsync(long id)
    {
        DeleteProductCategoryRequest request = new(id);
        var validationResult = await _validator.ValidateDeleteCategoryAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
}
