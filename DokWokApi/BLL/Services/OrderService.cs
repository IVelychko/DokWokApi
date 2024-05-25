using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models;
using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using DokWokApi.Exceptions;

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

    private static void CheckForNull<T>(T? model, string errorMessage)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model), errorMessage);
        }
    }

    public static void ThrowIfTrue(bool value, string errorMessage)
    {
        if (value)
        {
            throw new OrderException(nameof(value), errorMessage);
        }
    }

    public async Task<OrderModel> AddAsync(OrderForm model)
    {
        CheckForNull(model, "The passed model is null.");
        var entity = mapper.Map<Order>(model);
        var cart = await cartService.GetCart();
        ThrowIfTrue(cart.Lines.Count < 1, "There are no products in the cart");
        var orderLines = mapper.Map<List<OrderLine>>(cart.Lines);
        entity.CreationDate = DateTime.Now;
        entity.OrderLines = orderLines;
        entity.TotalOrderPrice = cart.TotalCartPrice;

        var addedEntity = await orderRepository.AddAsync(entity);
        var addedEntityWithDetails = await orderRepository.GetByIdWithDetailsAsync(addedEntity.Id);
        return mapper.Map<OrderModel>(addedEntityWithDetails);
    }

    public Task DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<OrderModel>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<OrderModel?> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<OrderModel> UpdateAsync(OrderModel model)
    {
        throw new NotImplementedException();
    }
}
