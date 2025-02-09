using Application.Mapping.Extensions;
using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Orders;
using Domain.DTOs.Responses.Orders;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Models;
using Domain.Shared;
using System.Linq.Expressions;

namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<OrderResponse> AddAsync(AddDeliveryOrderCommand command)
    {
        Ensure.ArgumentNotNull(command);
        OrderResponse response = await AddAsync(command.ToEntity());
        return response;
    }
    
    public async Task<OrderResponse> AddAsync(AddTakeawayOrderCommand command)
    {
        Ensure.ArgumentNotNull(command);
        OrderResponse response = await AddAsync(command.ToEntity());
        return response;
    }

    public async Task DeleteAsync(long id)
    {
        Order? entityToDelete = await _orderRepository.GetByIdAsync(id);
        entityToDelete = Ensure.EntityFound(entityToDelete, "The order was not found");
        _orderRepository.Delete(entityToDelete);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync("allOrders");
        await _cacheService.RemoveByPrefixAsync("paginatedAllOrders");
        await _cacheService.RemoveAsync($"orderById{id}");
        if (entityToDelete.UserId.HasValue)
        {
            await _cacheService.RemoveAsync($"allOrdersByUserId{entityToDelete.UserId.Value}");
        }
    }

    public async Task<IEnumerable<OrderResponse>> GetAllAsync(PageInfo? pageInfo = null)
    {
        string key = pageInfo is null ? "allOrders" :
            $"paginatedAllOrders-page{pageInfo.Number}-size{pageInfo.Size}";

        Specification<Order> specification = new() { PageInfo = pageInfo };
        IncludeExpression<Order> includeOrderLine = new(o => o.OrderLines);
        Expression<Func<object?, object?>> thenIncludeProduct = ol => (ol as OrderLine)!.Product;
        Expression<Func<object?, object?>> thenIncludeProductCategory = p => (p as Product)!.Category;
        includeOrderLine.ThenIncludes.AddRange([thenIncludeProduct, thenIncludeProductCategory]);
        specification.IncludeExpressions.AddRange([
            new(o => o.User),
            new(o => o.Shop),
            includeOrderLine
            ]);
        IList<Order> entities = await Caching.GetCollectionFromCache(_cacheService,
            key, specification, _orderRepository.GetAllBySpecificationAsync);

        return entities.Select(p => p.ToResponse());
    }

    public async Task<IEnumerable<OrderResponse>> GetAllByUserIdAsync(long userId, PageInfo? pageInfo = null)
    {
        string key = pageInfo is null ? $"allOrdersByUserId{userId}" :
            $"paginatedAllOrdersByUserId{userId}-page{pageInfo.Number}-size{pageInfo.Size}";

        Specification<Order> specification = new()
        {
            Criteria = o => o.UserId == userId,
            PageInfo = pageInfo
        };
        IncludeExpression<Order> includeOrderLine = new(o => o.OrderLines);
        Expression<Func<object?, object?>> thenIncludeProduct = ol => (ol as OrderLine)!.Product;
        Expression<Func<object?, object?>> thenIncludeProductCategory = p => (p as Product)!.Category;
        includeOrderLine.ThenIncludes.AddRange([thenIncludeProduct, thenIncludeProductCategory]);
        specification.IncludeExpressions.AddRange([
            new(o => o.User),
            new(o => o.Shop),
            includeOrderLine
            ]);

        var entities = await Caching.GetCollectionFromCache(_cacheService,
            key, specification, _orderRepository.GetAllBySpecificationAsync);

        return entities.Select(p => p.ToResponse());
    }

    public async Task<OrderResponse?> GetByIdAsync(long id)
    {
        string key = $"orderById{id}";
        Specification<Order> specification = new() { Criteria = o => o.Id == id };
        IncludeExpression<Order> includeOrderLine = new(o => o.OrderLines);
        Expression<Func<object?, object?>> thenIncludeProduct = ol => (ol as OrderLine)!.Product;
        Expression<Func<object?, object?>> thenIncludeProductCategory = p => (p as Product)!.Category;
        includeOrderLine.ThenIncludes.AddRange([thenIncludeProduct, thenIncludeProductCategory]);
        specification.IncludeExpressions.AddRange([
            new(o => o.User),
            new(o => o.Shop),
            includeOrderLine
            ]);

        Order? entity = await Caching.GetEntityFromCache(_cacheService,
            key, specification, _orderRepository.GetAllBySpecificationAsync);

        return entity?.ToResponse();
    }

    public async Task<OrderResponse> UpdateAsync(UpdateOrderCommand command)
    {
        Ensure.ArgumentNotNull(command);
        Order entity = command.ToEntity();
        _orderRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        
        Order updatedEntity = await _orderRepository.GetByIdWithDetailsAsync(entity.Id) 
                              ?? throw new DbException("There was a database error");
        await _cacheService.RemoveAsync("allOrders");
        await _cacheService.RemoveAsync($"orderById{entity.Id}");
        await _cacheService.RemoveByPrefixAsync("paginatedAllOrders");
        await _cacheService.RemoveByPrefixAsync("allOrdersByUserId");
        
        return updatedEntity.ToResponse();
    }
    
    private async Task<OrderResponse> AddAsync(Order entity)
    {
        Ensure.ArgumentNotNull(entity);
        if (entity.OrderLines.Count > 0)
        {
            await SetTotalLinePricesAsync(entity.OrderLines);
        }

        entity.CreationDate = DateTime.UtcNow;
        entity.Status = OrderStatuses.BeingProcessed;
        SetTotalOrderPrice(entity);
        
        await _orderRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        Order createdEntity = await _orderRepository.GetByIdWithDetailsAsync(entity.Id) 
                              ?? throw new DbException("There was a database error");

        await _cacheService.RemoveAsync("allOrders");
        await _cacheService.RemoveByPrefixAsync("paginatedAllOrders");
        if (createdEntity.UserId.HasValue)
        {
            await _cacheService.RemoveAsync($"allOrdersByUserId{createdEntity.UserId.Value}");
        }

        return createdEntity.ToResponse();
    }

    private async Task SetTotalLinePricesAsync(IEnumerable<OrderLine> orderLines)
    {
        foreach (OrderLine line in orderLines)
        {
            Product? product = await _productRepository.GetByIdAsync(line.ProductId);
            product = Ensure.EntityFound(product, "The related product was not found");
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
}
