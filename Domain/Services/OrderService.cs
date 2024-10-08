using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Helpers;
using Domain.Mapping.Extensions;
using Domain.Models;
using System.Linq.Expressions;

namespace Domain.Services;

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

    public async Task<Result<OrderModel>> AddAsync(OrderModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("orderModel", "The passed model is null");
            return Result<OrderModel>.Failure(error);
        }

        if (model.OrderLines.Count > 0)
        {
            var totalPriceResult = await SetTotalLinePricesAsync(model.OrderLines);
            if (totalPriceResult.IsFaulted)
            {
                return Result<OrderModel>.Failure(totalPriceResult.Error);
            }
        }

        model.CreationDate = DateTime.UtcNow;
        model.Status = OrderStatuses.BeingProcessed;
        model.SetTotalOrderPrice();

        var entity = model.ToEntity();
        await _orderRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        var createdEntity = await _orderRepository.GetByIdWithDetailsAsync(entity.Id) ?? throw new DbException("There was a database error");

        await _cacheService.RemoveAsync("allOrders");
        await _cacheService.RemoveByPrefixAsync("paginatedAllOrders");
        if (createdEntity.UserId.HasValue)
        {
            await _cacheService.RemoveAsync($"allOrdersByUserId{createdEntity.UserId.Value}");
        }

        return createdEntity.ToModel();
    }

    public async Task DeleteAsync(long id)
    {
        var entityToDelete = await _orderRepository.GetByIdAsync(id) ?? throw new DbException("There was a database error");
        await _orderRepository.DeleteByIdAsync(id);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync("allOrders");
        await _cacheService.RemoveByPrefixAsync("paginatedAllOrders");
        await _cacheService.RemoveAsync($"orderById{id}");
        if (entityToDelete.UserId.HasValue)
        {
            await _cacheService.RemoveAsync($"allOrdersByUserId{entityToDelete.UserId.Value}");
        }
    }

    public async Task<IEnumerable<OrderModel>> GetAllAsync(PageInfo? pageInfo = null)
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
        var entities = await Caching.GetCollectionFromCache(_cacheService,
            key, specification, _orderRepository.GetAllBySpecificationAsync);

        return entities.Select(p => p.ToModel());
    }

    public async Task<IEnumerable<OrderModel>> GetAllByUserIdAsync(long userId, PageInfo? pageInfo = null)
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

        return entities.Select(p => p.ToModel());
    }

    public async Task<OrderModel?> GetByIdAsync(long id)
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

        var entity = await Caching.GetEntityFromCache(_cacheService,
            key, specification, _orderRepository.GetAllBySpecificationAsync);

        return entity?.ToModel();
    }

    public async Task<Result<OrderModel>> UpdateAsync(OrderModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("orderModel", "The passed model is null");
            return Result<OrderModel>.Failure(error);
        }

        var entityToUpdate = await _orderRepository.GetByIdAsync(model.Id) ?? throw new DbException("There was a database error");
        await _cacheService.RemoveAsync("allOrders");
        await _cacheService.RemoveAsync($"orderById{entityToUpdate.Id}");
        await _cacheService.RemoveByPrefixAsync("paginatedAllOrders");
        if (entityToUpdate.UserId.HasValue)
        {
            await _cacheService.RemoveAsync($"allOrdersByUserId{entityToUpdate.UserId.Value}");
        }

        var entity = model.ToEntity();
        _orderRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        var updatedEntity = await _orderRepository.GetByIdWithDetailsAsync(entity.Id) ?? throw new DbException("There was a database error");

        return updatedEntity.ToModel();
    }

    private async Task<Result<List<OrderLineModel>>> SetTotalLinePricesAsync(List<OrderLineModel> orderLines)
    {
        Dictionary<string, string[]> errors = [];
        foreach (var line in orderLines)
        {
            var product = await _productRepository.GetByIdAsync(line.ProductId);
            if (product is null)
            {
                errors.Add(nameof(product), [$"Incorrect order line data. There is no product with the id: {line.ProductId}"]);
            }
            else if (errors.Count < 1 && line.TotalLinePrice == 0)
            {
                line.TotalLinePrice = product.Price * line.Quantity;
            }
        }

        return errors.Count < 1 ? orderLines : Result<List<OrderLineModel>>.Failure(new ValidationError(errors));
    }
}
