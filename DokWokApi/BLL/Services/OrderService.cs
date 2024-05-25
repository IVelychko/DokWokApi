using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Order;
using DokWokApi.BLL.Models.Product;
using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.BLL.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository orderRepository;

    private readonly ICartService cartService;

    private readonly IMapper mapper;

    public OrderService(IOrderRepository orderRepository, ICartService cartService, IMapper mapper)
    {
        this.orderRepository = orderRepository;
        this.cartService = cartService;
        this.mapper = mapper;
    }

    public async Task<OrderModel> AddAsync(OrderForm model)
    {
        ServiceHelper.CheckForNull(model, "The passed model is null.");
        var entity = mapper.Map<Order>(model);
        var cart = await cartService.GetCart();
        ServiceHelper.ThrowIfTrue(cart.Lines.Count < 1, "There are no products in the cart");
        var orderLines = mapper.Map<List<OrderLine>>(cart.Lines);
        entity.CreationDate = DateTime.Now;
        entity.OrderLines = orderLines;
        entity.TotalOrderPrice = cart.TotalCartPrice;

        var addedEntity = await orderRepository.AddAsync(entity);
        var addedEntityWithDetails = await orderRepository.GetByIdWithDetailsAsync(addedEntity.Id);
        return mapper.Map<OrderModel>(addedEntityWithDetails);
    }

    public async Task DeleteAsync(long id)
    {
        await orderRepository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<OrderModel>> GetAllAsync()
    {
        var queryable = orderRepository.GetAllWithDetails();
        var entities = await queryable.ToListAsync();
        var models = mapper.Map<IEnumerable<OrderModel>>(entities);
        return models;
    }

    public async Task<OrderModel?> GetByIdAsync(long id)
    {
        var entity = await orderRepository.GetByIdWithDetailsAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = mapper.Map<OrderModel>(entity);
        return model;
    }

    public Task<OrderModel> UpdateAsync(OrderModel model)
    {
        throw new NotImplementedException();
    }
}
