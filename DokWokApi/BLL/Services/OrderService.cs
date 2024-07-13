using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Order;
using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using DokWokApi.Exceptions;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.BLL.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartService _cartService;
    private readonly IMapper _mapper;

    public OrderService(IOrderRepository orderRepository, ICartService cartService, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _cartService = cartService;
        _mapper = mapper;
    }

    public async Task<Result<OrderModel>> AddAsync(OrderModel model)
    {
        if (model is null)
        {
            var exception = new ValidationException("The passed model is null.");
            return new Result<OrderModel>(exception);
        }

        var entity = _mapper.Map<Order>(model);
        var result = await _orderRepository.AddAsync(entity);

        return result.Match(o => _mapper.Map<OrderModel>(o),
            e => new Result<OrderModel>(e));
    }

    public async Task<Result<OrderModel>> AddOrderFromCartAsync(OrderModel model)
    {
        if (model is null)
        {
            var exception = new ValidationException("The passed model is null.");
            return new Result<OrderModel>(exception);
        }

        var cart = await _cartService.GetCart();
        if (cart is null)
        {
            var exception = new CartException("There was an error while getting the cart.");
            return new Result<OrderModel>(exception);
        }

        if (cart.Lines.Count < 1)
        {
            var exception = new CartException("There are no products in the cart");
            return new Result<OrderModel>(exception);
        }
        
        var orderLines = _mapper.Map<List<OrderLineModel>>(cart.Lines);
        model.CreationDate = DateTime.UtcNow;
        model.OrderLines = orderLines;
        model.TotalOrderPrice = cart.TotalCartPrice;
        model.Status = OrderStatuses.BeingProcessed;

        var addedModel = await AddAsync(model);
        await _cartService.ClearCart();
        return addedModel;
    }

    public async Task<bool?> DeleteAsync(long id)
    {
        return await _orderRepository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<OrderModel>> GetAllAsync()
    {
        var queryable = _orderRepository.GetAllWithDetails();
        var entities = await queryable.ToListAsync();
        var models = _mapper.Map<IEnumerable<OrderModel>>(entities);
        return models;
    }

    public async Task<IEnumerable<OrderModel>> GetAllByUserIdAsync(string userId)
    {
        var entities = _orderRepository.GetAllWithDetails();
        var filteredEntities = entities.Where(o => o.UserId == userId);
        var list = await filteredEntities.ToListAsync();
        var models = _mapper.Map<IEnumerable<OrderModel>>(list);
        return models;
    }

    public async Task<OrderModel?> GetByIdAsync(long id)
    {
        var entity = await _orderRepository.GetByIdWithDetailsAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = _mapper.Map<OrderModel>(entity);
        return model;
    }

    public async Task<Result<OrderModel>> UpdateAsync(OrderModel model)
    {
        if (model is null)
        {
            var exception = new ValidationException("The passed model is null.");
            return new Result<OrderModel>(exception);
        }

        var entity = _mapper.Map<Order>(model);
        var result = await _orderRepository.UpdateAsync(entity);

        return result.Match(o => _mapper.Map<OrderModel>(o),
            e => new Result<OrderModel>(e));
    }
}
