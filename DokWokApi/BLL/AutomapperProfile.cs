using AutoMapper;
using DokWokApi.BLL.Models;
using DokWokApi.BLL.Models.Post;
using DokWokApi.BLL.Models.Put;
using DokWokApi.BLL.Models.ShoppingCart;
using DokWokApi.BLL.Models.User;
using DokWokApi.DAL.Entities;

namespace DokWokApi.BLL;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        // Regular models
        CreateMap<ProductCategory, ProductCategoryModel>()
            .ReverseMap();

        CreateMap<Product, ProductModel>()
            .ForMember(pm => pm.CategoryName, opt => opt.MapFrom(p => p.Category != null ? p.Category.Name : string.Empty));
        CreateMap<ProductModel, Product>();

        CreateMap<UserModel, ApplicationUser>()
            .ReverseMap();

        CreateMap<UserRegisterModel, UserModel>();

        CreateMap<UserPutModel, UserModel>();

        CreateMap<Order, OrderModel>();

        CreateMap<OrderForm, Order>();

        CreateMap<OrderLine, OrderLineModel>();

        CreateMap<CartLine, OrderLine>()
            .ForMember(ol => ol.ProductId, opt => opt.MapFrom(cl => cl.Product.Id))
            .ForMember(ol => ol.Product, opt => opt.Ignore());

        // Post models
        CreateMap<ProductCategoryPostModel, ProductCategoryModel>();

        CreateMap<ProductPostModel, ProductModel>();

        // Put models
        CreateMap<ProductCategoryPutModel, ProductCategoryModel>();

        CreateMap<ProductPutModel, ProductModel>();
    }
}
