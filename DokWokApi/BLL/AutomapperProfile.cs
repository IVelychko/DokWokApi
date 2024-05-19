using AutoMapper;
using DokWokApi.BLL.Models;
using DokWokApi.BLL.Models.Post;
using DokWokApi.BLL.Models.Put;
using DokWokApi.BLL.Models.User;
using DokWokApi.DAL.Entities;

namespace DokWokApi.BLL;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<ProductCategory, ProductCategoryModel>()
            .ReverseMap();

        CreateMap<Product, ProductModel>()
            .ForMember(pm => pm.CategoryName, opt => opt.MapFrom(p => p.Category != null ? p.Category.Name : string.Empty));

        CreateMap<ProductModel, Product>();

        CreateMap<ProductCategoryPostModel, ProductCategoryModel>();

        CreateMap<ProductPostModel, ProductModel>();

        CreateMap<ProductCategoryPutModel, ProductCategoryModel>();

        CreateMap<ProductPutModel, ProductModel>();

        CreateMap<UserModel, ApplicationUser>()
            .ReverseMap();

        CreateMap<UserRegisterModel, UserModel>();

        CreateMap<UserPutModel, UserModel>();
    }
}
