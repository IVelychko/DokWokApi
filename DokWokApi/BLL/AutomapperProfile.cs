﻿using AutoMapper;
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
        CreateMap<Order, OrderModel>().ReverseMap();
        CreateMap<OrderForm, OrderModel>();

        // Order line
        CreateMap<OrderLine, OrderLineModel>();
        CreateMap<OrderLineModel, OrderLine>()
            .ForMember(ol => ol.Product, opt => opt.Ignore())
            .ForMember(ol => ol.ProductId, opt => opt.MapFrom(olm => olm.Product != null ? olm.Product.Id : 0));
        CreateMap<CartLine, OrderLineModel>();
    }
}
