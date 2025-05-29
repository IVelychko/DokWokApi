using Application.Mapping.Extensions;
using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.DTOs.Responses.Orders;
using Domain.Entities;
using Domain.Shared;
using Application.Extensions;
using Domain.Abstractions.Validation;
using Domain.DTOs.Requests.Orders;
using Domain.Specifications.Orders;

namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderServiceValidator _validator;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IOrderServiceValidator validator)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<OrderResponse> AddAsync(AddDeliveryOrderRequest request)
    {
        await ValidateAddDeliveryOrderRequestAsync(request);
        var response = await AddAsync(request.ToEntity());
        return response;
    }
    
    public async Task<OrderResponse> AddAsync(AddTakeawayOrderRequest request)
    {
        await ValidateAddTakeawayOrderRequestAsync(request);
        var response = await AddAsync(request.ToEntity());
        return response;
    }

    public async Task DeleteAsync(long id)
    {
        await ValidateDeleteOrderRequestAsync(id);
        var entityToDelete = await _orderRepository.GetByIdAsync(id);
        entityToDelete = Ensure.EntityExists(entityToDelete, "The order was not found");
        _orderRepository.Delete(entityToDelete);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IList<OrderResponse>> GetAllAsync()
    {
        var entities = await _orderRepository
            .GetAllBySpecificationAsync(OrderSpecification.IncludeAll);
        return entities.Select(p => p.ToResponse()).ToList();
    }

    public async Task<IList<OrderResponse>> GetAllByUserIdAsync(long userId)
    {
        OrderSpecification specification = new()
        {
            UserId = userId,
            IncludeOrderLines = true,
            IncludeProduct = true,
            IncludeCategory = true
        };
        var entities = await _orderRepository.GetAllBySpecificationAsync(specification);
        return entities.Select(p => p.ToResponse()).ToList();
    }

    public async Task<OrderResponse> GetByIdAsync(long id)
    {
        OrderSpecification specification = new()
        {
            Id = id,
            IncludeOrderLines = true,
            IncludeProduct = true,
            IncludeCategory = true
        };
        var entity = await _orderRepository.GetBySpecificationAsync(specification);
        entity = Ensure.EntityExists(entity, "The order was not found");
        return entity.ToResponse();
    }

    public async Task<OrderResponse> UpdateAsync(UpdateOrderRequest request)
    {
        await ValidateUpdateOrderRequestAsync(request);
        var entity = request.ToEntity();
        _orderRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        var updatedEntity = await GetOrderByIdWithDetailsAsync(
            entity.Id, "The updated order was not found");
        return updatedEntity.ToResponse();
    }
    
    private async Task<OrderResponse> AddAsync(Order entity)
    {
        await SetTotalLinePricesAsync(entity.OrderLines);
        entity.CreationDate = DateTime.UtcNow;
        entity.Status = OrderStatuses.BeingProcessed;
        SetTotalOrderPrice(entity);
        await _orderRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        var createdEntity = await GetOrderByIdWithDetailsAsync(
            entity.Id, "The created order was not found");
        return createdEntity.ToResponse();
    }
    
    private async Task ValidateAddDeliveryOrderRequestAsync(AddDeliveryOrderRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var validationResult = await _validator.ValidateAddDeliveryOrderAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task ValidateAddTakeawayOrderRequestAsync(AddTakeawayOrderRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var validationResult = await _validator.ValidateAddTakeawayOrderAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task ValidateDeleteOrderRequestAsync(long id)
    {
        DeleteOrderRequest request = new(id);
        var validationResult = await _validator.ValidateDeleteOrderAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task ValidateUpdateOrderRequestAsync(UpdateOrderRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var validationResult = await _validator.ValidateUpdateOrderAsync(request);
        validationResult.ThrowIfValidationFailed();
    }

    private async Task SetTotalLinePricesAsync(ICollection<OrderLine> orderLines)
    {
        foreach (var line in orderLines)
        {
            var product = await GetRelatedProductByIdAsync(line.ProductId);
            if (line.TotalLinePrice == 0)
            {
                line.TotalLinePrice = product.Price * line.Quantity;
            }
        }
    }
    
    private static void SetTotalOrderPrice(Order entity)
    {
        if (entity.OrderLines.Count > 0)
        {
            entity.TotalOrderPrice = entity.OrderLines.Sum(ol => ol.TotalLinePrice);
        }
    }
    
    private async Task<Product> GetRelatedProductByIdAsync(long id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        product = Ensure.EntityExists(product, "The related product was not found");
        return product;
    }
    
    private async Task<Order> GetOrderByIdWithDetailsAsync(long id, string errorMessage)
    {
        OrderSpecification specification = new()
        {
            Id = id,
            IncludeOrderLines = true,
            IncludeProduct = true,
            IncludeCategory = true
        };
        var createdEntity = await _orderRepository.GetBySpecificationAsync(specification);
        return Ensure.EntityExists(createdEntity, errorMessage);
    }
}
