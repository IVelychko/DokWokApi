using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Helpers;
using Domain.Mapping.Extensions;
using Domain.Models;
using Domain.ResultType;

namespace Domain.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
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
        return createdEntity.ToModel();
    }

    public async Task DeleteAsync(long id)
    {
        await _orderRepository.DeleteByIdAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<OrderModel>> GetAllAsync(PageInfo? pageInfo = null)
    {
        var entities = await _orderRepository.GetAllWithDetailsAsync(pageInfo);
        var models = entities.Select(o => o.ToModel());
        return models;
    }

    public async Task<IEnumerable<OrderModel>> GetAllByUserIdAsync(long userId, PageInfo? pageInfo = null)
    {
        var entities = await _orderRepository.GetAllWithDetailsByUserIdAsync(userId, pageInfo);
        var models = entities.Select(o => o.ToModel());
        return models;
    }

    public async Task<OrderModel?> GetByIdAsync(long id)
    {
        var entity = await _orderRepository.GetByIdWithDetailsAsync(id);
        return entity?.ToModel();
    }

    public async Task<Result<OrderModel>> UpdateAsync(OrderModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("orderModel", "The passed model is null");
            return Result<OrderModel>.Failure(error);
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
