using Application.Extensions;
using Application.Mapping.Extensions;
using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Abstractions.Validation;
using Domain.DTOs.Requests.Products;
using Domain.DTOs.Responses;
using Domain.DTOs.Responses.Products;
using Domain.Entities;
using Domain.Shared;
using Domain.Specifications.Products;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IProductCategoryRepository _productCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductServiceValidator _validator;

    public ProductService(
        IProductRepository productRepository,
        IProductCategoryRepository productCategoryRepository,
        IUnitOfWork unitOfWork,
        IProductServiceValidator validator)
    {
        _productRepository = productRepository;
        _productCategoryRepository = productCategoryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ProductResponse> AddAsync(AddProductRequest request)
    {
        await ValidateAddProductRequestAsync(request);
        var entity = request.ToEntity();
        await _productRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        var category = await GetCategoryByIdAsync(entity.CategoryId);
        entity.Category = category;
        return entity.ToResponse();
    }

    public async Task DeleteAsync(long id)
    {
        await ValidateDeleteProductRequestAsync(id);
        var entityToDelete = await _productRepository.GetByIdAsync(id);
        entityToDelete = Ensure.EntityExists(entityToDelete, "The product was not found");
        _productRepository.Delete(entityToDelete);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IList<ProductResponse>> GetAllAsync()
    {
        var entities = await _productRepository.GetAllBySpecificationAsync(ProductSpecification.IncludeAll);
        return entities.Select(p => p.ToResponse()).ToList();
    }

    public async Task<IList<ProductResponse>> GetAllByCategoryIdAsync(long categoryId)
    {
        ProductSpecification specification = new()
        {
            IncludeCategory = true,
            CategoryId = categoryId
        };
        var entities = await _productRepository.GetAllBySpecificationAsync(specification);
        return entities.Select(p => p.ToResponse()).ToList();
    }

    public async Task<ProductResponse> GetByIdAsync(long id)
    {
        var entity = await _productRepository.GetByIdAsync(id);
        entity = Ensure.EntityExists(entity, "The product was not found");
        var category = await GetCategoryByIdAsync(entity.CategoryId);
        entity.Category = category;
        return entity.ToResponse();
    }

    public async Task<ProductResponse> UpdateAsync(UpdateProductRequest request)
    {
        await ValidateUpdateProductRequestAsync(request);
        var entity = request.ToEntity();
        _productRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        var category = await GetCategoryByIdAsync(entity.CategoryId);
        entity.Category = category;
        return entity.ToResponse();
    }

    public async Task<IsTakenResponse> IsNameTakenAsync(string name)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(name, nameof(name));
        var isUnique = await _productRepository.IsNameUniqueAsync(name);
        return new IsTakenResponse(!isUnique);
    }
    
    private async Task ValidateDeleteProductRequestAsync(long id)
    {
        DeleteProductRequest request = new(id);
        var validationResult = await _validator.ValidateDeleteProductAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task ValidateUpdateProductRequestAsync(UpdateProductRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var validationResult = await _validator.ValidateUpdateProductAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task ValidateAddProductRequestAsync(AddProductRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var validationResult = await _validator.ValidateAddProductAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task<ProductCategory> GetCategoryByIdAsync(long id)
    {
        var category = await _productCategoryRepository.GetByIdAsync(id);
        return Ensure.EntityExists(category, "The related category was not found");
    }
}
