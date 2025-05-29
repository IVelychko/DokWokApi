using Application.Extensions;
using Application.Mapping.Extensions;
using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Abstractions.Validation;
using Domain.DTOs.Requests.OrderLines;
using Domain.DTOs.Responses.OrderLines;
using Domain.Entities;
using Domain.Shared;
using Domain.Specifications.OrderLines;
using Domain.Specifications.Products;

namespace Application.Services;

public class OrderLineService : IOrderLineService
{
    private readonly IOrderLineRepository _orderLineRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderLineServiceValidator _validator;

    public OrderLineService(
        IOrderLineRepository orderLineRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IOrderLineServiceValidator validator)
    {
        _orderLineRepository = orderLineRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<OrderLineResponse> AddAsync(AddOrderLineRequest request)
    {
        await ValidateAddOrderLineRequestAsync(request);
        var entity = request.ToEntity();
        var product = await GetRelatedProductByIdAsync(entity.ProductId);
        entity.TotalLinePrice = product.Price * entity.Quantity;
        await _orderLineRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        entity.Product = product;
        return entity.ToResponse();
    }

    public async Task DeleteAsync(long id)
    {
        await ValidateDeleteOrderLineRequestAsync(id);
        var entityToDelete = await _orderLineRepository.GetByIdAsync(id);
        entityToDelete = Ensure.EntityExists(entityToDelete, "The order line to delete was not found");
        _orderLineRepository.Delete(entityToDelete);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IList<OrderLineResponse>> GetAllAsync()
    {
        var entities = await _orderLineRepository
            .GetAllBySpecificationAsync(OrderLineSpecification.IncludeAll);
        return entities.Select(p => p.ToResponse()).ToList();
    }

    public async Task<IList<OrderLineResponse>> GetAllByOrderIdAsync(long orderId)
    {
        OrderLineSpecification specification = new()
        {
            IncludeProduct = true,
            IncludeCategory = true,
            OrderId = orderId,
        };
        var entities = await _orderLineRepository.GetAllBySpecificationAsync(specification);
        return entities.Select(p => p.ToResponse()).ToList();
    }

    public async Task<OrderLineResponse> GetByIdAsync(long id)
    {
        var entity = await _orderLineRepository.GetByIdAsync(id);
        entity = Ensure.EntityExists(entity, "The order line was not found");
        entity.Product = await GetRelatedProductByIdAsync(entity.ProductId);
        return entity.ToResponse();
    }

    public async Task<OrderLineResponse> GetByOrderAndProductIdsAsync(long orderId, long productId)
    {
        var entity = await _orderLineRepository.GetByOrderAndProductIdsAsync(orderId, productId);
        entity = Ensure.EntityExists(entity, "The order line was not found");
        entity.Product = await GetRelatedProductByIdAsync(entity.ProductId);
        return entity.ToResponse();
    }

    public async Task<OrderLineResponse> UpdateAsync(UpdateOrderLineRequest request)
    {
        await ValidateUpdateOrderLineRequestAsync(request);
        var entity = request.ToEntity();
        var product = await GetRelatedProductByIdAsync(entity.ProductId);
        entity.TotalLinePrice = product.Price * entity.Quantity;
        _orderLineRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        entity.Product = product;
        return entity.ToResponse();
    }
    
    private async Task ValidateAddOrderLineRequestAsync(AddOrderLineRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var validationResult = await _validator.ValidateAddOrderLineAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task ValidateDeleteOrderLineRequestAsync(long id)
    {
        DeleteOrderLineRequest request = new(id);
        var validationResult = await _validator.ValidateDeleteOrderLineAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task ValidateUpdateOrderLineRequestAsync(UpdateOrderLineRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var validationResult = await _validator.ValidateUpdateOrderLineAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task<Product> GetRelatedProductByIdAsync(long id)
    {
        ProductSpecification specification = new()
        {
            IncludeCategory = true,
            Id = id
        };
        var product = await _productRepository.GetBySpecificationAsync(specification);
        product = Ensure.EntityExists(product, "The related product was not found");
        return product;
    }
}
