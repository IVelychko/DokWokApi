using AutoMapper;
using DokWokApi.BLL.Models.Order;
using DokWokApi.BLL.Models.Product;
using DokWokApi.BLL.Models.ProductCategory;
using DokWokApi.BLL.Models.ShoppingCart;
using DokWokApi.BLL.Models.User;
using DokWokApi.DAL.Entities;

namespace DokWokApi.BLL;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        // Product category
        CreateMap<ProductCategory, ProductCategoryModel>()
            .ReverseMap();
        CreateMap<ProductCategoryPostModel, ProductCategoryModel>();
        CreateMap<ProductCategoryPutModel, ProductCategoryModel>();

        // Product
        CreateMap<Product, ProductModel>()
            .ForMember(pm => pm.CategoryName, opt => opt.MapFrom(p => p.Category != null ? p.Category.Name : string.Empty));
        CreateMap<ProductModel, Product>();
        CreateMap<ProductPostModel, ProductModel>();
        CreateMap<ProductPutModel, ProductModel>();

        // User
        CreateMap<UserModel, ApplicationUser>()
            .ReverseMap();
        CreateMap<UserRegisterModel, UserModel>();
        CreateMap<UserPutModel, UserModel>();

        // Order
        CreateMap<Order, OrderModel>();
        CreateMap<OrderForm, Order>();

        // Order line
        CreateMap<OrderLine, OrderLineModel>();
        CreateMap<CartLine, OrderLine>()
            .ForMember(ol => ol.ProductId, opt => opt.MapFrom(cl => cl.Product.Id))
            .ForMember(ol => ol.Product, opt => opt.Ignore());
    }
}
