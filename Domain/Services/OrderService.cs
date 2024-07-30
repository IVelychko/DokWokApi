using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Errors;
using Domain.Helpers;
using Domain.Mapping.Extensions;
using Domain.Models;
using Domain.ResultType;

namespace Domain.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<Result<OrderModel>> AddAsync(OrderModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("The passed model is null");
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
        var result = await _orderRepository.AddAsync(entity);
        return result.Match(o => o.ToModel(), Result<OrderModel>.Failure);
    }

    public async Task<bool?> DeleteAsync(long id)
    {
        return await _orderRepository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<OrderModel>> GetAllAsync()
    {
        var entities = await _orderRepository.GetAllWithDetailsAsync();
        var models = entities.Select(o => o.ToModel());
        return models;
    }

    public async Task<IEnumerable<OrderModel>> GetAllByUserIdAsync(string userId)
    {
        var entities = await _orderRepository.GetAllWithDetailsByUserIdAsync(userId);
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
            var error = new ValidationError("The passed model is null");
            return Result<OrderModel>.Failure(error);
        }

        var entity = model.ToEntity();
        var result = await _orderRepository.UpdateAsync(entity);
        return result.Match(o => o.ToModel(), Result<OrderModel>.Failure);
    }

    private async Task<Result<List<OrderLineModel>>> SetTotalLinePricesAsync(List<OrderLineModel> orderLines)
    {
        List<string> errors = [];
        foreach (var line in orderLines)
        {
            var product = await _productRepository.GetByIdAsync(line.ProductId);
            if (product is null)
            {
                errors.Add($"Incorrect order line data. There is no product with the id: {line.ProductId}");
            }
            else if (errors.Count < 1)
            {
                line.TotalLinePrice = product.Price * line.Quantity;
            }
        }

        return errors.Count < 1 ? orderLines : Result<List<OrderLineModel>>.Failure(new ValidationError(errors));
    }
}
